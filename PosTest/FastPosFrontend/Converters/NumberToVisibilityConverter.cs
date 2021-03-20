using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FastPosFrontend.Converters
{
    class NumberToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int threshold=0;
            string op=null;

            if (parameter != null)
            {
                string p = parameter as string;
                op = p[0].ToString();
                threshold = System.Convert.ToInt32(p.Remove(0,1));
            }
            //else
            //{
            //    threshold = 0;
            //    op = "=";
            //}
            decimal dvalue;
            try
            {
                dvalue = System.Convert.ToDecimal(value);
            }
            catch (Exception)
            {
                return Visibility.Collapsed;
            }
            switch (op)
            {
                case "=":
                    return dvalue == threshold ? Visibility.Collapsed : Visibility.Visible;
                    
                case "l":
                    return dvalue == threshold ? Visibility.Collapsed : Visibility.Visible;
                    
                case ">":
                    
                default:
                    return dvalue <= threshold ? Visibility.Collapsed : Visibility.Visible;

            }
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
