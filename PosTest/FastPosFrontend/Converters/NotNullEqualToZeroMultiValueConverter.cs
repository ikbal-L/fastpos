using System;
using System.Globalization;
using System.Windows.Data;

namespace FastPosFrontend.Converters
{
    class NotNullEqualToZeroMultiValueConverter:IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var obj = values[0];
            var val = values[1];
            if (val==null)
            {
                return false;
            }
            return (obj != null) && ((decimal)val == 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
