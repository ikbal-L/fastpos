using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using ServiceInterface.Model;

namespace FastPosFrontend.ViewModels.SubViewModel
{
    public class DelivereyViewModel : PropertyChangedBase
    {
        private Order _selectedOrder;
        private ICollectionView _orders;
        
        public DelivereyViewModel(CheckoutViewModel checkoutViewModel)
        {
            Parent = checkoutViewModel;
            OrderViewSource = new CollectionViewSource();
            OrderViewSource.Source = Parent.Orders;
            OrderViewSource.Filter += OrderTypeFilter;
            Orders = OrderViewSource.View;
        }
        public CollectionViewSource OrderViewSource { get; set; }
        public void OrderTypeFilter(object sender, FilterEventArgs e)
        {
            Order order = e.Item as Order;
            if (order != null)
            {
                // Filter out products with price 25 or above
                if (order.Type == OrderType.Delivery)
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
        }

        public ICollectionView Orders 
        {
            get => _orders;
            set
            {
                _orders = value;
                NotifyOfPropertyChange();
            }
        } 


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
        public int OrderCount => Orders.Cast<Order>().Count();

    }
}
