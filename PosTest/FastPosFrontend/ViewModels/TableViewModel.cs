using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FastPosFrontend.ViewModels
{
    public class TableViewModel : PropertyChangedBase
    {
        private Order _selectedOrder;

        public Order SelectedOrder
        {
            get { return _selectedOrder; }
            set { Set(ref _selectedOrder , value); }
        }



        public TableViewModel(Table table, ObservableCollection<Order> orders)
        {
            Table = table;
            Orders = orders;
            OrdersViewSource = new CollectionViewSource() { Source = orders };
            OrdersViewSource.Filter += OrdersViewSource_Filter;
        }

        private void OrdersViewSource_Filter(object sender, FilterEventArgs e)
        {
            OrderState?[] filteredStates = { OrderState.Canceled, OrderState.Payed, OrderState.Removed };
            if (e.Item is Order order)
            {
                e.Accepted = order.Table == Table && !filteredStates.Contains((order.State));
            }
        }
        public ObservableCollection<Order> Orders { get; set; }


        public CollectionViewSource OrdersViewSource { get; set; }
        public Table Table { get; }
    }

    public static class TableEx
    {
        public static TableViewModel ToTableViewModel(this Table table, ObservableCollection<Order> orders)
        {
            return new TableViewModel(table,orders);
        }
    }
}
