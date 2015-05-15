﻿using System;
using System.Linq;
using Dabesoft.Xmltv;
using Dabesoft.Xmltv.Models;
using DabeSoft.A1;
using System.Collections.Generic;
using System.IO;
using DabeSoft.A1.Models;
using Newtonsoft.Json;
using A1Json2Xmltv.Models;
using System.Threading;
using log4net.Repository.Hierarchy;
using log4net;
using log4net.Appender;
using log4net.Layout;

namespace A1Json2Xmltv
{
    class Program
    {







        // /R -> Reload avalaible Stations
        private static void Main(string[] args)
        {
            //DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            //start = start.AddSeconds(1431702600);


            //Console.WriteLine(start);
            //Console.WriteLine(start.ToLocalTime());

            //Console.ReadLine();


            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.RemoveAllAppenders(); /*Remove any other appenders*/

            FileAppender fileAppender = new FileAppender();
            fileAppender.AppendToFile = true;
            fileAppender.LockingModel = new FileAppender.MinimalLock();
            fileAppender.File = "log.txt";
            fileAppender.AppendToFile = false;
            PatternLayout pl = new PatternLayout();
            pl.ConversionPattern = "%d [%2%t] %-5p [%-10c]   %m%n%n";
            pl.ActivateOptions();
            fileAppender.Layout = pl;
            fileAppender.ActivateOptions();

            log4net.Config.BasicConfigurator.Configure(fileAppender);

            ILog _log = LogManager.GetLogger(typeof(Program));


            Facade facade = new Facade();
            Settings settings = Settings.GetInstance();

            _log.Info("Starting - getting available Stations");

            List<Station> availableStations;
            if (args.Contains("/R") || !File.Exists(settings.StationListPath) || DateTime.Now - new FileInfo(settings.StationListPath).LastWriteTime > new TimeSpan(settings.StationListUpdateIntervalDays, 0, 0, 0))
            {
                availableStations = facade.GetStations();
                File.WriteAllText(settings.StationListPath, JsonConvert.SerializeObject(availableStations, Formatting.Indented));
            }
            else
            {
                availableStations = JsonConvert.DeserializeObject<List<Station>>(File.ReadAllText(settings.StationListPath));
            }
            _log.Info("got available Stations");


            List<int> idList = availableStations.Where(a => settings.SenderDefinitions.Contains(a.DisplayName)).Select(a => a.UID).ToList();

            if (idList.Count == 0)
            {
                Console.WriteLine("KEINE SENDER DEFINIERT");
                _log.Warn("KEINE SENDER DEFINIERT");
                return;
            }


            TmpOutput tmpOutPut;
            if (File.Exists(settings.TmpOutputPath))
                tmpOutPut = JsonConvert.DeserializeObject<TmpOutput>(File.ReadAllText(settings.TmpOutputPath));
            else
                tmpOutPut = new TmpOutput { Programs = new List<ProgramInfo>(), Stations = new List<Station>() };

            tmpOutPut.Stations = availableStations.Where(a => idList.Contains(a.UID)).ToList();

            DateTime mostRecent = DateTime.MinValue;
            tmpOutPut.Programs.ForEach(p => mostRecent = mostRecent < p.End ? p.End : mostRecent);


            for (int i = 0; i < settings.DaysToLoad; i++)
            {
                DateTime current = DateTime.Now.Date.AddDays(i);

                if (mostRecent.Date > current)
                    continue;
                _log.Info("Getting ProgramInfos for " + current.ToShortDateString());

                var programdatas = facade.GetChannelDatas(idList, current);

                foreach (var pd in programdatas)
                {
                    if (tmpOutPut.Programs.FirstOrDefault(w => w.EventId == pd.EventId) == null)
                        tmpOutPut.Programs.Add(pd);
                }
                File.WriteAllText(settings.TmpOutputPath, JsonConvert.SerializeObject(tmpOutPut, Formatting.Indented));
            }

            int count = tmpOutPut.Programs.Count(w => string.IsNullOrWhiteSpace(w.Description));
            int x = 1;

            WriteXml(tmpOutPut, settings, _log);

            if (settings.GetDescriptions)
            {
                _log.Info("Getting Descriptions with " + settings.RequestsPerMinute + " requests per minute");
                foreach (var item in tmpOutPut.Programs.Where(w => string.IsNullOrWhiteSpace(w.Description)))
                {
                    try
                    {
                        Console.WriteLine("(" + x + "/" + count + ") Getting ProgramInfo for " + item.Name);
                        _log.Info("(" + x + "/" + count + ") Getting ProgramInfo for " + item.Name);
                        var details = facade.GetDescription(item.EventId);

                        item.Description = details.Description ?? "";
                        item.Category = details.Genre ?? "";
                        Thread.Sleep(60000 / settings.RequestsPerMinute);
                    }
                    catch (Exception e)
                    {
                        _log.Error(e);
                        Thread.Sleep(60000);
                    }
                    if (x % 10 == 0)
                    {
                        File.WriteAllText(settings.TmpOutputPath, JsonConvert.SerializeObject(tmpOutPut, Formatting.Indented));
                    }
                    x++;
                }

                File.WriteAllText(settings.TmpOutputPath, JsonConvert.SerializeObject(tmpOutPut, Formatting.Indented));

                _log.Info("Got all data");
                Console.WriteLine("DONE Checking");

                WriteXml(tmpOutPut, settings, _log);
            }
            return;
            //TODO Add Icon to XMLTV
            //TODO VERIFY THAT IT WORKS
            //TODO MORE LOGGING
            //TODO COnfigure Logging
            //TODO MAIL ME IF NOT WORKING
        }


