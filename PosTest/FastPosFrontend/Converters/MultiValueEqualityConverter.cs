﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;
using FastPosFrontend.Enums;
using ServiceInterface.Model;

namespace FastPosFrontend.Converters
{
    public class MultiValueEqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values?.All(o => o?.Equals(values[0]) == true) == true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class MultiValueEqualityConverterWithTestOfNotEmptyProdcut : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0]!=null && (values[0] as Product).Name != null)
            {
                return false;
            }
            return values?.All(o => o?.Equals(values[0]) == true) == true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class MultiValueEqualityConverterWithTestOfEmptyProdcut : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != null && (values[0] as Product).Name == null)
            {
                return false;
            }
            return values?.All(o => o?.Equals(values[0]) == true) == true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class MultiValueEnumListKindConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if( (values[0] is BindableCollection<Table> && (ListKind)values[1]==ListKind.Table) ||
                (values[0] is BindableCollection<Deliveryman> && (ListKind)values[1] == ListKind.Delivery) ||
                (values[0] is BindableCollection<Waiter> && (ListKind)values[1] == ListKind.Waiter)||
                (values[0] is System.ComponentModel.ICollectionView && (ListKind)values[1] == ListKind.Customer)
                
                )
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
