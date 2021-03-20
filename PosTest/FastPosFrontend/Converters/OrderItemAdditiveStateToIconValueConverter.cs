using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;
using ServiceInterface.Model;

namespace FastPosFrontend.Converters
{
    public class OrderItemAdditiveStateToIconValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is Additive additive && values[1] is List<OrderItemAdditive> orderItemAdditives)
            {
                var orderItemAdditive =
                    orderItemAdditives.FirstOrDefault(itemAdditive => additive.Id == itemAdditive.AdditiveId);
                if (orderItemAdditive!= null)
                {
                    if (orderItemAdditive.State == AdditiveState.Added)
                    {
                        return PackIconKind.PlusBox;
                    }
                    if (orderItemAdditive.State == AdditiveState.Removed)
                    {
                        return PackIconKind.MinusBox;
                    } 
                }

                return null;
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}