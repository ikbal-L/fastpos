using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using FastPosFrontend.Configurations;
using ServiceLib.Service;

namespace FastPosFrontend.Converters
{
    public class ProductTemplateHeightConverter : IValueConverter
    {
        private readonly LengthConverter _lengthConverter = new LengthConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var conv = new LengthConverter();
            var values = (parameter as string)?.Split(',')
                .Select(s =>
                {
                    var length = _lengthConverter.ConvertFromString(s);
                    if (length != null) return (double)length;
                    return default;
                });
            var configuration = ConfigurationManager.Get<PosConfig>().ProductLayout;
            int rows = configuration.Rows;
         
            var windowHeight = (double)value;
            var newHeight = (windowHeight - values.Sum())/rows;
            return newHeight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ProductTemplateWidthConverter : IValueConverter
    {
        private LengthConverter _lengthConverter;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            var configuration = ConfigurationManager.Get<PosConfig>().ProductLayout;
            var cols = configuration.Columns;

            _lengthConverter = new LengthConverter();
            var values = (parameter as string)?.Split(',')
                .Select(s =>
                {
                    var length = _lengthConverter.ConvertFromString(s);
                    if (length != null) return (double)length ;
                    return default;
                });
            
            var windowWidth = (double)value;

            var newWidth = (windowWidth - values.Sum()) / cols;
            return newWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CategoryTemplateWidthConverter : IValueConverter
    {
        private readonly LengthConverter _lengthConverter = new LengthConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           
            var configuration = ConfigurationManager.Get<PosConfig>().General;
            int cols = cols = configuration.CategoryPageSize;

            var conv = new LengthConverter();
            var orderItemListWidth = (double)conv.ConvertFromString("417px");
            var paginationButtonsWidth = (double) conv.ConvertFromString("70px");
            var extra = (double)conv.ConvertFromString("50px");

            var values = (parameter as string)?.Split(',')
                .Select(s =>
                {
                    var length = _lengthConverter.ConvertFromString(s);
                    if (length != null) return (double)length;
                    return default;
                });
            var windowWidth = (double)value;

            var newWidth = (windowWidth - values.Sum()) / cols;
            return newWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}