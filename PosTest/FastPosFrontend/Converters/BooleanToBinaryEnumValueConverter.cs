using System;
using System.Globalization;
using System.Windows.Data;

namespace FastPosFrontend.Converters
{
    public class BooleanToBinaryEnumValueConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolean = (bool)value;
            var binaryValue = (Array) parameter;
            return boolean ? binaryValue.GetValue(1) : binaryValue.GetValue(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}