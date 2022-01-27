using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using Caliburn.Micro;
using ServiceInterface.Model;

namespace FastPosFrontend.ViewModels.SubViewModel
{
    public class TakeawayViewModel : PropertyChangedBase
    {
        private Order _selectedOrder;
        private ICollectionView _orders;

        public TakeawayViewModel(CheckoutViewModel checkoutViewModel)
        {
            Parent = checkoutViewModel;
            TakeawayOrders = new BindableCollection<Order>();
            OrderViewSource = new CollectionViewSource {Source = Parent.Orders};
     
            OrderViewSource.Filter += OrderTypeFilter;
            Orders = OrderViewSource.View;// CollectionViewSource.GetDefaultView(Parent.Orders);
                                          // Orders.Filter = o => (o as Order).Type == OrderType.Takeaway;
           
        }
        public CollectionViewSource OrderViewSource { get; set; }
        public BindableCollection<Order> TakeawayOrders { get; set; }
        public CheckoutViewModel Parent { get; set; }
        public void OrderTypeFilter(object sender, FilterEventArgs e)
        {
            Order order = e.Item as Order;
            if (order != null)
            {
                OrderState?[] filteredStates = { OrderState.Canceled, OrderState.Payed, OrderState.Removed };
                if (order.Type == OrderType.TakeAway && !filteredStates.Contains((order.State)))
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
