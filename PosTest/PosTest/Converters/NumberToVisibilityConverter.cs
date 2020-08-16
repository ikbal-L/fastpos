using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PosTest.Converters
{
    class NumberToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int threshold;
            string op=null;
            if ((string)parameter == "#0")
            {
                threshold = 0;
                op = "eq";
            }
            else
            {
                threshold = parameter==null ? 0 : int.Parse((string)parameter);
            }
            decimal dvalue;
            try
            {
                dvalue = System.Convert.ToDecimal(value);
            }
            catch (Exception)
            {
                return Visibility.Collapsed;
            }
            if (op == "eq")
            {
                if (dvalue == threshold)
                {
                    return Visibility.Collapsed;
                }
            }
            else
            {
                if (dvalue <= threshold)
                {
                    return Visibility.Collapsed;
                }
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    class NumberToVisibilityConverterVisibleHidden : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal dvalue;
            try
            {
                dvalue = System.Convert.ToDecimal(value);
            }
            catch (Exception)
            {
                return Visibility.Hidden;
            }
            var threshold = parameter==null ? 0 : int.Parse((string)parameter);
            if (dvalue <= threshold)
            {
                return Visibility.Hidden;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
