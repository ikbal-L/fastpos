﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FastPosFrontend.Converters
{
    class BooleanToHiddenVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == false)
                return Visibility.Hidden;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Visibility)value==Visibility.Visible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
