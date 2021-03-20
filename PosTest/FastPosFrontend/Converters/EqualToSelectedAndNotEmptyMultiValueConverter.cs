using System;
using System.Globalization;
using System.Windows.Data;

namespace FastPosFrontend.Converters
{
    class EqualToSelectedAndNotEmptyMultiValueConverter: IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var selected = values[0];
            var obj = values[1];
            var str = values[2] as string;
            // expect issue
            if (selected == null) return false;

            return (selected.Equals(obj) && !string.IsNullOrEmpty(str));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
