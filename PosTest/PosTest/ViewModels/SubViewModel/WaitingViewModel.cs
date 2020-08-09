using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.ViewModels.SubViewModel
{
    public class WaitingViewModel : PropertyChangedBase
    {
        private Order _selectedOrder;

        public WaitingViewModel()
        {
            WaitingOrders = new BindableCollection<Order>();
        }
        public BindableCollection<Order> WaitingOrders { get; set; }
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
