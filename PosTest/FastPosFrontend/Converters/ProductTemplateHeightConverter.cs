using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using FastPosFrontend.Helpers;
using FastPosFrontend.ViewModels.Settings;
using FastPosFrontend.ViewModels.SubViewModel;

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
            var configuration = AppConfigurationManager.Configuration<ProductLayoutConfiguration>();
            int rows = 5;
            if (configuration!= null)
            {
                rows = configuration.Rows;
            }

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
            
            var configuration = AppConfigurationManager.Configuration<ProductLayoutConfiguration>();
            var cols = 6;
            if (configuration != null)
            {
                cols = configuration.Columns;
            }

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
           
            var configuration = AppConfigurationManager.Configuration<GeneralSettings>();
            int cols = 6;
            if (configuration != null)
            {                                                               
                cols = configuration.NumberCategores;
            }

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