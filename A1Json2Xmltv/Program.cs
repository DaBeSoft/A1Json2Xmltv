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

namespace A1Json2Xmltv
{
    class Program
    {

        // /R -> Reload avalaible Stations
        private static void Main(string[] args)
        {

            Facade facade = new Facade();
            Settings settings = Settings.GetInstance();


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

            List<int> idList = availableStations.Where(a => settings.SenderDefinitions.Contains(a.DisplayName)).Select(a => a.UID).ToList();

            if(idList.Count == 0)
            {
                Console.WriteLine("KEINE SENDER DEFINIERT");
                Console.ReadLine();
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


            foreach (var item in tmpOutPut.Programs.Where(w => string.IsNullOrWhiteSpace(w.Description)))
            {
                Console.WriteLine("(" + x + "/" + count + ") Getting ProgramInfo for " + item.Name);
                var details = facade.GetDescription(item.EventId);

                item.Description = details.Description ?? "";
                item.Category = details.Genre ?? "";
                Thread.Sleep(60000 / settings.RequestsPerMinute);

                if (x % 10 == 0)
                {
                    File.WriteAllText(settings.TmpOutputPath, JsonConvert.SerializeObject(tmpOutPut, Formatting.Indented));
                }
                x++;
            }

            File.WriteAllText(settings.TmpOutputPath, JsonConvert.SerializeObject(tmpOutPut, Formatting.Indented));


            Console.WriteLine("DONE Checking");
            var c = new XmltvGenerator();

            foreach(var p in tmpOutPut.Stations)
            {
                c.AddChannel(p.DisplayName);
            }

            foreach(var show in tmpOutPut.Programs)
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

            Console.ReadKey();
            return;
            //TODO Add Icon to XMLTV
            //TODO VERIFY THAT IT WORKS
            //TODO MORE LOGGING
            //TODO COnfigure Logging
            //TODO MAIL ME IF NOT WORKING

            //var c = new XmltvGenerator();

            //if (args.Contains("/I"))
            //{
            //    Console.WriteLine("Getting ProgramData from A1...");
            //    var b = new GetProgramInfos();
            //    b.GetChannelData();

            //    foreach (var data in b.Datas)
            //    {
            //        c.AddChannel(data.Name);
            //        //var st = d.AddStation(data.Id, data.Name, "");
            //        foreach (var show in data.Programs)
            //        {
            //            c.AddProgramInfos(new ShowInfo
            //            {
            //                Category = show.Category,
            //                Year = show.Year,
            //                Name = show.Name,
            //                Description = show.Description,
            //                ShortInfo = show.ShortInfo,
            //                End = show.End,
            //                Start = show.Start,
            //                StationName = data.Name
            //            });
            //            //d.AddShow(st, s.EventId, s.Start, s.End, s.Name, s.Category, s.Year, "", "", s.Description, data.Id, s.ShortInfo);
            //        }
            //    }
            //    c.Write(Settings.GetInstance().OutputPath);
            //    //d.Save();
            //}


            ////c.AddChannels(d.GetStations().Select(t => t.Name).ToList());

            ////foreach (var station in d.GetStations())
            ////{
            ////    //c.AddChannel(station.Name);
            ////    foreach (var show in station.Shows)
            ////    {
            ////        try
            ////        {
            ////            c.AddProgramInfos(new ShowInfo
            ////            {
            ////                Category = show.Genre.DvbName,
            ////                Year = show.Year,
            ////                Name = show.Name,
            ////                Description = show.Description,
            ////                ShortInfo = show.SubName,
            ////                End = show.End,
            ////                Start = show.Start,
            ////                StationName = station.Name
            ////            });
            ////        }
            ////        catch (Exception e)
            ////        {
            ////            Console.WriteLine(e.Message);
            ////            Console.WriteLine("Error @" + station.Name + " _ " + show.Name);
            ////        }
            ////    }
            ////}

            ////Console.WriteLine("Writing Data");
            ////c.Write(Settings.GetInstance().OutputPath);
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

            Console.ReadLine();
        }

    }

}
