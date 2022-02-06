using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace FastPosFrontend.Converters
{
    public class ColorContrastConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush)
            {
                var color = (value as SolidColorBrush).Color;
                const double threshold = (3 * 255) / 2;
                int sum = color.R + color.G + color.B;
                return new SolidColorBrush(sum > threshold ? Colors.Black : Colors.White);
            }

            if (value is Color selectedColor)
            {
                const double threshold = (3 * 255) / 2;
                int sum = selectedColor.R + selectedColor.G + selectedColor.B;
                return new SolidColorBrush(sum > threshold ? Colors.Black : Colors.White);
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
