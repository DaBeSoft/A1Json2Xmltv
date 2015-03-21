using System.Collections.Generic;
using System.IO;
using System.Text;

namespace A1Json2Xmltv
{
    class XmltvGenerator
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
                var n = name.Replace(" ", "").Replace("/", "I").Replace("_", "");
                n += ".1";

                _resultBuilder.AppendLine(string.Format("<channel id='{0}'>", n));
                _resultBuilder.AppendLine(string.Format("<display-name lang='de'>{0}</display-name>", name));
                _resultBuilder.AppendLine("</channel>");
            }
        }

        public void AddProgramInfos(TvData td)
        {
            //YYYYMMDDhhmmss +0100 for gmt+1

            foreach (var prog in td.Programs)
            {
                if (prog.Name.Contains("&"))
                    prog.Name = prog.Name.Replace("&", "&amp;");
                if (!string.IsNullOrWhiteSpace(prog.ShortInfo) && prog.ShortInfo.Contains("&"))
                    prog.ShortInfo = prog.ShortInfo.Replace("&", "&amp;");


                var channelName = td.Name.Replace(" ", "").Replace("/", "I").Replace("_", "");
                channelName += ".1";

                _resultBuilder.AppendLine(
                    string.Format("<programme start='{0}' stop='{1}' channel='{2}'>", DateFormatter(prog.Start), DateFormatter(prog.End), channelName));
                _resultBuilder.AppendLine(string.Format("<title lang='de'>{0}</title>", prog.Name));



                if (!string.IsNullOrWhiteSpace(prog.ShortInfo))
                    _resultBuilder.AppendLine(string.Format("<sub-title lang='de'>{0}</sub-title>", prog.ShortInfo));

                if (!string.IsNullOrWhiteSpace(prog.Description))
                    _resultBuilder.AppendLine(string.Format("<desc lang='de'>{0}</desc>", getUtf8String(prog.Description)));

                if (prog.Year != -1)
                    _resultBuilder.AppendLine(string.Format("<date>{0}</date>", prog.Year));
                if (!string.IsNullOrEmpty(prog.Category))
                    _resultBuilder.AppendLine(string.Format("<category lang='de'>{0}</category>", prog.Category));


                _resultBuilder.AppendLine("</programme>");
            }
        }


        private string getUtf8String(string s)
        {
            var bytes = Encoding.Default.GetBytes(s);
            s = Encoding.UTF8.GetString(bytes);
            return s;
        }

        private static string DateFormatter(string date)
        {
            var ret = "";
            ret += date.Substring(0, date.IndexOf('T'));
            ret += date.Substring(date.IndexOf('T') + 1);
            ret += "00";
            ret += " +0100";
            return ret;
        }

        public void Write()
        {
            _resultBuilder.AppendLine("</tv>");
            var result = _resultBuilder.ToString();
            _resultBuilder.Clear();
            
            using (var sw = new StreamWriter(Settings.GuidePath, false))
            {
                sw.Write(result);
            }

        }
    }
}
