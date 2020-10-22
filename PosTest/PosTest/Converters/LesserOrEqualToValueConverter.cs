using System;
using System.Globalization;
using System.Windows.Data;

namespace PosTest.Converters
{
    public class LesserOrEqualToValueConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int) value;
            int para = (int)val;
            return val <= para;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}