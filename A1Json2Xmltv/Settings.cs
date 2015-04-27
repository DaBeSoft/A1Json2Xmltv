using System;
using System.IO;
using Newtonsoft.Json;

namespace A1Json2Xmltv
{
    public class Settings
    {
        private int _hoursToLoad = 480;
        private string _outputPath = SettingsPath + "/tvguide.xml";

        [JsonProperty(PropertyName = "hoursToLoad")]
        public int HoursToLoad
        {
            get { return _hoursToLoad; }
            set { _hoursToLoad = value; }
        }

        [JsonProperty(PropertyName = "OutputPath")]
        public string OutputPath
        {
            get { return _outputPath; }
            set { _outputPath = value; }
        }

        [JsonProperty(PropertyName = "senderDefinitions")]
        public SenderSetting[] SenderDefinitions { get; set; }

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
            if(_settings == null && !SettingsExist)
                _settings = new Settings();
            return _settings;
        }

        private static Settings LoadSettings()
        {
            using (var sr = new StreamReader(SettingsFilePath))
            {
                var json = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<Settings>(json);
            }
        }

        public static void SaveSettings()
        {
            var set = GetInstance();
            using (var sw = new StreamWriter(SettingsFilePath, false))
            {
                sw.Write(JsonConvert.SerializeObject(set, Formatting.Indented));
            }
        }


        public static void CreateDefaultSettings()
        {
            if (!SettingsExist)
            {
                SaveSettings();
                Console.WriteLine("Default Settings created");
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
