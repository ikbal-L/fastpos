using System;
using System.Globalization;
using System.Windows.Data;

namespace FastPosFrontend.Converters
{
    public class IsStringNullOrEmptyValueConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return string.IsNullOrEmpty(str);
            }

            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}