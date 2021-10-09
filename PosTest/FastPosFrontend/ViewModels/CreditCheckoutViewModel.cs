using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using static FastPosFrontend.ViewModels.DeliveryAccounting.DeliveryAccountingViewModel;

namespace FastPosFrontend.ViewModels
{
    [NavigationItem("Credit Checkout", typeof(CreditCheckoutViewModel), isQuickNavigationEnabled: true)]
    public class CreditCheckoutViewModel : LazyScreen
    {
        private ObservableCollection<Customer> _customerCollection;
        private ObservableCollection<Order> _ordersCollection;

        public CreditCheckoutViewModel() : base()
        {

        }

        private void CreditOrders_Filter(object sender, FilterEventArgs e)
        {
            if (SelectedCustomer == null)
            {
                e.Accepted = false;
                return;
            }

            if (e.Item is Order order && order.CustomerId == SelectedCustomer.Id && (order.State == OrderState.Credit || order.State == OrderState.CreditPartiallyRePaid))
            {
                e.Accepted = true;
                return;
            }
            e.Accepted = false;
        }

        private decimal CalculateDiscount()
        {
            if (!IsDiscountEnabled) return 0;
            if (string.IsNullOrEmpty(NumericZone)) return 0;

            if (SelectedCustomer == null) return 0;

            if (!decimal.TryParse(NumericZone.Replace("%", ""), out var value)) return 0;

            if (NumericZone.Contains("%"))
            {

                return SelectedCustomer.Balance * (100 - value) / 100;
            }

            return SelectedCustomer.Balance - value;


        }



        public CollectionViewSource CustomerCollection { get; set; }

