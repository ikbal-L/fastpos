using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using ServiceInterface.Model;
using System.Windows.Media;

namespace PosTest.Converters
{
    class OrderStateEnumToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Brushes.Gray;
            }
            switch ((OrderState)value)
            {
                case OrderState.Delivered:
                    return Brushes.BurlyWood;
                case OrderState.Payed:
                    return Brushes.BlueViolet;
                case OrderState.Ordered:
                    return Brushes.Green;
                default:
                    return Brushes.Gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
