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
using System.Threading.Tasks;
using System.Windows.Data;
using static FastPosFrontend.ViewModels.DeliveryAccounting.DeliveryAccountingViewModel;

namespace FastPosFrontend.ViewModels
{
    [NavigationItem("Credit Checkout", typeof(CreditCheckoutViewModel), isQuickNavigationEnabled: true)]
    [PreAuthorize("Create_Payment_Client,Read_Payment_Client,Update_Payment_Client")]
    public class CreditCheckoutViewModel : LazyScreen
    {
        private ObservableCollection<Customer> _customerCollection;


        public CreditCheckoutViewModel() : base()
        {
            SetupEmbeddedRightCommandBar();
        }

        private void SetupEmbeddedRightCommandBar()
        {
            EmbeddedRightCommandBar = EmbeddedCommandBarViewModel.Right(this);
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
        private IRepository<Customer, long> _customerRepo;
        private Customer _selectedCustomer;

        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                Set(ref _selectedCustomer, value);
                NotifyOfPropertyChange(nameof(SelectedCustomer));
                UpdateHistory();

                NumericZone = string.Empty;
            }
        }

        public string SearchQuery
        {
            get => _searchQuery; set
            {
                Set(ref _searchQuery, value);
                CustomerCollection.View.Refresh();
            }
        }

        public Paginator<Order> UnpaidOrders { get; set; }
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
        private string _searchQuery = string.Empty;
        private readonly RestApi api = new RestApi();

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


                if (SelectedTab == CreditViewTabs.UNPAID_ORDERS_TAB)
                {
                    UnpaidOrders.Reload();
                }

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
                Set(ref isDiscountEnabled, value);
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
            decimal? retunedAmount = null;

            if (!decimal.TryParse(NumericZone, out var payedAmount)) return;


            if (payedAmount <= 0 || SelectedCustomer?.Balance == 0)
            {
                NumericZone = "";
                return;
            }

            if (payedAmount > SelectedCustomer?.Balance)
            {
                retunedAmount = SelectedCustomer.Balance - payedAmount;
                payedAmount = SelectedCustomer.Balance;
            }

            var api = new RestApi();
            var payment = new Payment() { Amount = payedAmount, Date = DateTime.Now, CustomerId = SelectedCustomer.Id.Value, PaymentSource = PaymentSource.Customer };
            if (IsDiscountEnabled)
            {
                payment.DiscountAmount = Discount;
            }
            var result = GenericRest.PostThing<Payment>(api.Resource<Payment>(EndPoint.SAVE), payment);

            if (result.status == 201)
            {
                if (retunedAmount.HasValue)
                {
                    NumericZone = retunedAmount + "";
                }
                else
                {
                    NumericZone = "";
                }
                IsDiscountEnabled = false;
                var savedPayment = result.Item2;


                var url = api.Resource<Customer>("getwithbalance", SelectedCustomer.Id);

                var result2 = GenericRest.GetThing<Customer>(url);
                if (result2.status == 200)
                {
                    SelectedCustomer.Balance = result2.Item2.Balance;
                }

                UnpaidOrders.Reload();

                NotifyOfPropertyChange(nameof(Discount));
            }


        }

        public async Task<List<Customer>> GetCustomersWithBalance()
        {
            var result = await _customerRepo.GetAllAsync("getallwithbalance");
            if (result.status != 200) return new List<Customer>();
            return result.Item2.ToList();
        }


        protected override void Setup()
        {
            _orderRepo = StateManager.GetService<Order, IOrderRepository>();
            _paymentRepo = StateManager.GetService<Payment, IPaymentRepository>();
            _customerRepo = StateManager.Instance.GetRepository<Customer>();

            _data = new NotifyAllTasksCompletion(GetCustomersWithBalance());
        }


        public override void Initialize()
        {

            //var customers = StateManager.GetAll<Customer>();
            var customers = _data.GetResult<List<Customer>>();
            _customerCollection = new ObservableCollection<Customer>(customers);
            CustomerCollection = new CollectionViewSource() { Source = _customerCollection };
            CustomerCollection.Filter += CustomerCollection_Filter;
            var unpaidOrdersPageRetreiver = new PageRetriever<Order>(RetriveUnpaidOrdersPage);
            UnpaidOrders = new Paginator<Order>(unpaidOrdersPageRetreiver, canGoNext: CanGoToNextPage);

            var paidOrdersPageRetreiver = new PageRetriever<Order>(RetrivePaidOrdersPage);
            PaidOrders = new Paginator<Order>(paidOrdersPageRetreiver, canGoNext: CanGoToNextPage);

            var paymentPageRetreiver = new PageRetriever<Payment>(RetrivePaymentPage);
            Payments = new Paginator<Payment>(paymentPageRetreiver, canGoNext: CanGoToNextPage);


        }

        private void CustomerCollection_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is Customer customer)
            {
                var (name, mobile) = CustomerViewModel.GetNameAndMobile(SearchQuery);
                e.Accepted = customer.Name.Contains(name) && (customer.PhoneNumbers?.Any(m => m.Contains(mobile))??false);
            }
        }

       

        private bool CanGoToNextPage() => SelectedCustomer != null;

        private Page<Order> RetriveUnpaidOrdersPage(int pageIndex, int pageSize)
        {
            if (SelectedCustomer?.Id == null)
            {
                ToastNotification.Notify("Select a Customer First");
                return new Page<Order>();
            }


            var orderFilter = new OrderFilter()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                CustomerIds = { SelectedCustomer.Id.Value },
                OrderBy = "orderTime",
                States = { OrderState.Credit, OrderState.CreditPartiallyRePaid }
            };
            var result = _orderRepo.GetByCriterias(orderFilter);
            return result;
        }

        public Page<Order> RetrivePaidOrdersPage(int pageIndex, int pageSize)
        {
            if (SelectedCustomer == null)
            {
                ToastNotification.Notify("Select Customer First");
                return new Page<Order>();
            }


            var orderFilter = new OrderFilter()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                CustomerIds = { SelectedCustomer.Id.Value },
                OrderBy = "orderTime",
                States = { OrderState.CreditRePaid }
            };
            var result = _orderRepo.GetByCriterias(orderFilter);
            return result;
        }

        public Page<Payment> RetrivePaymentPage(int pageIndex, int pageSize)
        {
            if (SelectedCustomer == null)
            {
                ToastNotification.Notify("Select Customer First");
                return new Page<Payment>();
            }

            var orderFilter = new PaymentFilter()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                CustomerIds = { SelectedCustomer.Id.Value },
                OrderBy = "date",
                SortOrder = SortOrder.Desc
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

        public void NavigateToCheckout()
        {
            var nav = (Parent as MainViewModel)?.Navigator;
            var navItem = nav?.QuickNavigationItems.FirstOrDefault(i => i.Target == typeof(CheckoutViewModel));
            if (nav != null)
            {
                nav.SelectedNavigationItem = navItem;
            }

        }
    }


}
