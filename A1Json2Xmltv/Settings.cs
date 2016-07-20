using System;
using System.IO;
using Newtonsoft.Json;
using A1Json2Xmltv.Models;
using System.Collections.Generic;
using System.Text;

namespace A1Json2Xmltv
{
    public class Settings
    {
        private bool _getDescriptions = false;

        [JsonProperty(PropertyName = "getDescriptions")]
        public bool GetDescriptions
        {
            get { return _getDescriptions; }
            set { _getDescriptions = value; }
        }

        private int _requestsPerMinute = 10;

        [JsonProperty(PropertyName = "requestsPerMinute")]
        public int RequestsPerMinute
        {
            get { return _requestsPerMinute; }
            set { _requestsPerMinute = value; }
        }

        private int _daysToLoad = 10;

        [JsonProperty(PropertyName = "daysToLoad")]
        public int DaysToLoad
        {
            get { return _daysToLoad; }
            set { _daysToLoad = value; }
        }

        private int _stationListUpdateIntervalDays = 5;

        [JsonProperty(PropertyName = "stationListUpdateIntervalDays")]
        public int StationListUpdateIntervalDays
        {
            get { return _stationListUpdateIntervalDays; }
            set { _stationListUpdateIntervalDays = value; }
        }

        private string _outputPath = SettingsPath + "/tvguide.xml";

        [JsonProperty(PropertyName = "outputPath")]
        public string OutputPath
        {
            get { return _outputPath; }
            set { _outputPath = value; }
        }

        private string _tmpOutputPath = SettingsPath + "/tvguidetmp.json";

        [JsonProperty(PropertyName = "tmpOutputPath")]
        public string TmpOutputPath
        {
            get { return _tmpOutputPath; }
            set { _tmpOutputPath = value; }
        }

        private string _stationListPath = SettingsPath + "/StationList.json";

        [JsonProperty(PropertyName = "stationListPath")]
        public string StationListPath
        {
            get { return _stationListPath; }
            set { _stationListPath = value; }
        }


        List<string> _senderDefinitions = new List<string>();

        [JsonProperty(PropertyName = "senderDefinitions")]
        public List<string> SenderDefinitions {
            get { return _senderDefinitions; }
            set { _senderDefinitions = value; }
        }

        private static string SettingsPath { get { return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location); } }
        private static string SettingsFilePath { get { return SettingsPath + "/Settings.json"; } }

        public static bool SettingsExist { get { return File.Exists(SettingsFilePath); } }

        protected Settings()
        {
            
        }
        
        private static Settings _settings;

        public static Settings GetInstance()
        {
            if (_settings == null && SettingsExist)
                _settings = LoadSettings();
            if (_settings == null && !SettingsExist)
            {
                _settings = new Settings();
                SaveSettings();
            }
            return _settings;
        }

        private static Settings LoadSettings()
        {
            using (var sr = new StreamReader(SettingsFilePath, Encoding.UTF8))
            {
                var json = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<Settings>(json);
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

    public class SenderSetting
    {
        public SenderSetting(int id, string name)
        {
            Id = id;
            Name = name;
        }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

    }
}
