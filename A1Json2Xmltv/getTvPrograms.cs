using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace A1Json2Xmltv
{
    internal class GetTvPrograms
    {
        public static void GetSender()
        {
            var json = new WebClient().DownloadString("http://epggw.a1.net/a/api.mobile.station.get?type=JSON.4");
            var sender = new Dictionary<int, string>();

            var all = (IList)JsonConvert.DeserializeObject(json);

            var allSender = (IList)all[2];

            foreach (JArray one in allSender)
            {
                
                var id = one[0].Value<int>();
                var name = one[2].Value<string>();
                sender.Add(id, name);
            }

            var sw = new StreamWriter(Settings.Sender, false);

            foreach (var keyValuePair in sender)
            {
                sw.WriteLine("{0}: {1}", keyValuePair.Key, keyValuePair.Value);
            }

            sw.WriteLine();

        }
    }
}
