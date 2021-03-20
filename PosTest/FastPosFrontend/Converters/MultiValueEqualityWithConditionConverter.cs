using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FastPosFrontend.Converters
{
    public class MultiValueEqualityWithConditionConverter: IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3 ) return false;
            var obj1 = values[0];
            var obj2 = values[1];
            var condition = (bool)values[2];

            return (obj1.Equals(obj2) && condition)?Visibility.Visible:Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}