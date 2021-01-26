using System;
using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;
using ServiceInterface.Model;

namespace PosTest.Converters
{
    public class OrderItemStateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var orderItemState = (OrderItemState) value;
            if (orderItemState == OrderItemState.Added)
            {
                return PackIconKind.PlusBox;
            }
            if (orderItemState == OrderItemState.Removed)
            {
                return PackIconKind.MinusBox;
            }
            if (orderItemState == OrderItemState.IncreasedQuantity)
            {
                return PackIconKind.PlusBoxMultiple;
            }

            if (orderItemState == OrderItemState.DecreasedQuantity)
            {
                return PackIconKind.MinusBoxMultiple;
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}