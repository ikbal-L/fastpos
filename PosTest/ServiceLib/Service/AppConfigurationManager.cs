using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ServiceLib.Service
{
    public static class AppConfigurationManager
    {
        private static readonly string FileName = "FastPos.Config";
        private static Dictionary<string, object> Configurations { get; set; }


        static AppConfigurationManager()
        {
            var filePath = GetLocalFilePath(FileName);
            
            if (File.Exists(filePath))
            {
                var configurationString = File.ReadAllText(filePath);
                Configurations = JsonConvert.DeserializeObject<Dictionary<string, object>>(configurationString);
            }
            else
            {
                Configurations = new Dictionary<string, object>();
            }
            
        }
        

        public static void Save<T>(T configuration)
        {
            var key = typeof(T).Name;
            Save(key,configuration);
            
        }

        public static void Save(string key, object configuration)
        {
            if (Configurations.ContainsKey(key))
            {
                Configurations[key] = configuration;
            }
            else
            {
                Configurations.Add(key, configuration);
            }
            WriteToFile();
        }

        

        public static T Configuration<T>(string key=null) 
        {
            key ??= typeof(T).Name ;
            if (!Configurations.ContainsKey(key)) return default;
            if (!(Configurations[key] is JObject value)) return (T) Configurations[key];
            var configurationString = value.ToString();
            return JsonConvert.DeserializeObject<T>(configurationString);

        }
        public static object Configuration(string key)
        {
            return !Configurations.ContainsKey(key) ? default : Configurations[key];
        }
        public static bool ContainsKey(string key)
        {
            return Configurations.ContainsKey(key);
        }
        public static bool ContainsKey<T>()
        {
            return Configurations.ContainsKey(typeof(T).Name);
        }

        private static string GetLocalFilePath(string fileName)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, fileName);
        }

        private static void WriteToFile()
        {
            var configurationString = JsonConvert.SerializeObject(Configurations, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            File.WriteAllText(GetLocalFilePath(FileName),configurationString);
        }

    }
}