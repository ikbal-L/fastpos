using System;
using System.Globalization;
using System.Windows.Data;

namespace FastPosFrontend.Converters
{
    public class EntityIdValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           
            return value.GetType().GetProperty("Id") == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}