using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using FastPosFrontend.Enums;

namespace FastPosFrontend.Extensions
{
    public class ScreenResolution : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Width != null && Height != null) throw new Exception();
            var result = false;
            if (Width != null)
            {
                result =  Compare(SystemParameters.PrimaryScreenWidth, Width.Value);
            }
            else if( Height != null)
            {
                result =  Compare(SystemParameters.PrimaryScreenHeight, Height.Value);
            }

            

            return Converter?.Convert(result, null, null, null) ?? result;
        }

        private bool Compare(double dimensionValue, double value)
        {
            return  Operator switch
            {
                ComparisonOperator.Equal => dimensionValue == value,
                ComparisonOperator.NotEqual => dimensionValue != value,
                ComparisonOperator.GreaterThan => dimensionValue > value,
                ComparisonOperator.GreaterThanEquals => dimensionValue >= value,
                ComparisonOperator.LesserThan => dimensionValue < value,
                ComparisonOperator.LesserThanEquals => dimensionValue <= value,
                _ => false
            };

        }

        public ComparisonOperator Operator { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }

        public IValueConverter Converter { get; set; } = new BooleanToWindowStateConverter();
    }

    class BooleanToWindowStateConverter:IValueConverter
    {   
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool t && t)
            {
                return WindowState.Maximized;
            }
            return WindowState.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}