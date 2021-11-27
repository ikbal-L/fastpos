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
                return PackIconKind.BasketFill;
            }
            if (orderItemState == OrderItemState.Removed)
            {
                return PackIconKind.BasketUnfill;
            }
            if (orderItemState == OrderItemState.IncreasedQuantity)
            {
                return PackIconKind.BasketPlusOutline;
            }

            if (orderItemState == OrderItemState.DecreasedQuantity)
            {
                return PackIconKind.BasketMinusOutline;
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}