using System.Collections.Generic;
using Newtonsoft.Json;
using DabeSoft.A1.Models;
using log4net;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DabeSoft.A1
{
    public class Facade
    {
        ILog _log = LogManager.GetLogger(typeof(Facade));

        public List<Station> GetStations()
        {
            var sender = new List<Station>();
            try
            {
                var wc = new AndroidWebClient();
                var json = wc.DownloadString(A1Settings.GetInstance().StationDataUri);

                var result = JsonConvert.DeserializeObject<Rootobject<StationData>>(json);

                if (result.Head.Status != 200)
                    throw new Exception("GOT STATUS " + result.Head.Status + " - " + result.Head.Message);

                sender.AddRange(result.Data.Select(a => a.Station).ToList());
            }
            catch (Exception e)
            {
                _log.Error("Could not Load Sender List!", e);
            }
            return sender;
        }


        public List<ProgramInfo> GetChannelDatas(IEnumerable<int> ids, DateTime date)
        {
            var dateString = date.ToString("yyyyMMdd");

            string channels = "";
            ids.ToList().ForEach(id => channels += id + "%2C");
            channels = channels.Substring(0, channels.Length - 3);

            var query = string.Format(A1Settings.GetInstance().ChannelDataUri, channels, dateString, A1Settings.GetInstance().HoursToLoad + "H");

            var wc = new AndroidWebClient();
            var json = wc.DownloadString(query);

            var result = (JArray)JsonConvert.DeserializeObject(json);
            var head = ((JArray)result[0]);

            if (head[0].Value<int>() != 200)
                throw new Exception("ERROR - " + head[0].Value<int>());

            result = result[1].Value<JArray>();

            List<ProgramInfo> ChannelDatas = new List<ProgramInfo>();

            foreach (var r in result)
            {
                foreach (var programInfo in r[2])
                {
                    if (!programInfo.HasValues)
                    {
                        _log.Warn("NO VALUE OBJECT - ignoring entry");
                        continue;
                    }
                    DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    start = start.AddSeconds(programInfo[1].Value<int>()).ToLocalTime();

                    DateTime end = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    end = end.AddSeconds(programInfo[2].Value<int>()).ToLocalTime();

                    var pi = new ProgramInfo
                    {
                        StationId = r[0].Value<int>(),
                        StationName = r[1].Value<string>(),
                        EventId = programInfo[0].Value<int>(),
                        Start = start,
                        End = end,
                        Name = programInfo[3].Value<string>(),
                        ShortInfo = programInfo[4].Value<string>(),
                        LustigerBuchstabe = programInfo[6].Value<string>(),
                        Year = -1,
                    };

                    int.TryParse(programInfo[7].Value<string>(), out pi.Year);

                    foreach (var genre in programInfo[5].Values<string>())
                        pi.Genres.Add(genre);

                    ChannelDatas.Add(pi);
                }
            }
            return ChannelDatas;
        }

        public Event GetDescription(int id)
        {
            var query = string.Format(A1Settings.GetInstance().DescriptionUri, id);

            var wc = new AndroidWebClient();
            var json = wc.DownloadString(query);

            var data = JsonConvert.DeserializeObject<Rootobject<ProgramDetail>>(json);

            if (data.Head.Status != 200)
                throw new Exception("GOT STATUS " + data.Head.Status);

            return data.Data[0].Event;
        }
    }
}
