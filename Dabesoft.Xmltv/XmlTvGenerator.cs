﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using Dabesoft.Xmltv.Models;
using System;

namespace Dabesoft.Xmltv
{
    public class XmltvGenerator
    {
        private readonly StringBuilder _resultBuilder = new StringBuilder();

        public XmltvGenerator()
        {
            _resultBuilder.AppendLine("<?xml version='1.0' encoding='UTF-8'?>");
            _resultBuilder.AppendLine("<tv generator-info-name='DaBe`s Epic A1 EPG Grabber'>");
        }

        public void AddChannels(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                AddChannel(name);
            }
        }

        public void AddChannel(string name)
        {
            var n = name.Replace(" ", "").Replace("/", "I").Replace("_", "");
            n += ".1";

            _resultBuilder.AppendLine(string.Format("<channel id='{0}'>", n));
            _resultBuilder.AppendLine(string.Format("<display-name lang='de'>{0}</display-name>", name));
            _resultBuilder.AppendLine("</channel>");
        }

        public void AddProgramInfos(ShowInfo si)
        {
            //YYYYMMDDhhmmss +0100 for gmt+1

            if (si.Name.Contains("&"))
                si.Name = si.Name.Replace("&", "&amp;");
            if (!string.IsNullOrWhiteSpace(si.ShortInfo) && si.ShortInfo.Contains("&"))
                si.ShortInfo = si.ShortInfo.Replace("&", "&amp;");


            var channelName = si.StationName.Replace(" ", "").Replace("/", "I").Replace("_", "");
            channelName += ".1";

            _resultBuilder.AppendLine(
                string.Format("<programme start='{0}' stop='{1}' channel='{2}'>", DateFormatter(si.Start),
                    DateFormatter(si.End), channelName));
            _resultBuilder.AppendLine(string.Format("<title lang='de'>{0}</title>", si.Name));



            if (!string.IsNullOrWhiteSpace(si.ShortInfo))
                _resultBuilder.AppendLine(string.Format("<sub-title lang='de'>{0}</sub-title>", si.ShortInfo));

            if (!string.IsNullOrWhiteSpace(si.Description))
                _resultBuilder.AppendLine(string.Format("<desc lang='de'>{0}</desc>", GetUtf8String(si.Description)));

            if (si.Year != -1)
                _resultBuilder.AppendLine(string.Format("<date>{0}</date>", si.Year));
            if (!string.IsNullOrEmpty(si.Category))
                _resultBuilder.AppendLine(string.Format("<category lang='de'>{0}</category>", si.Category));


            _resultBuilder.AppendLine("</programme>");
        }

        private static string GetUtf8String(string s)
        {
            var bytes = Encoding.Default.GetBytes(s);
            s = Encoding.UTF8.GetString(bytes);
            return s;
        }

        private static string DateFormatter(string date)
        {
            double d = Convert.ToDouble(date);
            
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(d).ToLocalTime();

            var offsetHour = DateTime.Now.Hour - DateTime.UtcNow.Hour;
            var offsetMinute = DateTime.Now.Minute - DateTime.UtcNow.Minute;

            string offsetstring = string.Format(" +{0:00}{1:00}", offsetHour, offsetMinute);

            return dtDateTime.ToString("yyyyMMddHHmmss" + offsetstring);
        }

        public void Write(string path)
        {
            _resultBuilder.AppendLine("</tv>");
            var result = _resultBuilder.ToString();
            _resultBuilder.Clear();

            using (var sw = new StreamWriter(path, false))
            {
                sw.Write(result);
            }

        }
    }
}
