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
    public class DelivereyViewModel : PropertyChangedBase
    {
        private Order _selectedOrder;
        private ICollectionView _orders;

        public DelivereyViewModel(CheckoutViewModel checkoutViewModel)
        {
            Parent = checkoutViewModel;
            DelivereyOrders = new BindableCollection<Order>();
            //var orders = new CollectionViewSource();
            
            Orders = CollectionViewSource.GetDefaultView(Parent.Orders);
            Orders.Filter = o => (o as Order).Type == OrderType.Delivery;
            //orders.Filter += new FilterEventHandler(ShowOnlyBargainsFilter);
            //Orders = orders.View;


        }

        //public void ShowOnlyBargainsFilter(object sender, FilterEventArgs e)
        //{
        //    Order order = e.Item as Order;
        //    if (order != null)
        //    {
        //        // Filter out products with price 25 or above
        //        if (order.Type == OrderType.Delivery)
        //        {
        //            e.Accepted = true;
        //        }
        //        else
        //        {
        //            e.Accepted = false;
        //        }
        //    }
        //}

        public ICollectionView Orders 
        {
            get => _orders;
            set
            {
                _orders = value;
                NotifyOfPropertyChange();
            }
        } 

        public BindableCollection<Order> DelivereyOrders { get; set; }

        public CheckoutViewModel Parent { get; set; }

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
