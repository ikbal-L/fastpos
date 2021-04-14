using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FastPosFrontend.Converters
{
    public class CollectionContainsValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = values[0] as IEnumerable<object>;
            if (collection==null)
            {
                return false;
            }
            var item = values[1] as object;

            return collection.ToList().Contains(item);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            //TODO must provide the convert back value
            throw new NotImplementedException();
        }
    }
}