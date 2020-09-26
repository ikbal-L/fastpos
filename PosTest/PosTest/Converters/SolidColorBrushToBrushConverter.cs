using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using ServiceInterface.Model;
using System.Windows.Media;

namespace PosTest.Converters
{
    class SolidColorBrushToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            //if (!(value is Color))
            //    throw new InvalidOperationException("Value must be a Color");
            //return new SolidColorBrush((Color)value);

            Color color = ((SolidColorBrush)value).Color;
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color t = (Color)ColorConverter.ConvertFromString(value.ToString());
            SolidColorBrush b = new SolidColorBrush(t);
            return b;
        }
    }
}
