using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using ServiceInterface.Model;
using PosTest.ViewModels;


namespace PosTest.Converters
{
    class ListKindToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ListKind listkind = (ListKind)value;
            string p = (string)parameter;
            Console.WriteLine("" + listkind+"=="+p);

            if (listkind == ListKind.Customer && p== "customer")
                return Visibility.Visible;
            if (listkind == ListKind.Deliverey && p == "delivrey")
                return Visibility.Visible;
            if (listkind == ListKind.Table && p == "tab")
                return Visibility.Visible;
            if (listkind == ListKind.Waiter && p == "waiter")
                return Visibility.Visible;
            return Visibility.Hidden;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
