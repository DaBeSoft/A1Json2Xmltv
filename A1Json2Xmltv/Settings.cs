using System;
using System.IO;

namespace A1Json2Xmltv
{
    static class Settings
    {

        private static string Settingspath { get { return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location); } }

        public static bool SettingsExist
        {
            get { return File.Exists(Settingspath + "\\LoadInfo.txt"); }
        }

        public static void CreateDefaultSettings()
        {
            if (!Directory.Exists(Settingspath))
                Directory.CreateDirectory(Settingspath);

        }

        static public string GuidePath
        {
            get { return Settingspath + @"\tvguide.xml"; }
        }

        static public string LoadInfo
        {
            get { return Settingspath + @"\LoadInfo.txt"; }
        }

        static public string Sender
        {
            get { return Settingspath + @"\Sender.txt"; }
        }

        static public int HoursToLoad
        {
            get
            {
                if (!File.Exists(Settingspath + "\\HoursToLoad.txt"))
                {
                    using (var sw = new StreamWriter(Settingspath + "\\HoursToLoad.txt", false))
                    {
                        sw.WriteLine("480");
                        sw.WriteLine("In der ersten Zeile darf nur eine Zahl stehen!");
                        sw.WriteLine("Diese Zahl gibt an wie viele Stunden ab jetzt an Programminformationen geladen werden sollen");
                    }
                }
                using (var sr = new StreamReader(Settingspath + "\\HoursToLoad.txt"))
                    return Convert.ToInt32(sr.ReadLine());
            }
        }

    }
}