        private static void WriteXml(TmpOutput tmpOutPut, Settings settings, ILog _log)
        {
            var c = new XmltvGenerator();

            _log.Info("Writing XML File");

            foreach (var p in tmpOutPut.Stations)
            {
                c.AddChannel(p.DisplayName);
            }

            foreach (var show in tmpOutPut.Programs)
            {
                c.AddProgramInfos(new ShowInfo
                {
                    Category = show.Category,
                    Year = show.Year,
                    Name = show.Name,
                    Description = show.Description,
                    ShortInfo = show.ShortInfo,
                    End = show.End.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString(), //to linux time string
                    Start = show.Start.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString(), //DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    StationName = show.StationName
                });
            }
            c.Write(settings.OutputPath);
            Console.WriteLine("DONE");
        }


        private static void PrintHelp()
        {
            Console.WriteLine("Kommandos:\r\n" +
                "/I	lädt alle ausgewählten Sender aus dem Internet herunter" +
                "/A	lädt alle verfügbaren Sender in die Settings Datei\r\n" +
                "/?	Gibt diese Hilfeseite aus\r\n" +
                "Ohne Argument -> lädt alle ausgewählten Sender aus der Datenbank in das XMLTV File, bzw. zeigt diese Seite an, falls keine LoadInfo.txt existiert.\r\n\r\n" +
                "Funktionsweise:\r\n" +
                "Alle Sender in der Datei \\A1Json2Xmltv\\LoadInfo.txt werden geladen. Um diese Datei zu erstellen, führen Sie dieses Programm mit dem Argument /A aus.\r\n" +
                "Löschen Sie nun alle Sender aus der \\A1Json2Xmltv\\Sender.txt die Sie nicht benötigen und benennen Sie die Datei in LoadInfo.txt um.\r\n" +
                "Führen Sie das Programm danach erneut aus, die Programminformationen befinden sich dann in der Datei \\A1Json2Xmltv\\tvGuide.xml.\r\n");

            Console.WriteLine();
            Console.WriteLine("Beliebige Taste zum beenden drücken");

        }

    }

}
