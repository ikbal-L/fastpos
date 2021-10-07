using MaterialDesignThemes.Wpf;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FastPosFrontend.Converters
{
    public class OrderTypeToPackIconKindConverter : SLConverter<OrderType, PackIconKind>
    {
        public override PackIconKind Convert(OrderType value, Type targetType, object parameter, CultureInfo culture)
        {
            var icon = value switch
            {


                OrderType.Delivery => PackIconKind.Moped,
                OrderType.OnTable => PackIconKind.TableFurniture,
                OrderType.TakeAway => PackIconKind.Basket,
                _ => PackIconKind.Timer,
            };
            return icon;
        }

        public override OrderType ConvertBack(PackIconKind value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
