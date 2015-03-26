﻿using System;
using A1Dal;
using Dabesoft.Xmltv;
using Dabesoft.Xmltv.Models;

namespace A1Json2Xmltv
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("DabeSoft's A1 TV Xmltv Creator");


            if ((args.Length == 0 && !Settings.SettingsExist) || (args.Length > 0 && args[0] == "/?"))
            {
                PrintHelp();
                return;
            }

            if ((args.Length > 0 && args[0] == "/A"))
            {
                Settings.CreateDefaultSettings();
                GetTvPrograms.GetSender();
                return;
            }

            Console.WriteLine("Starting");

            var b = new GetProgramInfos();
            b.GetChannelData();

            //todo trigger for get only from db or get from internet
            var d = new Dal();
            foreach (var data in b.Datas)
            {
                var st = d.AddStation(data.Id, data.Name, "");
                foreach (var s in data.Programs)
                {
                    d.AddShow(st, s.EventId, s.Start, s.End, s.Name, s.Category, s.Year, "", "", s.Description, data.Id, s.ShortInfo);
                }
            }




            var c = new XmltvGenerator();
            foreach (var station in d.GetStations())
            {
                c.AddChannel(station.Name);
                foreach (var show in station.Shows)
                {
                    c.AddProgramInfos(new ShowInfo
                    {
                        Category = show.Genre.DvbName,
                        Year = show.Year,
                        Name = show.Name,
                        Description = show.Description,
                        ShortInfo = show.SubName,
                        End = show.End,
                        Start = show.Start
                    });
                }
            }

            Console.WriteLine("Writing Data");
            c.Write(Settings.GuidePath);
        }


        private static void PrintHelp()
        {
            Console.WriteLine("Kommandos:\r\n" +
                "/A	lädt alle verfügbaren Sender in die Datei \\A1Json2Xmltv\\Sender.txt\r\n" +
                "/?	Gibt diese Hilfeseite aus\r\n" +
                "Ohne Argument -> führt das Programm aus, bzw. zeigt diese Seite an, falls keine LoadInfo.txt existiert.\r\n\r\n" +
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
