using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace FastPosFrontend.Converters
{
    public class SumValuesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                IEnumerable<int> integers => integers.Sum(),
                IEnumerable<long> longs => longs.Sum(),
                IEnumerable<float> floats => floats.Sum(),
                IEnumerable<double> doubles => doubles.Sum(),
                IEnumerable<decimal> decimals => decimals.Sum(),
                _ => 0
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}