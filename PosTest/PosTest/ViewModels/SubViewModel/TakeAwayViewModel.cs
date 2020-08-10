using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PosTest.ViewModels.SubViewModel
{
    public class TakeawayViewModel : PropertyChangedBase
    {
        private Order _selectedOrder;
        private ICollectionView _orders;

        public TakeawayViewModel(CheckoutViewModel checkoutViewModel)
        {
            Parent = checkoutViewModel;
            TakeawayOrders = new BindableCollection<Order>();
            Orders = CollectionViewSource.GetDefaultView(Parent.Orders);
            Orders.Filter = o => (o as Order).Type == OrderType.Takeaway;

        }
        public BindableCollection<Order> TakeawayOrders { get; set; }
        public CheckoutViewModel Parent { get; set; }

        public ICollectionView Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                NotifyOfPropertyChange();
            }
        }
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                NotifyOfPropertyChange();
            }
        }

        internal void AddOrder(Order order)
        {
            if (!TakeawayOrders.Any(t => t == order))
            {
                TakeawayOrders.Add(order);
            }
        }

        internal void RemoveOrder(Order order)
        {
            if (TakeawayOrders == null || TakeawayOrders.Count == 0)
            {
                return;
            }
            TakeawayOrders.Remove(order);
        }

    }
}
