using System;
using System.Globalization;
using System.Windows.Data;
using ServiceInterface.Model;

namespace PosTest.Converters
{
    public class EnumToIntegerIndexValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.ToObject(targetType, value);
        }
    }
}