using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ServiceInterface.Model;

namespace PosTest.Converters
{
    class OrderItemObjectToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderItem item && item.Product is Platter && (item.Product as Platter).Additives != null)
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