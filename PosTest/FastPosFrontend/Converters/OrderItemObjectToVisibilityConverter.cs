using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ServiceInterface.Model;

namespace FastPosFrontend.Converters
{
    class OrderItemObjectToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderItem item && item.Product.IsPlatter && (item.Product.Additives != null))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }

          
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}