        private IOrderRepository _orderRepo;
        private IPaymentRepository _paymentRepo;
        private Customer _selectedCustomer;

        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                Set(ref _selectedCustomer, value);
                NotifyOfPropertyChange(nameof(SelectedCustomer));
                UpdateHistory();
                UnpaidOrders.View.Refresh();
            }
        }

        public CollectionViewSource UnpaidOrders { get; set; }
        public Paginator<Order> PaidOrders { get; set; }
        public Paginator<Payment> Payments { get; set; }

        private Order _selectedOrder;

        public Order SelectedOrder
        {
            get { return _selectedOrder; }
            set { Set(ref _selectedOrder, value); }
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

        private string _numericZone;
        private int _selectedTab;
        private bool isDiscountEnabled;

        public string NumericZone
        {
            get { return _numericZone; }
            set { Set(ref _numericZone, value); NotifyOfPropertyChange(nameof(Discount)); }
        }

        public int SelectedTab
        {
            get => _selectedTab;
            set
            {
                Set(ref _selectedTab, value);
                UpdateHistory();
            }
        }

        private void UpdateHistory()
        {
            if (SelectedCustomer != null)
            {
                if (SelectedTab == CreditViewTabs.PAID_ORDERS_TAB)
                {
                    PaidOrders.Reload();
                }

                if (SelectedTab == CreditViewTabs.PAYMENT_HISTORY_TAB)
                {
                    Payments.Reload();
                }
            }
        }

        public decimal Discount => CalculateDiscount();

        public bool IsDiscountEnabled
        {
            get => isDiscountEnabled; 
            set
            {
                Set(ref isDiscountEnabled , value);
                NotifyOfPropertyChange(nameof(Discount));
            }
        }

       

        public void CopyTotalPaymentField()
        {
            NumericZone = $"{SelectedCustomer?.Balance}";
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

                    if (SelectedCustomer != null)
                    {
                        PayementAction();
                    }
                    else
                    {
                        ToastNotification.Notify("قم باختيار زبون من أجل الدفع");
                    }
    

                    break;




            }
        }

        public void NumericKeyboard(string number)
        {
            if (String.IsNullOrEmpty(number))
                return;
            if (number.Length > 1)
                return;
            var numbers = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", "%" };
            if (!numbers.Contains(number))
                return;
            if (NumericZone == null)
                NumericZone = string.Empty;


            if (number.Equals("."))
            {
                NumericZone = NumericZone.Contains(".") ? NumericZone : NumericZone + ".";
                return;
            }

            if (number.Equals("%"))
            {
                NumericZone = NumericZone.Contains("%") ? NumericZone : NumericZone + "%";
                return;
            }

            if (NumericZone.Contains("%"))
            {
                var percentStr = NumericZone.Remove(NumericZone.Length - 1, 1) + number;
                var percent = Convert.ToDecimal(percentStr);
                if (percent < 0 || percent > 100)
                {
                    ToastNotification.Notify("Invalid value for Percentagte", NotificationType.Warning);
                }
                else
                {
                    NumericZone = NumericZone.Remove(NumericZone.Length - 1, 1) + number + "%";
                }

                return;
            }

            NumericZone += number;
        }

        private void PayementAction()
        {


            if (!decimal.TryParse(NumericZone, out var payedAmount)) return;


            if (payedAmount <= 0)
            {
                NumericZone = "";
                return;
            }

            var api = new RestApis();
            var payment = new Payment() { Amount = payedAmount, Date = DateTime.Now, CustomerId = SelectedCustomer.Id.Value ,PaymentSource =PaymentSource.Customer };
            if (IsDiscountEnabled)
            {
                payment.DiscountAmount = Discount;
            }
            var result = GenericRest.PostThing<Payment>(api.Resource<Payment>(EndPoint.SAVE), payment);

            if (result.status == 201)
            {
                NumericZone = "";
                IsDiscountEnabled = false;
                var savedPayment = result.Item2;

                UpdateOrdersFromPayment(savedPayment);
                var url = api.Resource<Customer>("getwithbalance", SelectedCustomer.Id);

                var result2 = GenericRest.GetThing<Customer>(url);
                if (result2.status == 200)
                {
                    SelectedCustomer.Balance = result2.Item2.Balance;
                }
                UnpaidOrders?.View?.Refresh();
                NotifyOfPropertyChange(nameof(Discount));
            }


        }

        private void UpdateOrdersFromPayment(Payment savedPayment)
        {
            foreach (var paymentOrder in savedPayment.Orders)
            {
                var order = _ordersCollection.FirstOrDefault(o => o.Id == paymentOrder.Id);
                if (order != null)
                {
                    order.State = paymentOrder.State;
                    order.GivenAmount = paymentOrder.GivenAmount;
                }
            }
        }

        protected override void Setup()
        {
            var api = new RestApis();
            var url = api.Resource<Customer>("getallwithbalance");
            var result = GenericRest.GetThing<List<Customer>>(url);
            List<Customer> data = new List<Customer>();
            if (result.status == 200)
            {
                data.AddRange(result.Item2);
            }

            _customerCollection = new ObservableCollection<Customer>(data);
            CustomerCollection = new CollectionViewSource() { Source = _customerCollection };

            _orderRepo = StateManager.GetService<Order, IOrderRepository>();
            _paymentRepo = StateManager.GetService<Payment, IPaymentRepository>();

            OrderState[] states = { OrderState.Credit, OrderState.CreditPartiallyRePaid };

            var customerIds = data.Select(d => d.Id.Value);

            var criterias = new OrderFilter() { States = states, CustomerIds = customerIds };
            var orders = _orderRepo.GetByCriteriasAsync(criterias);


            _data = new NotifyAllTasksCompletion(orders);
        }

        public override void Initialize()
        {
            var orders = _data.GetResult<List<Order>>();


            _ordersCollection = new ObservableCollection<Order>(orders);


            UnpaidOrders = new CollectionViewSource() { Source = _ordersCollection };

            var orderPageRetreiver = new PageRetriever<Order>(RetriveOrderPage);
            PaidOrders = new Paginator<Order>(orderPageRetreiver);

            var paymentPageRetreiver = new PageRetriever<Payment>(RetrivePaymentPage);
            Payments = new Paginator<Payment>(paymentPageRetreiver);

            UnpaidOrders.Filter += CreditOrders_Filter;
        }

        public IEnumerable<Order> RetriveOrderPage((int pageIndex, int pageSize) p)
        {
            var orderFilter = new OrderFilter()
            {
                PageIndex = p.pageIndex,
                PageSize = p.pageSize,
                CustomerId = SelectedCustomer.Id,
                DescendingOrder = true,
                OrderBy = "orderTime",
                State = OrderState.CreditRePaid
            };
            var result = _orderRepo.GetByCriterias(orderFilter);
            return result;
        }

        public IEnumerable<Payment> RetrivePaymentPage((int pageIndex, int pageSize) p)
        {
            var orderFilter = new PaymentFilter()
            {
                PageIndex = p.pageIndex,
                PageSize = p.pageSize,
                CustomerId = SelectedCustomer.Id,
                OrderBy = "date",
                DescendingOrder = true
            };
            var result = _paymentRepo.GetByCriterias(orderFilter);
            return result;
        }

        private void PaidCreditOrders_Filter(object sender, FilterEventArgs e)
        {
            if (SelectedCustomer == null)
            {
                e.Accepted = false;
                return;
            }

            if (e.Item is Order order && order.CustomerId == SelectedCustomer.Id && order.State == OrderState.CreditRePaid)
            {
                e.Accepted = true;
                return;
            }
            e.Accepted = false;
        }

        private void CreditRePayments_Filter(object sender, FilterEventArgs e)
        {
            if (SelectedCustomer == null)
            {
                e.Accepted = false;
                return;
            }

            if (e.Item is Payment payment && payment.CustomerId == SelectedCustomer.Id)
            {
                e.Accepted = true;
                return;
            }
            e.Accepted = false;
        }
    }

   
}
