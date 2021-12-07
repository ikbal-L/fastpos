using System;
using System.Globalization;
using System.Windows.Data;

namespace FastPosFrontend.Converters
{
    public class EqualToSelectedAndEmptyMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var selected = values[0];
            var clipboard = values[1];
            var obj = values[2];
            var str = values[3] as string;
            // expect issue
            if (selected == null) return false;

            return (selected.Equals(obj)&& clipboard == null && string.IsNullOrEmpty(str));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class IsEmptyCellSelectedAndClipboardSet : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var selected = values[0];
            var clipboard = values[1];
            var obj = values[2];
            var str = values[3] as string;
            // expect issue
            if (selected == null) return false;

            return (selected.Equals(obj) && clipboard != null && string.IsNullOrEmpty(str));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}