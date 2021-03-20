using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using ServiceInterface.Model;

namespace FastPosFrontend.Converters
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
    class OrderTypeEnumToMaterialDesignIcon : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "Lastpass";
            }
            switch ((OrderType)value)
            {
                case OrderType.Delivery:
                    return PackIconKind.Moped;
                case OrderType.OnTable:
                    return PackIconKind.TableFurniture;
                case OrderType.TakeAway:
                    return PackIconKind.BasketOutline;
                case OrderType.InWaiting:
                    return PackIconKind.TimerOutline;
                default:
                    return PackIconKind.Lastpass;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
