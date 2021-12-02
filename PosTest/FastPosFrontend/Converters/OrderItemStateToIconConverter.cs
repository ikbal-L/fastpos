using System;
using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;
using ServiceInterface.Model;

namespace FastPosFrontend.Converters
{
    public class OrderItemStateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var orderItemState = (OrderItemState) value;
            if (orderItemState == OrderItemState.Added)
            {
                return PackIconKind.PlusThick;
            }
            if (orderItemState == OrderItemState.Removed)
            {
                return PackIconKind.CloseThick;
            }
            if (orderItemState == OrderItemState.IncreasedQuantity)
            {
                return PackIconKind.PlusThick;
            }

            if (orderItemState == OrderItemState.DecreasedQuantity)
            {
                return PackIconKind.WindowMinimize;
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class OrderItemStateToCharConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var orderItemState = (OrderItemState)value;
            if (orderItemState == OrderItemState.Added)
            {
                return '+';
            }
            if (orderItemState == OrderItemState.Removed)
            {
                return 'x';
            }
            if (orderItemState == OrderItemState.IncreasedQuantity)
            {
                return '+';
            }

            if (orderItemState == OrderItemState.DecreasedQuantity)
            {
                return '-';
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}