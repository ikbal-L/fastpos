using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ServiceInterface.Model;

namespace PosTest.Converters
{
    public class EnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null || !(value is Enum))
                return Visibility.Collapsed;
            string State = value.ToString();
            string parameterString = parameter.ToString();

            foreach (string state in parameterString.Split('|'))
            {
                if (State.Equals(state))
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}