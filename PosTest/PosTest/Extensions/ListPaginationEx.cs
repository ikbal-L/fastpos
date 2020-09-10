using PosTest.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
                          new PropertyMetadata(0, OnItemsPerPageChanged));

        public static readonly DependencyProperty CurrentPageNumberProperty =
                    DependencyProperty.RegisterAttached(
                          "CurrentPageNumber", typeof(int), typeof(ListPaginationEx),
                          new PropertyMetadata(-1, OnCurrentPageNumberChanged));

        public static readonly DependencyProperty NumberOfPagesProperty =
                    DependencyProperty.RegisterAttached(
                          "NumberOfPages", typeof(int), typeof(ListPaginationEx),
                          new PropertyMetadata(0, OnNumberOfPagesChanged));

        public static readonly DependencyProperty NextCommandProperty =
            DependencyProperty.RegisterAttached(
                "NextCommand", typeof(ICommand),
                typeof(ListPaginationEx),
                 new PropertyMetadata(null, null));

        //public static readonly DependencyProperty NextCommandProperty =
        //    DependencyProperty.RegisterAttached(
        //        "NextCommand", typeof(ICommand),
        //        typeof(ListPaginationEx),
        //        new FrameworkPropertyMetadata
        //        {
        //            DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        //            DefaultValue = null
        //        });

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
                  "SelectedItems", typeof(IEnumerable),
                  typeof(ListPaginationEx),
                  new PropertyMetadata(null, OnCurrentPageItemsChanged));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached(
                  "ItemsSource", typeof(IEnumerable),
                  typeof(ListPaginationEx),
                  new PropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty CanExecuteMextProperty =
            DependencyProperty.RegisterAttached(
                  "CanExecuteMext", typeof(bool),
                  typeof(ListPaginationEx),
                  new PropertyMetadata(false, null));

        public static readonly DependencyProperty CanExecutePreviousProperty =
            DependencyProperty.RegisterAttached(
                  "CanExecutePrevious", typeof(bool),
                  typeof(ListPaginationEx),
                  new PropertyMetadata(false, null));

        static int oldPageNumber = -1, newPageNumber = -1;

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listBox = d as ListBox;
            var itemsSource = GetItemsSource(d);
            SetNextCommand(d, new DelegateCommandBase((_) => Paginate(d, NextOrPrevious.Next)));
            SetPreviousCommand(d, new DelegateCommandBase((_) => Paginate(d, NextOrPrevious.Previous)));
            var nbrPages = itemsSource.Cast<object>().Count() % GetItemsPerPage(d) == 0 ? 
                itemsSource.Cast<object>().Count()/GetItemsPerPage(d) : itemsSource.Cast<object>().Count() / GetItemsPerPage(d) + 1;
            SetNumberOfPages(d, nbrPages);
            //listBox.ItemsSource = itemsSource;
            Paginate(d, NextOrPrevious.First);
        }

        private static void OnCurrentPageNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int currentPage = GetCurrentPageNumber(d);
            int queriedpage = (int)e.NewValue;
            if (queriedpage > GetNumberOfPages(d) ||
                queriedpage < 1)
            {
                SetCurrentPageNumber(d, (int)e.OldValue);
                return;
            }

            var listBox = d as ListBox;
            var itemsPerPage = GetItemsPerPage(d);
            var itemsSource = GetItemsSource(d);
            if (itemsSource.Cast<object>().Count() <= itemsPerPage)
            {
                listBox.ItemsSource = itemsSource;
                oldPageNumber = GetCurrentPageNumber(d);
                SetCurrentPageNumber(d, 1);
                newPageNumber = GetCurrentPageNumber(d);
                SetCanExecuteMext(d, false);
                SetCanExecutePrevious(d, false);
                //CanExecuteMext = false;
                //CanExecutePrevious = false;
                return;
            }

            var listObjects = itemsSource.Cast<object>().ToList();
            int lastIndexForThisPage = (GetCurrentPageNumber(d)) * itemsPerPage;
            if (listObjects.Count > lastIndexForThisPage)
            {
                listObjects = listObjects.GetRange((GetCurrentPageNumber(d)-1) * itemsPerPage, itemsPerPage);
                SetCanExecuteMext(d, true);
                //CanExecuteMext = true;
            }
            else
            {
                listObjects = listObjects.GetRange((GetCurrentPageNumber(d) - 1) * itemsPerPage, listObjects.Count - ((GetCurrentPageNumber(d) - 1) * itemsPerPage));
                SetCanExecuteMext(d, false);
                //CanExecuteMext = false;
            }
            listBox.ItemsSource = listObjects;
            SetCanExecutePrevious(d, GetCurrentPageNumber(d) == 1 ? false : true);

        }

        private static void OnNextCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void OnNumberOfPagesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SetNumberOfPages(d, (int)e.NewValue);
        }

        private static void OnCurrentPageItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OnItemsPerPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private static void Paginate(DependencyObject d, NextOrPrevious nextOrprevious)
        {
            switch (nextOrprevious)
            {
                case NextOrPrevious.Next:
                    SetCurrentPageNumber(d, GetCurrentPageNumber(d) + 1);
                    break;
                case NextOrPrevious.Previous:
                    SetCurrentPageNumber(d, GetCurrentPageNumber(d) - 1);
                    break;
                case NextOrPrevious.First:
                    SetCurrentPageNumber(d, 1);
                    break;
                default:
                    break;
            }
            return;
            int currentPage = -1;
            if (nextOrprevious == NextOrPrevious.First)
            {
                //SetCurrentPageNumber(d, 0);
                currentPage = 0;
                nextOrprevious = NextOrPrevious.Next;
                SetCanExecuteMext(d, false);
                SetCanExecutePrevious(d, false);
            }
            else
            {
                currentPage = GetCurrentPageNumber(d);
            }
            var listBox = d as ListBox;
            var itemsPerPage = GetItemsPerPage(d);
            var itemsSource = GetItemsSource(d);
            if (itemsSource.Cast<object>().Count() <= itemsPerPage)
            {
                listBox.ItemsSource = itemsSource;
                oldPageNumber = GetCurrentPageNumber(d);
                SetCurrentPageNumber(d, 1);
                newPageNumber = GetCurrentPageNumber(d);
                SetCanExecuteMext(d, false);
                SetCanExecutePrevious(d, false);
                //CanExecuteMext = false;
                //CanExecutePrevious = false;
                return;
            }

            var listObjects = itemsSource.Cast<object>().ToList();
            int lastIndexForThisPage;
            lastIndexForThisPage = (GetCurrentPageNumber(d) + 1) * itemsPerPage;
            if (nextOrprevious == NextOrPrevious.Next)
            {
                if (listObjects.Count > lastIndexForThisPage)
                {
                    listObjects = listObjects.GetRange(currentPage * itemsPerPage, itemsPerPage);
                    SetCanExecuteMext(d, true);
                    //CanExecuteMext = true;
                }
                else
                {
                    listObjects = listObjects.GetRange(currentPage * itemsPerPage, listObjects.Count - (GetCurrentPageNumber(d) * itemsPerPage));
                    SetCanExecuteMext(d, false);
                    //CanExecuteMext = false;
                }
                oldPageNumber = GetCurrentPageNumber(d);
                SetCurrentPageNumber(d, currentPage + 1);
                newPageNumber = GetCurrentPageNumber(d);
            }
            else
            {
                if ((currentPage - 2) * itemsPerPage < 0)
                    return;
                listObjects = listObjects.GetRange((GetCurrentPageNumber(d) - 2) * itemsPerPage, itemsPerPage);
                oldPageNumber = GetCurrentPageNumber(d);
                SetCurrentPageNumber(d, currentPage - 1); //_pageNumber--;
                newPageNumber = GetCurrentPageNumber(d);
                SetCanExecuteMext(d, true);
                //CanExecuteMext = true;

            }
            listBox.ItemsSource = listObjects;

            SetCanExecutePrevious(d, GetCurrentPageNumber(d) == 1 ? false : true);
        }

        public static int GetItemsPerPage(DependencyObject d)
        {
            return (int)d.GetValue(ItemsPerPageProperty);
        }
        public static void SetItemsPerPage(DependencyObject d, int value)
        {
            d.SetValue(ItemsPerPageProperty, (int)value);
        }

        public static int GetCurrentPageNumber(DependencyObject d)
        {
            return (int)d.GetValue(CurrentPageNumberProperty);
        }
        public static void SetCurrentPageNumber(DependencyObject d, int value)
        {
            d.SetValue(CurrentPageNumberProperty, (int)value);
        }

        public static int GetNumberOfPages(DependencyObject d)
        {
            return (int)d.GetValue(NumberOfPagesProperty);
        }
        public static void SetNumberOfPages(DependencyObject d, int value)
        {
            d.SetValue(NumberOfPagesProperty, (int)value);
        }

        public static ICommand GetNextCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(NextCommandProperty);
        }
        public static void SetNextCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(NextCommandProperty, (ICommand)value);
        }

        public static ICommand GetPreviousCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(PreviousCommandProperty);
        }
        private static void SetPreviousCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(PreviousCommandProperty, (ICommand)value);
        }

        public static IEnumerable GetCurrentPageItems(DependencyObject d)
        {
            return (IEnumerable)d.GetValue(CurrentPageItemsProperty);
        }
        public static void SetCurrentPageItems(DependencyObject d, IEnumerable value)
        {
            d.SetValue(CurrentPageItemsProperty, (IEnumerable)value);
        }

        public static IEnumerable GetItemsSource(DependencyObject d)
        {
            return (IEnumerable)d.GetValue(ItemsSourceProperty);
        }
        public static void SetItemsSource(DependencyObject d, IEnumerable value)
        {
            d.SetValue(ItemsSourceProperty, (IEnumerable)value);
        }
        public static bool GetCanExecuteMext(DependencyObject d)
        {
            return (bool)d.GetValue(CanExecuteMextProperty);
        }
        private static void SetCanExecuteMext(DependencyObject d, bool value)
        {
            d.SetValue(CanExecuteMextProperty, value);
        }

        public static bool GetCanExecutePrevious(DependencyObject d)
        {
            return (bool)d.GetValue(CanExecutePreviousProperty);
        }
        private static void SetCanExecutePrevious(DependencyObject d, bool value)
        {
            d.SetValue(CanExecutePreviousProperty, value);
        }

    }

    internal enum NextOrPrevious
    {
        Next,
        Previous,
        First
    }
}
    
