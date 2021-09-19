using Caliburn.Micro;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
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
using static FastPosFrontend.ViewModels.DeliveryAccounting.DeliveryAccountingViewModel;

namespace FastPosFrontend.ViewModels
{
    [NavigationItem("Delivery Checkout", typeof(DeliveryCheckoutViewModel), isQuickNavigationEnabled:true)]
    public class DeliveryCheckoutViewModel:LazyScreen
    {
        private ObservableCollection<Deliveryman> _deliverymanCollection;
        private ObservableCollection<Order> _ordersCollection;
        private ObservableCollection<Payment> _paymentsCollection;
        public DeliveryCheckoutViewModel()
        {
            Setup();
            OnReady();
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
        public CollectionViewSource DeliveryPayments { get; set; }


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

        private string _numericZone ;

        public string NumericZone
        {
            get { return _numericZone; }
            set {  _numericZone = value; }
        }


        public void AddDeliveredOrder(Order order)
        {
            _ordersCollection.Add(order);
            DeliveryOrders.View.Refresh();
        }

        public void ActionKeyboard(ActionButton cmd)
        {

            switch (cmd)
            {
                case ActionButton.Backspase:
                    NumericZone = String.IsNullOrEmpty(NumericZone)
                        ? String.Empty
                        : NumericZone.Remove(NumericZone.Length - 1);
                    break;


                case ActionButton.Enter:

                    PayementAction();
                    //RelaodDeliveryMan();
                    
                    break;

                    //EditPayment();
                    //RelaodDeliveryMan();


            }
        }

        private void PayementAction()
        {

            var payedAmount = Convert.ToDecimal(NumericZone);
            if (payedAmount < 0)
            {
                NumericZone = "";
                return;
            }

            var res = StateManager.Save(new Payment() { Amount = payedAmount, Date = DateTime.Now, DeliveryManId = SelectedDeliveryman.Id.Value });
            if (res)
            {
                NumericZone = "";
            }


        }

        protected override void Setup()
        {
           

           
            var data = StateManager.Get<Deliveryman>();
            _deliverymanCollection = new ObservableCollection<Deliveryman>(data);
            DeliverymanCollection = new CollectionViewSource() { Source = _deliverymanCollection };

            var orderRepo = StateManager.GetService<Order, IOrderRepository>();
            var paymentRepo = StateManager.GetService<Payment, IPaymentRepository>();

            OrderState[] states = { OrderState.Delivered, OrderState.DeliveredPaid, OrderState.DeliveredReturned };

            var deliverymanIds = data.Select(d => d.Id.Value);

            var criterias = new OrderFilter() { States = states, DeliverymanIds = deliverymanIds };
            var orders = orderRepo.GetByCriteriasAsync(criterias);
            var payments = paymentRepo.GetByCriteriasAsync(new PaymentFilter() { DeliverymanIds = deliverymanIds });

            _data = new NotifyAllTasksCompletion(orders,
               payments);


            
        }

        public override void Initialize()
        {
            var orders = _data.GetResult<List<Order>>();
            var payments = _data.GetResult<List<Payment>>();

            _ordersCollection = new ObservableCollection<Order>(orders);
            _paymentsCollection = new ObservableCollection<Payment>(payments);

            DeliveryOrders = new CollectionViewSource() { Source = _ordersCollection };
            DeliveryPayments = new CollectionViewSource() { Source = _paymentsCollection };

            DeliveryOrders.Filter += DeliveryOrders_Filter;
            DeliveryPayments.Filter += DeliveryPayments_Filter;
        }

        private void DeliveryPayments_Filter(object sender, FilterEventArgs e)
        {
            if (SelectedDeliveryman == null)
            {
                e.Accepted = false;
                return;
            }

            if (e.Item is Payment payment && payment.DeliveryManId == SelectedDeliveryman.Id)
            {
                e.Accepted = true;
                return;
            }
            e.Accepted = false;
        }
    }
}
