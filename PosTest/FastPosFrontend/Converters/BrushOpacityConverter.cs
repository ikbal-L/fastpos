﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace FastPosFrontend.Converters
{
    public class BrushOpacityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                var opacity = System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
                return new SolidColorBrush(brush.Color)
                {
                    Opacity = opacity
                };
            }
            return null;
        }

        public object? ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}
