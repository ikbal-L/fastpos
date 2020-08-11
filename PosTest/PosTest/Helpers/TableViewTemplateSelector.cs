using PosTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PosTest.Helpers
{
    class TableViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TableFullView { get; set; }
        public DataTemplate TableButtomView { get; set; }
        public DataTemplate SplitView { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            // Null value can be passed by IDE designer
            if (item == null ||
                !(item is TablesViewModel)) 
                return null;

            var tableVM = item as TablesViewModel;
            // Select one of the DataTemplate objects, based on the 
            // value that represents the view kid
            if (tableVM.IsFullView)
            {
                return TableFullView;
            }
            else
            {
                return TableButtomView;
            }
        }
    }
}
