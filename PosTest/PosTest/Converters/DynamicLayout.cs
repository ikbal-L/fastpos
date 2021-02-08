using PosTest.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Newtonsoft.Json;
using PosTest.ViewModels.Settings;
using PosTest.ViewModels.SubViewModel;
using ServiceInterface.Model;

namespace PosTest.Converters
{
    public class DynamicLayout
    {
        
       

        
    }
    public class LengthPropertyConverter : IValueConverter
    {
        public static IList<double> ConvertLengthToDouble(IEnumerable<string> dimensions)
        {
            var conv = new LengthConverter();
            return dimensions.ToList().ConvertAll(input => (double)conv.ConvertFromString(input));
        }

        public static (string propertyName, string[] dimensions) ParseString(string s)
        {
            var res = s.Split(';');
            var propertyName = res[0];
            var dimensions = res[1].Replace("[", "").Replace("]", "").Split(',');
            return (propertyName: propertyName, dimensions: dimensions);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var propertyAndDimensions = (string)parameter;
            var (propertyName, dimensions) = ParseString(propertyAndDimensions);
            var divisor = propertyName switch
            {
                "width" => 6,
                "height" => 5,
                _ => 1
            };

            if (targetType == typeof(Category))
            {
                var Manager = new SettingsManager<GeneralSettings>("GeneralSettings.json");
                var configuration = Manager.LoadSettings();
                if (configuration != null)
                {
                    divisor = int.Parse(configuration.NumberCategores);
                }
            }
            else
            {
                var manager = new SettingsManager<ProductLayoutConfiguration>("product.layout.config");
                var configuration = manager.LoadSettings();
                if (configuration != null)
                {
                    divisor = propertyName =="width" ? configuration.Columns : configuration.Rows;
                }
            }




            var result = ConvertLengthToDouble(dimensions);
            var length = (double)value;

            var newLength = (length - result.Sum()) / divisor;
            return newLength;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}