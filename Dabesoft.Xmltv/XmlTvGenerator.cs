using System.Collections.Generic;
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

        //public void AddChannels(IEnumerable<string> names)
        //{
        //    foreach (var name in names)
        //    {
        //        AddChannel(name);
        //    }
        //}

        public void AddChannel(string name, int uid)
        {
            var n = name.Replace(" ", "").Replace("/", "I").Replace("_", "");
            n += ".1";

            _resultBuilder.AppendLine($"<channel id='{n}'>");
            _resultBuilder.AppendLine($"<display-name lang='de'>{name}</display-name>");
            //<icon src="file://C:\Perl\site/share/xmltv/icons/KTVT.gif" />
            //todo make switch in settings for fileformat
            //http://epggw.a1.net/img/station/darkbg/21.svg
            //http://epggw.a1.net/img/station/480x240/1.png
            if (uid != 0)
                _resultBuilder.AppendLine("<icon src='http://epggw.a1.net/img/station/darkbg/480x240/" + uid + ".png' />");
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
                $"<programme start='{DateFormatter(si.Start)}' stop='{DateFormatter(si.End)}' channel='{channelName}'>");
            _resultBuilder.AppendLine($"<title lang='de'>{si.Name}</title>");



            if (!string.IsNullOrWhiteSpace(si.ShortInfo))
                _resultBuilder.AppendLine($"<sub-title lang='de'>{si.ShortInfo}</sub-title>");

            if (!string.IsNullOrWhiteSpace(si.Description))
                _resultBuilder.AppendLine($"<desc lang='de'>{GetUtf8String(si.Description)}</desc>");

            if (si.Year != -1)
                _resultBuilder.AppendLine($"<date>{si.Year}</date>");
            if (!string.IsNullOrEmpty(si.Category))
                _resultBuilder.AppendLine($"<category lang='de'>{si.Category}</category>");


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

            if (offsetHour < 0) //when started via crontab, there is an offset of -24Hours, dont now why :S
                offsetHour += 24; 

            string offsetstring = $" +{offsetHour:00}{offsetMinute:00}";

            //todo
            //offsetstring = "";

            return dtDateTime.ToString("yyyyMMddHHmmss" + offsetstring);
        }

        public void Write(string path)
        {
            _resultBuilder.AppendLine("</tv>");
            var result = _resultBuilder.ToString();
            _resultBuilder.Clear();

            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                sw.Write(result);
            }
        }
    }
}
