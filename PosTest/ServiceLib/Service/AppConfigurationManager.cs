using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ServiceLib.Service
{
    public static class AppConfigurationManager
    {
        private static readonly string FileName = "pos.appconfig";
        private static Dictionary<string, object> Configurations { get; set; }


        static AppConfigurationManager()
        {
            var filePath = GetLocalFilePath(FileName);
            
            if (File.Exists(filePath))
            {
                try
                {
                    var configurationString = File.ReadAllText(filePath);
                    Configurations = JsonConvert.DeserializeObject<Dictionary<string, object>>(configurationString);

                }
                catch (System.Exception e)
                {

                    MessageBox.Show(e.Message);
                }
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
            //if (!(Configurations[key] is JObject value)) return (T) Configurations[key];
            if(!(Configurations[key].GetType() == typeof(JObject) || Configurations[key].GetType() == typeof(JArray))) return (T)Configurations[key];
            var configurationString = Configurations[key].ToString();
            return JsonConvert.DeserializeObject<T>(configurationString);

        }
        public static object Configuration(string key)
        {
            if (!ContainsNestedProperties(key)) return !Configurations.ContainsKey(key) ? default : Configurations[key];
            var sourcePropertyName = GetSourcePropertyName(key);
            var subPath = GetNestedSubPath(key);
            var value = VisitPath(Configurations[sourcePropertyName], subPath);
            return value;
        }
        public static bool ContainsKey(string key)
        {
            if (!ContainsNestedProperties(key))
            {
                return Configurations.ContainsKey(key);
            }
            else
            {
                var sourcePropertyName = GetSourcePropertyName(key);
                var subPath = GetNestedSubPath(key);
                var value = VisitPath(Configurations[sourcePropertyName], subPath);
                return value != null;
            }
        }
        public static bool ContainsKey<T>()
        {
            return Configurations.ContainsKey(typeof(T).Name);
        }

        private static string GetLocalFilePath(string fileName)
        {
            //var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appData = Directory.GetCurrentDirectory();
            return Path.Combine(appData, fileName);
        }

        private static void WriteToFile()
        {
            var configurationString = JsonConvert.SerializeObject(Configurations, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            File.WriteAllText(GetLocalFilePath(FileName),configurationString);
        }

        public static bool ContainsNestedProperties(string propertyName)
        {
            return !string.IsNullOrEmpty(propertyName) && propertyName.Contains(".");
        }

        public static object VisitPath(object source ,string path)
        {
            if (string.IsNullOrEmpty(path)|| source == null) return null;
            
            if (ContainsNestedProperties(path))
            {
                var sourcePropertyName = GetSourcePropertyName(path);
                var nestedSubPath = GetNestedSubPath(path);
                var nested =source.GetType().GetProperty(sourcePropertyName)?.GetValue(source);
               return VisitPath(nested, nestedSubPath);
            }

            if (source is JObject jObject)
            {
                return (jObject[path] as JValue)?.Value;
            }
            return source.GetType().GetProperty(path)?.GetValue(source);
        }


        public static (string source, string nested) GetSourcePropertyNameFromNestedPropertyName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return (null, null);
            var index = propertyName.IndexOf('.');
            var sourcePropertyName = propertyName.Substring(0, index);
            var nestedProperty = propertyName.Substring(index + 1);
            return (sourcePropertyName, nestedProperty);
        }

        public static string GetSourcePropertyName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return null;
            var index = propertyName.IndexOf('.');
            var sourcePropertyName = propertyName.Substring(0, index);
            return sourcePropertyName;
        }

        public static string GetNestedSubPath(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return null;
            var index = propertyName.IndexOf('.');
            var nestedProperty = propertyName.Substring(index + 1);
            return nestedProperty;
        }

        public static object GetNestedPropertyValue(object source, string nestedPropertyName)
        {
            if (source == null || string.IsNullOrEmpty(nestedPropertyName)) return null;
            var value = source.GetType().GetProperty(nestedPropertyName)?.GetValue(source);
            return value;

        }

    }
}