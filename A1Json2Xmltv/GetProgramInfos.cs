using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using A1Json2Xmltv.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace A1Json2Xmltv
{
    class GetProgramInfos
    {
        public readonly Dictionary<int, string> Senders = new Dictionary<int, string>();
        public readonly List<TvData> Datas = new List<TvData>();

        private int _currentSender;

        private void LoadChannelsToLoad()
        {
            foreach (var sd in Settings.GetInstance().SenderDefinitions)
            {
                Senders.Add(sd.Id, sd.Name);
            }
        }

        private void GetChannelDataForId(int id)
        {
            Console.WriteLine("Getting Channel Data for {0}", Senders[id]);
            var dateString = DateTime.Now.ToString("yyyyMMdd");
            var query =
                string.Format(
                    "http://epggw.a1.net/a/api.mobile.event.hour?type=JSON.1&stationuid={0}&period={1}T0000/{2}",
                    id, dateString, Settings.GetInstance().HoursToLoad + "H");

            var json = new WebClient().DownloadString(new Uri(query));

            var result = (JArray)JsonConvert.DeserializeObject(json);
            result = result[1].Value<JArray>();
            result = result[0].Value<JArray>();
            var td = new TvData { Id = result[0].Value<int>(), Name = result[1].Value<string>() };

            var current = 0;
            var all = result[2].Count();

            //var feResult = Parallel.ForEach(result[2], programInfo =>
            //{
            foreach (var programInfo in result[2])
            {



                var pos = Interlocked.Increment(ref current);
                Console.WriteLine("\t({4}/{5}){0}: Loading Details for {1} {2}/{3}", td.Name,
                    programInfo[3].Value<string>(), pos, all, _currentSender, Senders.Count);


                var moreData = GetDescription(programInfo[0].Value<int>());

                var pi = new ProgramInfo
                {
                    EventId = programInfo[0].Value<int>(),
                    Start = programInfo[1].Value<string>(),
                    End = programInfo[2].Value<string>(),
                    Name = programInfo[3].Value<string>(),
                    ShortInfo = programInfo[4].Value<string>(),
                    LustigerBuchstabe = programInfo[6].Value<string>(),
                    Year = -1,
                    Description = moreData != null ? moreData.Description : "",
                    Category = moreData != null ? moreData.Genre : ""
                };

                int.TryParse(programInfo[7].Value<string>(), out pi.Year);

                foreach (var genre in programInfo[5].Values<string>())
                    pi.Genres.Add(genre);

                td.Programs.Add(pi);
                //});

            }

            //while (!feResult.IsCompleted)
            //    Thread.Sleep(1);
            Datas.Add(td);
        }






        public void GetChannelData()
        {
            Console.WriteLine("Getting Channel Data");
            LoadChannelsToLoad();
            foreach (var i in Senders.Keys)
            {
                _currentSender++;
                GetChannelDataForId(i);
            }
            Console.WriteLine("-> Done");
        }

        private static Event GetDescription(int id)
        {
            try
            {
                var query = string.Format("http://epggw.a1.net/a/api.mobile.event.get?type=JSON.3&evid={0}", id);

                var wc = new WebClient();
                wc.Headers.Add("user-agent", "Mozilla/5.0 (Linux; U; Android 4.4.4; de-at; D5503 Build/14.4.A.0.157) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1 A1TV/3.2");
                //wc.Headers.Add("Accept-Encoding", "gzip");
                wc.Headers.Add("Accept", "application/json");
                var json = wc.DownloadString(query);

                var data = JsonConvert.DeserializeObject<Rootobject<ProgramDetail>>(json);

                return data.data[0].Event;
            }
            catch (Exception e)
            {
                Console.WriteLine("COULDN'T LOAD INFORMATION FOR " + id);
            }

            return null;
        }



    }
}
