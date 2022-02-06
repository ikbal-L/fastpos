using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace FastPosFrontend.Converters
{
    public class SelectedColorOrDefaultConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length==2 && values[0] is Color selected && values[1] is List<Color> hues && int.TryParse(parameter as string,out var @default))
            {
                if (hues.Contains(selected))
                {
                    return new SolidColorBrush() { Color = selected};
                }
                return new SolidColorBrush() { Color = hues[@default] };
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
