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
    [NavigationItem("Delivery Checkout", typeof(DeliveryCheckoutViewModel), isQuickNavigationEnabled: true)]
    [PreAuthorize("Create_Payment_Deliveryman,Read_Payment_Deliveryman,Update_Payment_Deliveryman")]
    public class DeliveryCheckoutViewModel : LazyScreen
    {
        private ObservableCollection<Deliveryman> _deliverymanCollection;


        public DeliveryCheckoutViewModel() : base()
        {

        }


        private decimal CalculateDiscount()
        {
            if (!IsDiscountEnabled) return 0;
            if (string.IsNullOrEmpty(NumericZone)) return 0;

            if (SelectedDeliveryman == null) return 0;

            if (!decimal.TryParse(NumericZone.Replace("%", ""), out var value)) return 0;

            if (NumericZone.Contains("%"))
            {

                return SelectedDeliveryman.Balance * (100 - value) / 100;
            }

            return SelectedDeliveryman.Balance - value;


        }



        public CollectionViewSource DeliverymanCollection { get; set; }

        private IOrderRepository _orderRepo;
        private IPaymentRepository _paymentRepo;
        private Deliveryman _selectedDeliveryman;

        public Deliveryman SelectedDeliveryman
        {
            get { return _selectedDeliveryman; }
            set
            {
                Set(ref _selectedDeliveryman, value);
                NotifyOfPropertyChange(nameof(SelectedDeliveryman));
                UpdateHistory();

                NumericZone = string.Empty;
            }
        }


        public Paginator<Order> UnpaidDeliveryOrders { get; set; }
        public Paginator<Order> PaidDeliveryOrders { get; set; }
        public Paginator<Payment> DeliveryPayments { get; set; }

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
        private RestApi api   = new RestApi();

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
            if (SelectedDeliveryman != null)
            {

                if (SelectedDeliveryman.Balance== 0)
                {
                    var url = api.Resource<Deliveryman>("getwithbalance", SelectedDeliveryman.Id);

                    var result2 = GenericRest.GetThing<Deliveryman>(url);
                    if (result2.status == 200)
                    {
                        SelectedDeliveryman.Balance = result2.Item2.Balance;

                    }
                }
                
                
                if (SelectedTab == CreditViewTabs.UNPAID_ORDERS_TAB)
                {
                    UnpaidDeliveryOrders.Reload();
                }
                
                if (SelectedTab == CreditViewTabs.PAID_ORDERS_TAB)
                {
                    PaidDeliveryOrders.Reload();
                }

                if (SelectedTab == CreditViewTabs.PAYMENT_HISTORY_TAB)
                {
                    DeliveryPayments.Reload();
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
            NumericZone = $"{SelectedDeliveryman?.Balance}";
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

                    if (SelectedDeliveryman != null)
                    {
                        PayementAction();
                    }
                    else
                    {
                        ToastNotification.Notify("قم باختيار مندوب التوصيل من أجل الدفع");
                    }
                    //RelaodDeliveryMan();

                    break;

                    //EditPayment();
                    //RelaodDeliveryMan();


            }
        }

        public void NumericKeyboard(string number)
        {
            if (string.IsNullOrEmpty(number))
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

            decimal? retunedAmount = null;
            if (!decimal.TryParse(NumericZone, out var payedAmount)) return;


            if (payedAmount <= 0 ||SelectedDeliveryman?.Balance==0)
            {
                NumericZone = "";
                return;
            }

            if(payedAmount> SelectedDeliveryman?.Balance)
            {
                retunedAmount = SelectedDeliveryman.Balance - payedAmount;
                payedAmount = SelectedDeliveryman.Balance;
            }

            var payment = new Payment() { Amount = payedAmount, Date = DateTime.Now, DeliveryManId = SelectedDeliveryman.Id.Value,PaymentSource =PaymentSource.Delivery };
            if (IsDiscountEnabled)
            {
                payment.DiscountAmount = Discount;
            }
            var result = GenericRest.PostThing<Payment>(api.Resource<Payment>(EndPoint.SAVE), payment);

            if (result.status == 201)
            {

                NumericZone = retunedAmount?.ToString() ?? "";
                
                IsDiscountEnabled = false;
                var savedPayment = result.Item2;


                var url = api.Resource<Deliveryman>("getwithbalance", SelectedDeliveryman.Id);

                var result2 = GenericRest.GetThing<Deliveryman>(url);
                if (result2.status == 200)
                {
                    SelectedDeliveryman.Balance = result2.Item2.Balance;
                    UnpaidDeliveryOrders.Reload();
                }

                NotifyOfPropertyChange(nameof(Discount));
            }
        }

     

        protected override void Setup()
        {
            _orderRepo = StateManager.GetService<Order, IOrderRepository>();
            _paymentRepo = StateManager.GetService<Payment, IPaymentRepository>();

            _data = new NotifyAllTasksCompletion(StateManager.GetAsync<Deliveryman>());
        }


        public override void Initialize()
        {

            var deliverymen = StateManager.GetAll<Deliveryman>();
            _deliverymanCollection = new ObservableCollection<Deliveryman>(deliverymen);
            DeliverymanCollection = new CollectionViewSource() { Source = _deliverymanCollection };

            var unpaidOrdersPageRetreiver = new PageRetriever<Order>(RetriveUnpaidOrdersPage);
            UnpaidDeliveryOrders = new Paginator<Order>(unpaidOrdersPageRetreiver, canGoNext: CanGoToNextPage);

            var paidOrdersPageRetreiver = new PageRetriever<Order>(RetrivePaidOrdersPage);
            PaidDeliveryOrders = new Paginator<Order>(paidOrdersPageRetreiver, canGoNext:CanGoToNextPage);

            var paymentPageRetreiver = new PageRetriever<Payment>(RetrivePaymentPage);
            DeliveryPayments = new Paginator<Payment>(paymentPageRetreiver, canGoNext: CanGoToNextPage);


        }

        private Page<Order> RetriveUnpaidOrdersPage(int pageIndex, int pageSize)
        {
            if (SelectedDeliveryman?.Id == null)
            {
                ToastNotification.Notify("Select Deliveryman First");
                return new Page<Order>();
            }


            var orderFilter = new OrderFilter()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                DeliverymanIds = { SelectedDeliveryman.Id.Value },
                OrderBy = "orderTime",
                States = { OrderState.Delivered,OrderState.DeliveredPartiallyPaid }
            };
            var result = _orderRepo.GetByCriterias(orderFilter);
            return result;
        }

        private bool CanGoToNextPage() => SelectedDeliveryman != null;

        public Page<Order> RetrivePaidOrdersPage(int pageIndex, int pageSize)
        {
            if (SelectedDeliveryman?.Id == null)
            {
                ToastNotification.Notify("Select Deliveryman First");
                return new Page<Order>();
            }


            var orderFilter = new OrderFilter()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                DeliverymanIds = { SelectedDeliveryman.Id.Value },
                OrderBy = "orderTime",
                States = { OrderState.DeliveredPaid }
            };
            var result = _orderRepo.GetByCriterias(orderFilter);
            return result;
        }

        public Page<Payment> RetrivePaymentPage(int pageIndex, int pageSize )
        {
            if (SelectedDeliveryman == null)
            {
                ToastNotification.Notify("Select Deliveryman First");
                return new Page<Payment>();
            }

            var orderFilter = new PaymentFilter()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                DeliverymanId = SelectedDeliveryman?.Id,
                OrderBy = "date",
                SortOrder = SortOrder.Desc
            };
            var result = _paymentRepo.GetByCriterias(orderFilter);
            return result;
        }

        private void PaidDeliveryOrders_Filter(object sender, FilterEventArgs e)
        {
            if (SelectedDeliveryman == null)
            {
                e.Accepted = false;
                return;
            }

            if (e.Item is Order order && order.DeliverymanId == SelectedDeliveryman.Id && order.State == OrderState.DeliveredPaid)
            {
                e.Accepted = true;
                return;
            }
            e.Accepted = false;
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

    public static class CreditViewTabs
    {
        public static readonly int UNPAID_ORDERS_TAB = 0;
        public static readonly int PAID_ORDERS_TAB = 1;
        public static readonly int PAYMENT_HISTORY_TAB = 2;
    }
}
