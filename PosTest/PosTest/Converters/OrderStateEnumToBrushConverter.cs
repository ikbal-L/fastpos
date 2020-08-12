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
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#838383")); //Brushes.Gray; //#4b5d67
            }
            switch ((OrderState)value)
            {
                case OrderState.Delivered:
                    return Brushes.BurlyWood;
                case OrderState.Payed:
                    return Brushes.BlueViolet;
                case OrderState.Ordered:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a35d6a")); // Brushes.Green; //#a35d6a
                case OrderState.Served:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3b5249")); //Brushes.Green; //#3b5249
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
