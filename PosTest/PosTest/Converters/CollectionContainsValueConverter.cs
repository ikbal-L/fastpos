using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using PosTest.ViewModels.SubViewModel;
using ServiceInterface.Model;

namespace PosTest.Converters
{
    public class CollectionContainsValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var product = values[0] as Product; ;
            if (!product.IsPlatter)
            {
                return false;
            }

            var platter = product as Platter;
            var collection = platter.Additives;
            if (collection==null)
            {
                return false;
            }
            var item = values[1] as object;

            return collection.Contains(item);
            //return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            //TODO must provide the convert back value
            return new object[1];
        }
    }
}