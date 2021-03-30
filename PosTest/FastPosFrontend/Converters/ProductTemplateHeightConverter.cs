using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FastPosFrontend.Helpers;
using FastPosFrontend.ViewModels.Settings;
using FastPosFrontend.ViewModels.SubViewModel;

namespace FastPosFrontend.Converters
{
    public class ProductTemplateHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var Manager = new SettingsManager<ProductLayoutConfiguration>("product.layout.config");
            var configuration = Manager.LoadSettings();
            int rows = 5;
            if (configuration!= null)
            {
                rows = configuration.Rows;
            }


            var conv = new LengthConverter();
            var ordersViewHeight =  (double)conv.ConvertFromString("313px");
            var topBannerHeight = (double)conv.ConvertFromString("85px");
            var extra = (double)conv.ConvertFromString("30px");
            var windowHeight = (double)value;
            var newHeight = (windowHeight - ordersViewHeight - topBannerHeight - extra)/rows;
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
            var Manager = new SettingsManager<ProductLayoutConfiguration>("product.layout.config");
            var configuration = Manager.LoadSettings();
            int cols = 6;
            if (configuration != null)
            {
                cols = configuration.Columns;
            }

            var conv = new LengthConverter();
            var orderItemListWidth = (double)conv.ConvertFromString("417px");
            var extra = (double)conv.ConvertFromString("50px");
            var windowWidth = (double)value;

            var newWidth = (windowWidth - orderItemListWidth - extra) / cols;
            return newWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CategoryTemplateWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var Manager = new SettingsManager<GeneralSettings>("GeneralSettings.json");
            var configuration = Manager.LoadSettings();
            int cols = 6;
            if (configuration != null)
            {                                                               
                cols = configuration.NumberCategores;
            }

            var conv = new LengthConverter();
            var orderItemListWidth = (double)conv.ConvertFromString("417px");
            var paginationButtonsWidth = (double) conv.ConvertFromString("70px");
            var extra = (double)conv.ConvertFromString("50px");
            var windowWidth = (double)value;

            var newWidth = (windowWidth - orderItemListWidth - paginationButtonsWidth - extra) / cols;
            return newWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}