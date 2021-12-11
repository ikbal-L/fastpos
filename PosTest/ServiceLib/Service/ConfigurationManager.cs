using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ServiceLib.Service
{
    public static class ConfigurationManager
    {
        private static readonly string FileName = "pos.appconfig";
        private static Dictionary<string, (string path ,IConfiguration value)> Configurations { get; set; } = new Dictionary<string, (string path, IConfiguration value)>();

        public static void Init<T>(string fileName) where T : IConfiguration,new()
        {
            var key = typeof(T).Name;

            var path = GetLocalFilePath(fileName);
            var content = File.ReadAllText(path);
            var configuration = JsonConvert.DeserializeObject<T>(content)??new T();
            if (!Configurations.ContainsKey(key))
            {
                Configurations.Add(key,(path,configuration));
                if (string.IsNullOrWhiteSpace(content))
                {
                    SaveConfiguration(configuration);
                }
                configuration.SaveRequested += Configuration_SaveRequested;
            }
        }

        private static void Configuration_SaveRequested(object sender, SaveRequestedEventArgs e)
        {
            SaveConfiguration(sender);
        }

        private static void SaveConfiguration(object obj)
        {
            var key = obj.GetType();
            var (path, configuration) = Configurations[key.Name];
            var content = JsonConvert.SerializeObject(configuration,Formatting.Indented);
            File.WriteAllText(path, content);
        }

        public static T Get<T>() where T : IConfiguration
        {
            var key = typeof(T).Name;
            if (Configurations.ContainsKey(key) && Configurations[key].value is T configuration) return configuration;
            return default;
        }

        public static IConfiguration Get(Type configurationType)
        {
            if (Configurations.ContainsKey(configurationType.Name)) return Configurations[configurationType.Name].value;
            return default;
        }

        public static IConfiguration Get(string name)
        {
            if (Configurations.ContainsKey(name)) return Configurations[name].value;
            return default;
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
    }
    public interface IConfiguration: INotifySaveRequested, IForwardRequest
    {
        
    }

    public class Configuration
    {

    }
    public interface IConfigurationProperty :INotifySaveRequested,IRequestSave
    {
        
    }

    public interface IRequestSave
    {
        public void RequestSave();
    }

    public interface IForwardRequest
    {
        public void ForwardRequest(object source);
    }

    public interface INotifySaveRequested
    {
       public event EventHandler<SaveRequestedEventArgs> SaveRequested;
    }

    public class SaveRequestedEventArgs:EventArgs
    {
        public object OriginalSource { get; set; }
    }
    public interface IProperty
    {
        public string Name { get;}

        public IProperty ChildProperty { get; }

        public bool HasChildProperty { get; }
    }

    public class Property : IProperty
    {
        public Property(string name, IProperty childProperty = null)
        {
            Name = name;
            ChildProperty = childProperty;
        }

        public string Name { get; private set; }

        public IProperty ChildProperty { get;  private set; }

        public bool HasChildProperty => ChildProperty != null;

       

        public static IProperty From(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;

            if (!path.Contains(".")) return new Property(path);

            var properties = path.Split('.');
            IProperty root;
            IProperty child = null;

            for (int i = properties.Length-1; i >= 0; i--)
            {
                var property = new Property(properties[i],child);
                child = property;
            }
            root = child;
            child = null;
            return root;
        }

        public static object GetPropertyValueOf(object obj, IProperty property)
        {
            if (property.HasChildProperty)
            {
                var childObj = GetPropertyValueOf(obj, property.Name);
                return GetPropertyValueOf(childObj, property.ChildProperty);
            }
            else
            {
                return GetPropertyValueOf(obj,property.Name);
            }
        }

        public static object GetPropertyValueOf(object obj,string property)
        {
            return obj?.GetType()?.GetProperty(property)?.GetValue(obj);
        }
        
    }
}