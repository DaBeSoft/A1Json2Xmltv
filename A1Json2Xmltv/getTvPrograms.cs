using System.Collections;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace A1Json2Xmltv
{
    internal class GetTvPrograms
    {
        public static int GetSender()
        {
            var json = new WebClient().DownloadString("http://epggw.a1.net/a/api.mobile.station.get?type=JSON.4");
            var sender = new List<SenderSetting>();

            var all = (IList)JsonConvert.DeserializeObject(json);
            var allSender = (IList)all[2];

            foreach (JArray one in allSender)
            {
                
                var id = one[0].Value<int>();
                var name = one[2].Value<string>();
                sender.Add(new SenderSetting(id,name));
            }

            var set = Settings.GetInstance();
            set.SenderDefinitions = sender.ToArray();
            Settings.SaveSettings();
            return sender.Count;
        }
    }
}
