using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
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
                          new PropertyMetadata(false));

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listBox = d as ListBox;
            var selectedItems = GetSelectedItems(d);
            for (int i = 0; i < listBox.Items.Count; i++)
            {
                if (selectedItems.Contains(listBox.Items[i]))
                {
                    listBox.UpdateLayout();
                    ListBoxItem listBoxItem = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(i);
                    listBoxItem.IsSelected = true;
                }
            }
           listBox.SelectionChanged -= SelectionChanged;
           listBox.SelectionChanged += SelectionChanged;
        }

        static void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            var selectedItems = GetSelectedItems(sender as DependencyObject);
            selectedItems.Clear();
            
            if (GetIsSorted(sender as DependencyObject))
            {
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
            else
            {
                foreach (var item in listBox.SelectedItems)
                {
                    selectedItems.Add(item);
                }
            }
            //selectedItemsObj.= listBox.SelectedItems;
            return;
            var added = e.AddedItems;
            var removed = e.AddedItems;
            foreach (var r in removed)
            {
                selectedItems.Remove(r);
            }
            foreach (var a in added)
            {
                selectedItems.Add(a);
            }
        }
        public static IList GetSelectedItems(DependencyObject d)
        {
            return (IList)d.GetValue(SelectedItemsProperty);
        }
        public static void SetSelectedItems(DependencyObject d, IList value)
        {
            d.SetValue(SelectedItemsProperty, value);
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
