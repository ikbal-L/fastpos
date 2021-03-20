using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using ServiceInterface.Model;

namespace FastPosFrontend.Converters
{
    class NotCollectionItemAnyGreaterThanZeroValueConverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value==null)
            {
                return true;
            }
            var collection = (value as IEnumerable<OrderItem>).ToList();
            
           return !collection.Any((item => item.DiscountAmount > 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
