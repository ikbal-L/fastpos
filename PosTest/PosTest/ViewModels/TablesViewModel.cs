using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PosTest.ViewModels
{
    public class TablesViewModel : PropertyChangedBase
    {
        public TablesViewModel(CheckoutViewModel checkoutViewModel)
        {
            Parent = checkoutViewModel;
            IsFullView = false;
            TablesViewSource = new CollectionViewSource();
            TablesViewSource.Source = Parent.Tables;
            TablesViewSource.Filter += TablesFilter;
            TablesView = TablesViewSource.View;
        }

        public void TablesFilter(object sender, FilterEventArgs e)
        {
            Table table = e.Item as Table;
            if (table != null)
            {
                // Filter out products with price 25 or above
                if (table.Orders.Cast<Order>().Count() > 0)
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
        }
        public CollectionViewSource TablesViewSource { get; set; }

        public ICollectionView TablesView { get; set; }
        public CheckoutViewModel Parent { get; set; }
        public bool IsFullView { get; set; }
    
        public void BackCommand()
        {
            Parent.IsDialogOpen = false;
            Parent.DialogViewModel = null;
        }
    }
}
