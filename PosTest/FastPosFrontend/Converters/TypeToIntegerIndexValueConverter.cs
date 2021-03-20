using System;
using System.Globalization;
using System.Windows.Data;
using ServiceInterface.Model;

namespace FastPosFrontend.Converters
{
    public class TypeToIntegerIndexValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type type = (Type) value;
            if (type == typeof(Category))
            {
                return 0;
            }
            if (type == typeof(Product))
            {
                return 1;
            }

            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? index = -1;
            if (value != null) index = (int) value;

            if (index ==0)
            {
                return typeof(Category);
            }

            if (index == 1)
            {
                return typeof(Product);
            }

            return null;
        }
    }
}