using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PosTest.Extensions
{
    public class ListBoxMultiSelectionEx : DependencyObject
    {
        public static readonly DependencyProperty SelectedItemsProperty =
                    DependencyProperty.RegisterAttached(
                          "SelectedItems", typeof(IList), typeof(ListBoxMultiSelectionEx),
                          new PropertyMetadata(null, OnSelectedItemsChanged));

        public static readonly DependencyProperty IsSortedProperty =
                    DependencyProperty.RegisterAttached(
                          "IsSorted", typeof(bool), typeof(ListBoxMultiSelectionEx),
                          new PropertyMetadata(false, OnIsSortedChanged));

        private static  void OnIsSortedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selectedItems = GetSelectedItems(d);
            if (selectedItems == null || selectedItems.Count <= 0)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                CopySortedSedelectedItems(d);
            }
            else
            {
                CopySedelectedItems(d);
            }
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listBox = d as ListBox;
            var selectedItems = GetSelectedItems(d);
            listBox.SelectionChanged -= SelectionChanged;
            listBox.SelectionChanged += SelectionChanged;
            if (selectedItems == null || selectedItems.Count <= 0)
            {
                return;
            }
            
            for (int i = 0; i < listBox.Items.Count; i++)
            {
                if (selectedItems.Contains(listBox.Items[i]))
                {
                    listBox.UpdateLayout();
                    listBox.ScrollIntoView(listBox.Items[i]);
                    ListBoxItem listBoxItem = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(i);
                    listBoxItem.IsSelected = true;
                }
            }
           
        }

        private static void CopySortedSedelectedItems(DependencyObject d)
        {
            var listBox = d as ListBox;
            var selectedItems = GetSelectedItems(d);

            if (selectedItems == null)
            {
                return;
            }

            selectedItems.Clear();
            var selectedIndexes = new Collection<int>();
            foreach (var item in listBox.SelectedItems)
            {
                selectedIndexes.Add(listBox.Items.IndexOf(item));
            }

            var SortedSelectedIndexes = from index in selectedIndexes
                                        orderby index
                                        select index;

            foreach (var index in SortedSelectedIndexes)
            {
                selectedItems.Add(listBox.Items[index]);
            }
        }

       private static void CopySedelectedItems(DependencyObject d)
        {
            var listBox = d as ListBox;
            var selectedItems = GetSelectedItems(d);
            if (selectedItems == null)
            {
                return;
            }
            selectedItems.Clear();
            foreach (var item in listBox.SelectedItems)
            {
                selectedItems.Add(item);
            }
        }

        static void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            var selectedItems = GetSelectedItems(sender as DependencyObject);           


            if (GetIsSorted(sender as DependencyObject))
            {
                CopySortedSedelectedItems(sender as DependencyObject);
            }
            else
            {

                IList l = (IList)selectedItems.GetType().GetConstructor(new Type[] { }).Invoke(new object[] { });// new BindableCollection<OrderItem>();
                foreach (var item in listBox.SelectedItems)
                {
                    l.Add(item);
                }
                SetSelectedItems(listBox, l);
                //CopySedelectedItems(sender as DependencyObject);
            }
        }

        public static IList GetSelectedItems(DependencyObject d)
        {
            return (IList)d.GetValue(SelectedItemsProperty);
        }
        public static void SetSelectedItems(DependencyObject d, IList value)
        {
            d.SetValue(SelectedItemsProperty, (IList)value);
        }

        public static bool GetIsSorted(DependencyObject d)
        {
            return (bool)d.GetValue(IsSortedProperty);
        }
        public static void SetIsSorted(DependencyObject d, bool value)
        {
            d.SetValue(IsSortedProperty, value);
        }
    }
}
