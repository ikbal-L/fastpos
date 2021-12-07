using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FastPosFrontend.Converters
{
    public class CollectionContainsAnyEqualConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null&& values.Length>=2)
            {
                if (values[0] is ICollection collection && values[1]!= null)
                {
                    bool found = false;
                    foreach (var item in collection)
                    {
                        if (item.Equals(values[1]))
                        {
                            found = true;
                            break;
                        }
                    }
                    return found;
                }
                
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
