using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Configuration;

namespace PokemonBattleOnline.PBO
{
    public class Config
    {
        public static void Load()
        {
            var settings = ConfigurationManager.AppSettings;
            foreach (var s in (settings["servers"]??"").Split(';')) Servers.Add(s);
            Name = settings["name"];
            int.TryParse(settings["avatar"], out Avatar);
        }

        public static readonly List<string> Servers = new List<string>();

        public static string Name;

        public static int Avatar;

        private Config()
        {
        }

        public static void Save()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            var key = "servers";
            var value = string.Join(";",Servers);
            if (settings[key] == null) settings.Add(key, value);
            else settings[key].Value = value;
            key = "name";
            value = Name;
            if (settings[key] == null) settings.Add(key, value);
            else settings[key].Value = value;
            key = "avatar";
            value = Avatar.ToString();
            if (settings[key] == null) settings.Add(key, value);
            else settings[key].Value = value;
            configFile.Save(ConfigurationSaveMode.Modified);
            //ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
    }
}
