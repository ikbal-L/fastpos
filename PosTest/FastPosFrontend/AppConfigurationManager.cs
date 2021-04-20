using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace FastPosFrontend
{
    public static class AppConfigurationManager
    {
        private static readonly string FileName = "FastPos.Config";

        static AppConfigurationManager()
        {
            var filePath = GetLocalFilePath(FileName);
            if (File.Exists(filePath))
            {
                var configurationString = File.ReadAllText(filePath);
                Configuration = JsonConvert.DeserializeObject<Dictionary<string, string>>(configurationString);
            }
            else
            {
                Configuration = new Dictionary<string, string>();
            }
            
        }
        public static Dictionary<string,string> Configuration { get; set; }

        public static void Save<T>(T configuration)
        {
            var key = typeof(T).Name;
            var value = JsonConvert.SerializeObject(configuration,Formatting.Indented);
            Save(key,value);
            
        }

        public static void Save(string key, object configuration)
        {
            var value = JsonConvert.SerializeObject(configuration, Formatting.Indented);
            Save(key, value);
        }

        public static void Save(string key, string configuration)
        {
            if (Configuration.ContainsKey(key))
            {
                Configuration[key] = configuration;
            }
            else
            {
                Configuration.Add(key, configuration);
            }
            WriteToFile();
        }

        private static string GetLocalFilePath(string fileName)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, fileName);
        }

        private static void WriteToFile()
        {
            var configurationString = JsonConvert.SerializeObject(Configuration, Formatting.Indented);
            File.WriteAllText(GetLocalFilePath(FileName),configurationString);
        }

    }
}