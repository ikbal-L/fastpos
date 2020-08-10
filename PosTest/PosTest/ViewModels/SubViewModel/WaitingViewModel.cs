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
    public class WaitingViewModel : PropertyChangedBase
    {
        private Order _selectedOrder;
        private ICollectionView _orders;

        public WaitingViewModel(CheckoutViewModel checkoutViewModel)
        {
            Parent = checkoutViewModel;
            WaitingOrders = new BindableCollection<Order>();
            Orders = CollectionViewSource.GetDefaultView(Parent.Orders);
            Orders.Filter = o => (o as Order).Type == OrderType.InWaiting;

        }
        public BindableCollection<Order> WaitingOrders { get; set; }
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
            if (!WaitingOrders.Any(t => t == order))
            {
                WaitingOrders.Add(order);
            }
        }

        internal void RemoveOrder(Order order)
        {
            if (WaitingOrders == null || WaitingOrders.Count == 0)
            {
                return;
            }
            WaitingOrders.Remove(order);
        }
    }
}
