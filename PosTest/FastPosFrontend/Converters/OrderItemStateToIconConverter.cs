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
                return PackIconKind.BasketPlusOutline;
            }
            if (orderItemState == OrderItemState.Removed)
            {
                return PackIconKind.BasketRemoveOutline;
            }
            if (orderItemState == OrderItemState.IncreasedQuantity)
            {
                return PackIconKind.Add;
            }

            if (orderItemState == OrderItemState.DecreasedQuantity)
            {
                return PackIconKind.Minus;
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}