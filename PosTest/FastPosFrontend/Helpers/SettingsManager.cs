using System;
using System.IO;
using Newtonsoft.Json;

namespace FastPosFrontend.Helpers
{
    public class SettingsManager<T> where T : class
    {
        private readonly string _filePath;

        public SettingsManager(string fileName)
        {
            _filePath = GetLocalFilePath(fileName);
        }

        private string GetLocalFilePath(string fileName)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, fileName);
        }

        public T LoadSettings() =>File.Exists(_filePath) ? JsonConvert.DeserializeObject<T>(File.ReadAllText(_filePath)) : null;

        public void SaveSettings(T settings)
        {
            string json = JsonConvert.SerializeObject(settings,new JsonSerializerSettings(){NullValueHandling = NullValueHandling.Ignore});
            File.WriteAllText(_filePath, json);
        }
  
    }
}
