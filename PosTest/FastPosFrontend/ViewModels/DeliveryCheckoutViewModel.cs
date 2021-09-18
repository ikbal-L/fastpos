using Caliburn.Micro;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FastPosFrontend.ViewModels
{
    public class DeliveryCheckoutViewModel:PropertyChangedBase
    {
        private ObservableCollection<Deliveryman> _deliverymanCollection;
        private ObservableCollection<Order> _ordersCollection;
        public DeliveryCheckoutViewModel()
        {
            var data = StateManager.Get<Deliveryman>();
            _deliverymanCollection = new ObservableCollection<Deliveryman>(data);
            DeliverymanCollection = new CollectionViewSource() { Source = _deliverymanCollection };

            var repo = StateManager.GetService<Order, IOrderRepository>();

            OrderState[] states = { OrderState.Delivered, OrderState.DeliveredPaid, OrderState.DeliveredReturned };

            var deliverymanIds = data.Select(d => d.Id.Value);

            var criterias = new OrderFilter() { States = states ,DeliverymanIds = deliverymanIds};
            var orders = repo.GetByCriterias(criterias);
            _ordersCollection = new ObservableCollection<Order>(orders);
            DeliveryOrders = new CollectionViewSource() { Source = _ordersCollection };
            DeliveryOrders.Filter += DeliveryOrders_Filter;
        }

        private void DeliveryOrders_Filter(object sender, FilterEventArgs e)
        {
            if (SelectedDeliveryman == null)
            {
                e.Accepted = false;
                return;
            }

            if (e.Item is Order order && order.DeliverymanId == SelectedDeliveryman.Id)
            {
                e.Accepted = true;
                return;
            }
            e.Accepted = false;
        }

        public CollectionViewSource DeliverymanCollection { get; set; }

        private Deliveryman? _selectedDeliveryman;

        public Deliveryman? SelectedDeliveryman
        {
            get { return _selectedDeliveryman; }
            set 
            { 
                Set( ref _selectedDeliveryman , value);
                DeliveryOrders.View.Refresh();
            }
        }

        public CollectionViewSource DeliveryOrders { get; set; }

        private Order _selectedOrder;

        public Order SelectedOrder
        {
            get { return _selectedOrder; }
            set { Set(ref _selectedOrder , value); }
        }

        private string _selectedFilterOption = DeliveryCheckoutFilter.DELIVERY_MAN;

        public string SelectedFilterOption
        {
            get { return _selectedFilterOption; }
            set 
            { 
                Set(ref _selectedFilterOption, value);
                if (_selectedFilterOption == DeliveryCheckoutFilter.DELIVERY_MAN)
                {
                    SelectedCriteria = DeliverymanFilterCriteria.NAME;
                }
                else
                {
                    SelectedCriteria = OrderFilterCriteria.PRICE;
                    SelectedSecondaryCriteria = OrderFilterCriteria.Secondary.OrderStateCriteria.ALL;
                }
            }
        }


        private string _selectedCriteria = DeliverymanFilterCriteria.NAME;

        public string SelectedCriteria
        {
            get { return _selectedCriteria; }
            set { Set(ref _selectedCriteria, value); }
        }

        private string _selectedSecondaryCriteria = DeliverymanFilterCriteria.NAME;

        public string SelectedSecondaryCriteria
        {
            get { return _selectedSecondaryCriteria; }
            set { Set(ref _selectedSecondaryCriteria, value); }
        }

        private string _filterText;

        public string FilterText
        {
            get { return _filterText; }
            set { Set(ref _filterText, value); }
        }

        public void AddDeliveredOrder(Order order)
        {
            _ordersCollection.Add(order);
            DeliveryOrders.View.Refresh();
        }
    }
}
