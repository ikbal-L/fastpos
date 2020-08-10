using System;
using System.Globalization;
using System.Windows.Data;
using ServiceInterface.Model;

namespace PosTest.Converters
{
    class ObjectToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderItem)
            {
                if (value is OrderItem item && item.Product is Platter && (item.Product as Platter).Additives != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
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
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
