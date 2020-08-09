using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.ViewModels.SubViewModel
{
    public class TakeAwayViewModel : PropertyChangedBase
    {
        private Order _selectedOrder;

        public BindableCollection<Order> TakeAwayOrders { get; set; }
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
            if (!TakeAwayOrders.Any(t => t == order))
            {
                TakeAwayOrders.Add(order);
            }
        }

        internal void RemoveOrder(Order order)
        {
            if (TakeAwayOrders == null || TakeAwayOrders.Count == 0)
            {
                return;
            }
            TakeAwayOrders.Remove(order);
        }

    }
}
