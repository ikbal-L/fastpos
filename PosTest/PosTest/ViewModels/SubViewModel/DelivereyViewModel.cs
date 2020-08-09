using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.ViewModels.SubViewModel
{
    public class DelivereyViewModel : PropertyChangedBase
    {
        private Order _selectedOrder;

        public BindableCollection<Order> DelivereyOrders { get; set; }
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
            if (!DelivereyOrders.Any(t => t == order))
            {
                DelivereyOrders.Add(order);
            }
        }

        internal void RemoveOrder(Order order)
        {
            if (DelivereyOrders == null || DelivereyOrders.Count == 0)
            {
                return;
            }
            DelivereyOrders.Remove(order);
        }
    }
}
