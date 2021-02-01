using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;

namespace PosTest.Converters
{
    public class ProductTemplateHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var conv = new LengthConverter();
            var ordersViewHeight =  (double)conv.ConvertFromString("313px");
            var topBannerHeight = (double)conv.ConvertFromString("85px");
            var extra = (double)conv.ConvertFromString("30px");
            var windowHeight = (double)value;
            var newHeight = (windowHeight - ordersViewHeight - topBannerHeight - extra)/5;
            return newHeight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ProductTemplateWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var conv = new LengthConverter();
            var orderItemListWidth = (double)conv.ConvertFromString("417px");
            var extra = (double)conv.ConvertFromString("50px");
            var windowWidth = (double)value;

            var newWidth = (windowWidth - orderItemListWidth - extra) / 6;
            return newWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}