using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using ServiceInterface.Model;

namespace FastPosFrontend.Converters
{
    class OrderItemObjectToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderItem item && item.Product!=null)
            {
                if (item.Product.IsPlatter && item.OrderItemAdditives != null&& item.Product.Additives.Any())
                {
                    return true;
                }
                
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    } 
    public class ObjectToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
