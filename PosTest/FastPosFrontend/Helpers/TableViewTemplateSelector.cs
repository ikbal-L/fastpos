using System.Windows;
using System.Windows.Controls;
using FastPosFrontend.ViewModels;
using ServiceInterface.Model;

namespace FastPosFrontend.Helpers
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
    
    class SettingsFreeProductsTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SeletedItemTemplate { get; set; }
        public DataTemplate NotSeletedItemTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            // Null value can be passed by IDE designer
            var product = item as Product;
            if (product == null) 
                return null;

            // Select one of the DataTemplate objects, based on the 
            // value that represents the view kid
            if (product.IsSelected)
            {
                return SeletedItemTemplate;
            }
            else
            {
                return NotSeletedItemTemplate;
            }
        }
    }
}
