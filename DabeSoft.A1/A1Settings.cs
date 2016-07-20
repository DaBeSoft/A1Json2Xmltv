using System;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace DabeSoft.A1
{
    public class A1Settings
    {
        private int _hoursToLoad = 24;

        [JsonProperty(PropertyName = "hoursToLoad")]
        public int HoursToLoad
        {
            get { return _hoursToLoad; }
            set { _hoursToLoad = value; }
        }

        private string _stationDataUri = "http://epggw.a1.net/a/api.mobile.station.get?type=JSON.5";

        [JsonProperty(PropertyName = "StationDataUri")]
        public string StationDataUri
        {
            get { return _stationDataUri; }
            set { _stationDataUri = value; }
        }

        private string _channelDataUri = "http://epggw.a1.net/a/api.mobile.event.hour?type=JSON.5&stationuid={0}&period={1}T0000%2F{2}";

        [JsonProperty(PropertyName = "channelDataUri")]
        public string ChannelDataUri
        {
            get { return _channelDataUri; }
            set { _channelDataUri = value; }
        }

        private string _descriptionUri = "http://epggw.a1.net/a/api.mobile.event.get?type=JSON.3&evid={0}";

        [JsonProperty(PropertyName = "descriptionUri")]
        public string DescriptionUri
        {
            get { return _descriptionUri; }
            set { _descriptionUri = value; }
        }


        private static string SettingsPath { get { return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location); } }
        private static string SettingsFilePath { get { return SettingsPath + "/A1Settings.json"; } }
        public static bool SettingsExist { get { return File.Exists(SettingsFilePath); } }

        protected A1Settings()
        {
        }
        
        private static A1Settings _settings;

        public static A1Settings GetInstance()
        {
            if (_settings == null && SettingsExist)
                _settings = LoadSettings();
            if (_settings == null && !SettingsExist)
            {
                _settings = new A1Settings();
                SaveSettings();
            }
            return _settings;
        }

        private static A1Settings LoadSettings()
        {
            using (var sr = new StreamReader(SettingsFilePath, Encoding.UTF8))
            {
                var json = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<A1Settings>(json);
            }
        }

        public static void SaveSettings()
        {
            var set = GetInstance();
            using (var sw = new StreamWriter(SettingsFilePath, false, Encoding.UTF8))
            {
                sw.Write(JsonConvert.SerializeObject(set, Formatting.Indented));
            }
        }
    }
}
