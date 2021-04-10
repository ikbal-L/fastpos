using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FastPosFrontend.Converters
{
    public class SolidColorBrushToColorConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var solidColorBrush = value as SolidColorBrush;
            return solidColorBrush?.Color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            var solidColorBrush = new SolidColorBrush(color);
            return solidColorBrush;
        }
    }
}