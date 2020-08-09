using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace PosTest.Extensions
{
    public class ListPaginationEx
    {
        public static readonly DependencyProperty ItemsPerPageProperty =
                    DependencyProperty.RegisterAttached(
                          "ItemsPerPage", typeof(int),
                          typeof(ListPaginationEx),
                          new PropertyMetadata(null, OnItemsPerPageChanged));

        public static readonly DependencyProperty CurrentPageNumberProperty =
                    DependencyProperty.RegisterAttached(
                          "CurrentPageNumber", typeof(int), typeof(ListPaginationEx),
                          new PropertyMetadata(false, OnCurrentPageNumberChanged));

        public static readonly DependencyProperty NumberOfPagesProperty =
                    DependencyProperty.RegisterAttached(
                          "CurrentPageNumber", typeof(int), typeof(ListPaginationEx),
                          new PropertyMetadata(false, OnNumberOfPagesChanged));

        public static readonly DependencyProperty NextCommandProperty =
            DependencyProperty.RegisterAttached(
                "NextCommand", typeof(ICommand),
                typeof(ListPaginationEx),
                new FrameworkPropertyMetadata
                {
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    DefaultValue = null
                });

        public static readonly DependencyProperty PreviousCommandProperty =
            DependencyProperty.RegisterAttached(
                "PreviousCommand", typeof(ICommand),
                typeof(ListPaginationEx),
                new FrameworkPropertyMetadata
                {
                    DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    DefaultValue = null
                });

        public static readonly DependencyProperty CurrentPageItemsProperty =
            DependencyProperty.RegisterAttached(
                  "SelectedItems", typeof(IList),
                  typeof(ListPaginationEx),
                  new PropertyMetadata(null, OnCurrentPageItemsChanged));

        private static void OnCurrentPageNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private static void OnNumberOfPagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OnCurrentPageItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OnItemsPerPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }


        public static int GetItemsPerPageProperty(DependencyObject d)
        {
            return (int)d.GetValue(ItemsPerPageProperty);
        }
        public static void SetItemsPerPageProperty(DependencyObject d, int value)
        {
            d.SetValue(ItemsPerPageProperty, (int)value);
        }

        public static int GetCurrentPageNumberProperty(DependencyObject d)
        {
            return (int)d.GetValue(CurrentPageNumberProperty);
        }
        public static void SetCurrentPageNumberProperty(DependencyObject d, IList value)
        {
            d.SetValue(CurrentPageNumberProperty, (IList)value);
        }

        public static int GetNumberOfPagesProperty(DependencyObject d)
        {
            return (int)d.GetValue(NumberOfPagesProperty);
        }
        public static void SetNumberOfPagesProperty(DependencyObject d, int value)
        {
            d.SetValue(NumberOfPagesProperty, (int)value);
        }

        public static ICommand GetNextCommandProperty(DependencyObject d)
        {
            return (ICommand)d.GetValue(NextCommandProperty);
        }
        public static void SetNextCommandProperty(DependencyObject d, ICommand value)
        {
            d.SetValue(NextCommandProperty, (ICommand)value);
        }

        public static ICommand GetPreviousCommandProperty(DependencyObject d)
        {
            return (ICommand)d.GetValue(PreviousCommandProperty);
        }
        public static void SetPreviousCommandProperty(DependencyObject d, ICommand value)
        {
            d.SetValue(PreviousCommandProperty, (ICommand)value);
        }

        public static IList Get(DependencyObject d)
        {
            return (IList)d.GetValue(CurrentPageItemsProperty);
        }
        public static void Set(DependencyObject d, IList value)
        {
            d.SetValue(CurrentPageItemsProperty, (IList)value);
        }

    }
}
    
