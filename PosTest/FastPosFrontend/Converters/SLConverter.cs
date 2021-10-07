using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FastPosFrontend.Converters
{
    public abstract class SLConverter<TInput, TOutput> : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TInput input)
            {
                return Convert(input, targetType, parameter, culture);
            }
            return default(TOutput);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TOutput output)
            {
                return ConvertBack(output, targetType, parameter, culture);
            }
            return default(TInput);
        }

        public abstract TOutput Convert(TInput value, Type targetType, object parameter, CultureInfo culture);
        public abstract TInput ConvertBack(TOutput value, Type targetType, object parameter, CultureInfo culture);
    }
}
