using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Printing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using Caliburn.Micro;
using FastPosFrontend.Enums;
using FastPosFrontend.Events;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using FastPosFrontend.sse;
using FastPosFrontend.ViewModels.Settings;
using FastPosFrontend.ViewModels.Settings.Customer;
using FastPosFrontend.ViewModels.SubViewModel;
using LaunchDarkly.EventSource;
using Newtonsoft.Json;
using ServiceInterface.Authorisation;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;
using Utilities.Mutation.Observers;
using Icon = FastPosFrontend.Helpers.Icon;
using Table = ServiceInterface.Model.Table;

namespace FastPosFrontend.ViewModels
{
    [NavigationItem(
        title: Constants.Navigation.Checkout, 
        target: typeof(CheckoutViewModel),"",
        keepAlive: true, isDefault: true)]
    public class CheckoutViewModel : LazyScreen, IHandle<AssignOrderTypeEventArgs>,ISettingsListener
    {
        #region Private fields

        private static readonly bool IsRunningFromXUnit =
            AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.StartsWith("XUnitTesting"));

        private static string scanValue;
        private BindableCollection<Additive> _additvesPage;
        private BindableCollection<Table> _tables;
        private BindableCollection<Product> _productsPage;
        private WarningViewModel _warningViewModel;
        private TablesViewModel _tablesViewModel;
        private WaitingViewModel _waitingViewModel;
        private DeliveryViewModel _deliveryViewModel;
        private TakeawayViewModel _takeAwayViewModel;
        private CustomerViewModel _customerViewModel;
        private Order _currentOrder;
        private int orderCount = 1;
        private Table _selectedTable;
        private Deliveryman _selectedDeliveryman;
        private Waiter _selectedWaiter;
        private ListKind _listKind;
        private string _numericZone;
        private bool _productsVisibility;
        private bool _canExecuteNext;
        private bool _canExecutePrevious;
        private bool _additivesVisibility;
        private bool _IsDialogOpen;
        private bool _IsTopDrawerOpen;
        private INotifyPropertyChanged _dialogViewModel;
        private decimal givenAmount;
        private decimal? _returnedAmount;
        private int _pageNumber;
        private int itemsPerCategoryPage;
        private int _categoryPageCount;
        private int _currentCategoryPageIndex;
        private Order _printOrder;
        private Category _currentCategory;
        private bool _isInWaitingViewActive = true;
        private bool _isTableViewActive;
        private bool _isTakeawayViewActive;
        private bool _isDeliveryViewActive;
        private bool _isReady;
        private ProductLayoutConfiguration _productLayout;
        private bool _isOrderInfoShown;
        private readonly int mainthread;
        private readonly DispatcherTimer _orderInfoCloseTimer;
        private string _lastModifiedOrderPropertyName;
        private Table _selectedOrderTable;
        private SplitViewModel _splitViewModel;

        #endregion

        #region Constructors

        void CalculateOrderElapsedTime()
        {
            while (true)
            {
                if (Orders != null)
                {
                    try
                    {
                        foreach (var o in Orders)
                        {
                            var lastStateTime = o.OrderStates?.LastOrDefault<OrderStateElement>();

                            o.ElapsedTime = lastStateTime != null
                                ? DateTime.Now - lastStateTime.StateTime
                                : DateTime.Now - o.OrderTime;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                Thread.Sleep(30000);
            }
        }

        public CheckoutViewModel(
        ) : base()
        {

            mainthread = Thread.CurrentThread.ManagedThreadId;
            _orderInfoCloseTimer = new DispatcherTimer(){Interval = new TimeSpan(0, 0, 3)};
            _orderInfoCloseTimer.Tick += (sender, args) =>
            {
                IsOrderInfoShown = false;
                _orderInfoCloseTimer.Stop();
            };
            if (AppConfigurationManager.ContainsKey("OrderCountModifiedDate"))
            {
                var dateString = AppConfigurationManager.Configuration<string>("OrderCountModifiedDate");
                orderCount = DateTime.Today.ToString("yyyy-MM-dd")
                    .Equals(dateString)
                    ? Convert.ToInt32(AppConfigurationManager.Configuration("OrderCount"))
                    : 1;
            }

            SetupEmbeddedCommandBar();
            SetupEmbeddedStatusBar();

            _productLayout = AppConfigurationManager.Configuration<ProductLayoutConfiguration>() ?? new ProductLayoutConfiguration() { Columns = 6, Rows = 5 };

            var pageSize = _productLayout.NumberOfProducts;

            
            MaxProductPageSize = pageSize;
            CurrentCategoryPageIndex = 0;
            itemsPerCategoryPage = 5;

            var configuration = AppConfigurationManager.Configuration<GeneralSettings>();


            if (configuration != null)
            {
                itemsPerCategoryPage = configuration.NumberOfCategories;
            }
            
            Setup();
            OnReady();
        }

        private void _eventSource_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Message);
            Order updatedOrder = null;
            Order order = null;
            if (e.EventName == SSEventType.CREATE_ORDER|| e.EventName == SSEventType.UPDATE_ORDER)
            {
                updatedOrder = JsonConvert.DeserializeObject<Order>(e.Message.Data);
                order = Orders.FirstOrDefault(o => o.Id == updatedOrder.Id);
               
            }


            Application.Current.Dispatcher.Invoke(() =>
            {

                if (e.EventName == SSEventType.CREATE_ORDER)
                {
                    Orders.Add(updatedOrder);
                    WaitingViewModel?.Orders.Refresh();
                    WaitingViewModel.NotifyOfPropertyChange(() => WaitingViewModel.OrderCount);
                    return;

                }

                if (e.EventName == SSEventType.UPDATE_ORDER)
                {

                    order?.UpdateOrderFrom(updatedOrder);

                    return;
                }

                if (e.EventName == SSEventType.PAY_ORDER)
                {
                    var id = long.Parse(e.Message.Data);
                    var orderToRemove = Orders.FirstOrDefault(o => o.Id == id);
                    Orders.Remove(orderToRemove);
                    if (CurrentOrder == orderToRemove)
                    {
                        CurrentOrder = null;
                    }
                    WaitingViewModel.NotifyOfPropertyChange(() => WaitingViewModel.OrderCount);
                    return;
                }

                if (e.EventName == SSEventType.CANCEL_ORDER)
                {
                    var id = long.Parse(e.Message.Data);
                    var orderToRemove = Orders.FirstOrDefault(o => o.Id == id);
                    Orders.Remove(orderToRemove);

                    if (CurrentOrder == orderToRemove)
                    {
                        CurrentOrder = null;
                    }
                    WaitingViewModel.NotifyOfPropertyChange(() => WaitingViewModel.OrderCount);
                    return;
                }

            });

            
        }

        protected override void Setup()
        {
            var categories = StateManager.GetAsync<Category>();
            var unprocessedOrders = StateManager.GetAsync<Order>(predicate: "unprocessed");

            _data = new NotifyAllTasksCompletion(categories,
                unprocessedOrders);
        }

        private void SetupEmbeddedCommandBar()
        {
            EmbeddedCommandBar = new EmbeddedCommandBarViewModel(this,"CheckoutLeftCommandBar");
            
        }

        private void SetupEmbeddedStatusBar()
        {
            EmbeddedRightCommandBar = new EmbeddedCommandBarViewModel(this, "CheckoutStatusBar");
            
        }

        public async override void Initialize()
        {

          

            var deliveryMen = StateManager.Get<Deliveryman>();
            var waiter = StateManager.Get<Waiter>();
            var tables = StateManager.Get<Table>();
            var customers = StateManager.Get<Customer>();
            var unprocessedOrders = StateManager.Get<Order>();
            var categories = StateManager.Get<Category>();
            var products = StateManager.Get<Product>();



            Orders = new BindableCollection<Order>(unprocessedOrders);
            OrdersCollectionObserver = new CollectionMutationObserver<Order>(Orders,true,true);

            ProductsPage = new BindableCollection<Product>();
            AdditivesPage = new BindableCollection<Additive>();
            Waiters = new BindableCollection<Waiter>(waiter);
            Delivereymen = new BindableCollection<Deliveryman>(deliveryMen);
            Tables = new BindableCollection<Table>(tables);
            Customers = new BindableCollection<Customer>(customers);
            OrderItemsCollectionViewSource = new CollectionViewSource();


            OrderItemsCollectionViewSource.Filter += OrderItemsCollectionViewSourceOnFilter;

            Task.Run(CalculateOrderElapsedTime);
            if (IsRunningFromXUnit)
            {
                CurrentOrder = new Order();
                Orders.Add(CurrentOrder);
            }


            AllProducts = products.ToList();
            AllCategories = categories.ToList();


            LoadCategoryPages();


            PaginatedCategories = new CollectionViewSource {Source = Categories};

            PaginatedCategories.Filter += PaginatedCategoriesOnFilter;

            CalculateTotalPages(Categories.Count);


            foreach (var table in Tables)
            {
                table.AllOrders = Orders;
                table.AllTables = Tables;
            }

            ProductsVisibility = true;
            AdditivesVisibility = false;

            TakeAwayViewModel = new TakeawayViewModel(this);
            DeliveryViewModel = new DeliveryViewModel(this);
            WaitingViewModel = new WaitingViewModel(this);
            CustomerViewModel = new CustomerViewModel(this);
            TablesViewModel = new TablesViewModel(this);
            CurrentCategory = Categories[0];
            ShowCategoryProducts(CurrentCategory);

            AppDrawerConductor.Instance.InitTop(this, "CheckoutWaiterDrawer", this, tag: ListKind.Waiter);
            AppDrawerConductor.Instance.InitTop(this, "CheckoutDeliverymanDrawer", this, tag: ListKind.Delivery);
            AppDrawerConductor.Instance.InitTop(this, "CheckoutTableDrawer", this, tag: ListKind.Table);
            AppDrawerConductor.Instance.InitTop(this, "CheckoutCustomerDrawer", this, tag: ListKind.Customer);

            
            var config = Configuration.Builder(new Uri("http://localhost:8080/events/subscribe")).ResponseStartTimeout(TimeSpan.FromSeconds(60 * 60 * 24)).Method(HttpMethod.Get).RequestHeader("Authorization", AuthProvider.Instance?.AuthorizationToken).Build();
            _eventSource = new EventSource(config);

            _eventSource.MessageReceived += _eventSource_MessageReceived;
            _eventSource.Error += (s, args) =>
            {
                Console.WriteLine(args.Exception);
            };

            await _eventSource.StartAsync();

        }

        public CollectionMutationObserver<Order> OrdersCollectionObserver { get; set; }

        private void ShowOrderInfo()
        {
            IsOrderInfoShown = true;
            _orderInfoCloseTimer.Start(); 
        }
        private void CurrentOrder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
      
            if (e.PropertyName == nameof(Order.Table)&& CurrentOrder?.Table == null)
            {
                SelectedTable = null;
            }
            if (e.PropertyName == nameof(Order.Customer)||e.PropertyName == nameof(Order.Type)
            ||e.PropertyName == nameof(Order.Waiter)||e.PropertyName == nameof(Order.Table)|| e.PropertyName == nameof(Order.Deliveryman))
            {
                ShowOrderInfo();
                LastModifiedOrderPropertyName = e.PropertyName;
            }
            if (e.PropertyName == nameof(Order.SelectedOrderItem))
            {
                if (CurrentOrder?.SelectedOrderItem != null)
                {
                    if (!CurrentOrder.SelectedOrderItem.CanAddAdditives)
                    {
                        AdditivesVisibility = false;
                        ProductsVisibility = true;
                    }

                    ShowProductAdditives(CurrentOrder?.SelectedOrderItem?.Product);
                }
            }
        }

        EventSource _eventSource;

        public ProductLayoutConfiguration ProductLayout
        {
            get => _productLayout;
            set => Set(ref _productLayout, value);
        }

        private void OrderItemsCollectionViewSourceOnFilter(object sender, FilterEventArgs e)
        {
            var orderItem = e.Item as OrderItem;
            if (orderItem.State == OrderItemState.Removed)
            {
                e.Accepted = false;
            }
            else
            {
                e.Accepted = true;
            }
        }

        OrderNotificationListener orderNotificationListener;
        
        public List<Category> AllCategories { get; set; }

        #endregion

        #region Properties

        public bool IsDialogOpen
        {
            get => _IsDialogOpen;
            set => Set(ref _IsDialogOpen, value);
        }

        public bool IsInWaitingViewActive
        {
            get => _isInWaitingViewActive;
            set {
                Set(ref _isInWaitingViewActive, value);
                NotifyOfPropertyChange(nameof(IsInWaitingViewActive));
            }
        }

        public bool IsTableViewActive
        {
            get => _isTableViewActive;
            set => Set(ref _isTableViewActive, value);
        }

        public bool IsTakeawayViewActive
        {
            get => _isTakeawayViewActive;
            set => Set(ref _isTakeawayViewActive, value);
        }

        public bool IsDeliveryViewActive
        {
            get => _isDeliveryViewActive;
            set => Set(ref _isDeliveryViewActive, value);
        }

        public bool CanExecuteMext
        {
            get => _canExecuteNext;
            set
            {
                _canExecuteNext = value;
                NotifyOfPropertyChange(() => CanExecuteMext);
            }
        }

        public bool CanExecutePrevious
        {
            get => _canExecutePrevious;
            set
            {
                _canExecutePrevious = value;
                NotifyOfPropertyChange(() => CanExecutePrevious);
            }
        }

        public string LastModifiedOrderPropertyName
        {
            get => _lastModifiedOrderPropertyName;
            set => Set(ref _lastModifiedOrderPropertyName, value);
        }

        public ICollectionView FilteredProducts { get; set; }
        public ICollection<Product> AllProducts { get; set; }

        public Category CurrentCategory
        {
            get => _currentCategory;
            set => Set(ref _currentCategory, value);
        }

        public int MaxProductPageSize { get; set; }

        public bool AdditivesVisibility
        {
            get => _additivesVisibility;
            set
            {
                _additivesVisibility = value;
                NotifyOfPropertyChange(nameof(AdditivesVisibility));
            }
        }

        public bool ProductsVisibility
        {
            get => _productsVisibility;
            set
            {
                _productsVisibility = value;
                NotifyOfPropertyChange(nameof(ProductsVisibility));
            }
        }

        public BindableCollection<Product> ProductsPage
        {
            get => _productsPage;
            set => Set(ref _productsPage, value);
        }

        public BindableCollection<Additive> AdditivesPage
        {
            get => _additvesPage;
            set => Set(ref _additvesPage, value);
        }


        private void SetSelectedInListedOrdersDisplayedOrder()
        {
            var table = Tables.Where(t => t.Orders.Contains(CurrentOrder)).FirstOrDefault();
            if (table != null)
            {
                table.SelectedOrder = CurrentOrder;
                TablesViewModel.SelectedTable = table;
            }

            foreach (var t in TablesViewModel.Tables.Where(tb => !tb.Orders.Contains(CurrentOrder)))
            {
                t.SelectedOrder = null;
                if (TablesViewModel.SelectedTable == t)
                {
                    TablesViewModel.SelectedTable = null;
                }
            }

            DeliveryViewModel.SelectedOrder = DeliveryViewModel.Orders.Cast<Order>().Where(o => o == CurrentOrder)
                .FirstOrDefault();

            TakeAwayViewModel.SelectedOrder = TakeAwayViewModel.Orders.Cast<Order>().Where(o => o == CurrentOrder)
                .FirstOrDefault();

            WaitingViewModel.SelectedOrder =
                WaitingViewModel.Orders.Cast<Order>().Where(o => o == CurrentOrder).FirstOrDefault();
        }

        public BindableCollection<Category> Categories { get; set; }

        public CollectionViewSource PaginatedCategories { get; set; }
        public BindableCollection<Order> Orders { get; set; }
        public BindableCollection<Customer> Customers { get; set; }

        public Order CurrentOrder
        {
            get { return _currentOrder; }
            set
            {
                if (value == null)
                {
                    SetNullCurrentOrder();
                }
                else
                {
                    _currentOrder = value;
                }

                SetSelectedInListedOrdersDisplayedOrder();
                OrderItemsCollectionViewSource.Source = CurrentOrder?.OrderItems;
                NotifyOfPropertyChange(() => CurrentOrder);
            }
        }


        public CollectionViewSource OrderItemsCollectionViewSource { get; set; }

        public string NumericZone
        {
            get => _numericZone;
            set
            {
                _numericZone = value;
                NotifyOfPropertyChange(() => NumericZone);
            }
        }

        public TablesViewModel TablesViewModel
        {
            get => _tablesViewModel;
            set
            {
                _tablesViewModel = value;
                NotifyOfPropertyChange();
            }
        }

        public WaitingViewModel WaitingViewModel
        {
            get => _waitingViewModel;
            set
            {
                _waitingViewModel = value;
                NotifyOfPropertyChange();
            }
        }

        public CustomerViewModel CustomerViewModel
        {
            get => _customerViewModel;
            set => Set(ref _customerViewModel, value);
        }

        public DeliveryViewModel DeliveryViewModel
        {
            get => _deliveryViewModel;
            set
            {
                _deliveryViewModel = value;
                NotifyOfPropertyChange();
            }
        }

        public TakeawayViewModel TakeAwayViewModel
        {
            get => _takeAwayViewModel;
            set
            {
                _takeAwayViewModel = value;
                NotifyOfPropertyChange();
            }
        }

        public SplitViewModel SplitViewModel
        {
            get => _splitViewModel;
            set => Set(ref _splitViewModel, value);
        }

        public Table SelectedTable
        {
            get => _selectedTable;
            set
            {
                if (value!= CurrentOrder?.Table)
                {
                    TableAction(value);
                }
                Set(ref _selectedTable, value);
            }
        }

        public Table SelectedOrderTable
        {
            get => _selectedOrderTable;
            set => Set(ref _selectedOrderTable, value);
        }

        public BindableCollection<Table> Tables
        {
            get => _tables;
            set
            {
                _tables = value;
                NotifyOfPropertyChange();
            }
        }

        public INotifyPropertyChanged DialogViewModel
        {
            get => _dialogViewModel;
            set
            {
                _dialogViewModel = value;
                NotifyOfPropertyChange();
            }
        }

        public WarningViewModel WarningViewModel
        {
            get => _warningViewModel;
            set
            {
                _warningViewModel = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        public decimal GivenAmount
        {
            get => givenAmount;
            set
            {
                givenAmount = value;
                NotifyOfPropertyChange(() => GivenAmount);
            }
        }

        public bool IsOrderInfoShown
        {
            get => _isOrderInfoShown;
            set => Set(ref _isOrderInfoShown, value);
        }

        public decimal? ReturnedAmount
        {
            get => _returnedAmount;
            set
            {
                _returnedAmount = value;
                NotifyOfPropertyChange(() => ReturnedAmount);
            }
        }


        #region Order Commands

        public bool SaveOrder(ref Order order)
        {
            if (IsRunningFromXUnit)
            {
                return false;
            }

            bool resp;
            try
            {
                resp = StateManager.Save(order);
            }
            catch (AggregateException)
            {
                ToastNotification.Notify("Check your server connection");
                return false;
            }

            return resp;
        }

        private void SaveCurrentOrder()
        {
            var resp = SaveOrder(ref _currentOrder);
            CurrentOrder = _currentOrder;
            NotifyOfPropertyChange(() => CurrentOrder);
            switch (resp)
            {
                case true:
                    if (CurrentOrder.State == OrderState.Payed ||
                        CurrentOrder.State == OrderState.Canceled ||
                        CurrentOrder.State == OrderState.Removed || CurrentOrder.State == OrderState.Delivered)
                    {
                        RemoveCurrentOrderForOrdersList();
                    }
                    else
                    {
                        CurrentOrder = null;
                    }

                    break;

                default:

                    ToastNotification.Notify("Unable to save order");
                    break;
            }

            AdditivesVisibility = false;
            ProductsVisibility = true;
        }

        public void ShowOrder(Order order)
        {
            if (order == null) return;

            CurrentOrder?.SaveScreenState(CurrentCategory, AdditivesPage, ProductsVisibility, AdditivesVisibility);
            if (CurrentOrder == order)
            {
                CurrentOrder = null;
                return;
            }
            else
            {
                CurrentOrder = order;
                CurrentOrder.PropertyChanged -= CurrentOrder_PropertyChanged;
                CurrentOrder.PropertyChanged += CurrentOrder_PropertyChanged;
            }


            if (order.ShownCategory == null && order.Id != null)
            {
                order.ShownCategory = CurrentCategory;
                order.ProductsVisibility = ProductsVisibility;
                order.AdditivesVisibility = AdditivesVisibility;
            }

            SelectedDeliveryman = CurrentOrder.Deliveryman;
            SelectedWaiter = CurrentOrder.Waiter;
            SelectedTable = CurrentOrder.Table;

            CurrentCategory = CurrentOrder.ShownCategory;

            if (CurrentCategory != null && ProductsPage.Any(p => p?.Category != CurrentCategory))
            {
                ShowCategoryProducts(CurrentCategory);
            }

            AdditivesPage = CurrentOrder.ShownAdditivesPage;

            ProductsVisibility = CurrentOrder.ProductsVisibility;
            AdditivesVisibility = CurrentOrder.AdditivesVisibility;
        }

        public void NewOrder()
        {
            AdditivesVisibility = false;
            ProductsVisibility = true;
            CurrentOrder?.SaveScreenState(CurrentCategory, AdditivesPage, ProductsVisibility, AdditivesVisibility);

            CurrentOrder = new Order(Orders) { OrderNumber = orderCount};
            orderCount++;

            OrderItemsCollectionViewSource.Source = CurrentOrder?.OrderItems;
            //OrderItemsCollectionViewSource.View.Refresh();
            CurrentOrder.PropertyChanged += CurrentOrder_PropertyChanged;
            //DisplayedOrder = CurrentOrder;
            Orders.Add(CurrentOrder);

            OrdersCollectionObserver.ObserveItem(CurrentOrder);

            SetCurrentOrderTypeAndRefreshOrdersLists(OrderType.InWaiting);
            GivenAmount = 0;
            ReturnedAmount = null;
            SelectedDeliveryman = null;
            SelectedWaiter = null;
            CustomerViewModel.SelectedCustomer = null;
        }

        //FilterEventHandler TableOrdersFilter;

        private void SetCurrentOrderTypeAndRefreshOrdersLists(OrderType? orderType, Table table = null)
        {
            if (CurrentOrder != null)
            {
                CurrentOrder.Type = orderType;
                if (orderType == OrderType.OnTable)
                {
                    CurrentOrder.Table = table;
                    SelectedDeliveryman = null;
                    IsTableViewActive = true;
                    IsTakeawayViewActive = false;
                    IsDeliveryViewActive = false;
                    IsInWaitingViewActive = false;
                }
                else
                {
                    if (orderType == OrderType.TakeAway)
                    {
                        SelectedDeliveryman = null;
                        IsTableViewActive = false;
                        IsTakeawayViewActive = true;
                        IsDeliveryViewActive = false;
                        IsInWaitingViewActive = false;
                    }

                    if (orderType == OrderType.InWaiting)
                    {
                        IsTableViewActive = false;
                        IsTakeawayViewActive = false;
                        IsDeliveryViewActive = false;
                        IsInWaitingViewActive = true;
                    }

                    if (orderType == OrderType.Delivery)
                    {
                        IsTableViewActive = false;
                        IsTakeawayViewActive = false;
                        IsDeliveryViewActive = true;
                        IsInWaitingViewActive = false;
                    }

                    CurrentOrder.Table = null;
                    SelectedTable = null;
                }
            }


            DeliveryViewModel.OrderViewSource.View.Refresh();
            WaitingViewModel.OrderViewSource.View.Refresh();
            TakeAwayViewModel.OrderViewSource.View.Refresh();
            DeliveryViewModel.NotifyOfPropertyChange(() => DeliveryViewModel.OrderCount);
            WaitingViewModel.NotifyOfPropertyChange(() => WaitingViewModel.OrderCount);
            TakeAwayViewModel.NotifyOfPropertyChange(() => TakeAwayViewModel.OrderCount);

            foreach (var t in Tables)
            {
                t.OrderViewSource.View.Refresh();
            }

            TablesViewModel.TablesViewSource.Filter -= TablesViewModel.TablesFilter;
            TablesViewModel.TablesViewSource.Filter += TablesViewModel.TablesFilter;
            // TablesViewModel.TablesView.Refresh();
            SetSelectedInListedOrdersDisplayedOrder();
            TablesViewModel.NotifyOfPropertyChange(() => TablesViewModel.OrderCount);
        }

        public void NewTotalToNumericZone()
        {
            if (CurrentOrder == null || CurrentOrder.NewTotal == 0)
            {
                return;
            }

            NumericZone = CurrentOrder.NewTotal.ToString();
        }

        public void SetNullCurrentOrder()
        {
            CurrentOrder?.SaveScreenState(CurrentCategory, AdditivesPage, ProductsVisibility, AdditivesVisibility);

            _currentOrder = null;
            //DisplayedOrder = null;
            SelectedDeliveryman = null;
            SelectedWaiter = null;
            CustomerViewModel.SelectedCustomer = null;
            SetSelectedInListedOrdersDisplayedOrder();
        }

        public void RemoveCurrentOrderForOrdersList()
        {
            if (CurrentOrder == null || Orders == null)
            {
                return;
            }

            Orders.Remove(CurrentOrder);
            var result = OrdersCollectionObserver?.UnObserve(CurrentOrder);

            CurrentOrder.PropertyChanged -= CurrentOrder_PropertyChanged;
            CurrentOrder = null;
            SetCurrentOrderTypeAndRefreshOrdersLists(null);
        }

        public void CancelOrder()
        {
            if (CurrentOrder == null)
            {
                ToastNotification.Notify("No order to remover / cancel!",NotificationType.Information);
                return;
            }

            
            if (CurrentOrder?.Id == null)
            {
                Orders.Remove(CurrentOrder);
                WaitingViewModel?.Orders.Refresh();
                WaitingViewModel?.NotifyOfPropertyChange(nameof(WaitingViewModel.OrderCount));
                CurrentOrder = null;
                return;
            }

  
            var response = ModalDialogBox.YesNo("Are you sure you want to Cancel this Order?", "Cancel Order").Show();
            if (response)
            {
                CancelOrderAction(this);
            }
        }


        public void CancelOrderAction(object param)
        {
            var checkoutViewModel = param as CheckoutViewModel;
            if (checkoutViewModel?.CurrentOrder.State == null)
            {
                checkoutViewModel.CurrentOrder.State = OrderState.Removed;
            }
            else
            {
                checkoutViewModel.CurrentOrder.State = OrderState.Canceled;
            }

            checkoutViewModel.SaveCurrentOrder();
            IsDialogOpen = false;
        }

        #endregion

        public bool ChangesMade { get; set; }

        void LoadCategoryPages()
        {
            var comparer = new Comparer<Category>();
            var retrieveCategories = AllCategories;
            var categories = new List<Category>(retrieveCategories.Where(c => c.Rank != null));
            categories.Sort(comparer);
            Categories = new BindableCollection<Category>();
            var maxRank =  categories.Max(c => c.Rank)??0;
            int _categpryPageSize = AppConfigurationManager.Configuration<GeneralSettings>().NumberOfCategories;
            int nbpage = (maxRank / _categpryPageSize) + (maxRank % _categpryPageSize == 0 ? 0 : 1);
            nbpage = nbpage == 0 ? 1 : nbpage;
            CategoryPageCount = nbpage;
            var size = nbpage * _categpryPageSize;

            RankedItemsCollectionHelper.LoadPagesFilled(source: categories, target: Categories, size: size);
        }

        public void ShowCategoryProducts(Category category)
        {
            AdditivesVisibility = false;
            ProductsVisibility = true;
            if (category == CurrentCategory && !(ProductsPage.Count == 0 || ProductsPage == null)) return;

            ProductsPage.Clear();
            ProductsVisibility = true;
            if (category?.Id == null) return;
            var filteredProducts = category.Products;

            var comparer = new Comparer<Product>();
            var listOfFliteredProducts = filteredProducts.ToList();
            listOfFliteredProducts.Sort(comparer);
            CurrentCategory = category;
            RankedItemsCollectionHelper.LoadPagesNotFilled(source: listOfFliteredProducts, target: ProductsPage,
                size: MaxProductPageSize, parameter: category);
        }

        public void ShowProductAdditives(Product product)
        {
            AdditivesPage?.Clear();
            if (product == null || !product.IsPlatter ||
                (product.IsPlatter && (product.IdAdditives == null || product.IdAdditives.Count == 0)))
            {
                AdditivesVisibility = false;
                ProductsVisibility = true;
                return;
            }
            var comparer = new Comparer<Additive>();
            var additives = product.Additives.ToList();
            additives.Sort(comparer);

            RankedItemsCollectionHelper.LoadPagesNotFilled(source: additives, target: AdditivesPage,
                size: MaxProductPageSize);
            AdditivesVisibility = true;
            ProductsVisibility = false;
        }


        #region Command Buttons' Actions

        public void ActionKeyboard(ActionButton cmd)
        {
            if (cmd == ActionButton.CopyToNumericZone)
            {
                if (CurrentOrder != null) NumericZone = CurrentOrder.NewTotal + "";
                return;
            }
            if (string.IsNullOrEmpty(NumericZone) &&
                cmd != ActionButton.Split &&
                cmd != ActionButton.Cmd &&
                cmd != ActionButton.Deliverey &&
                cmd != ActionButton.Takeaway &&
                cmd != ActionButton.Table &&
                cmd != ActionButton.Served &&
                cmd != ActionButton.Del && cmd != ActionButton.DElIVERED)
            {
                ToastNotification.Notify("Enter the required value before ..", NotificationType.Warning);
                return;
            }

            switch (cmd)
            {
                case ActionButton.Del:
                    NumericZone = String.IsNullOrEmpty(NumericZone)
                        ? String.Empty
                        : NumericZone.Remove(NumericZone.Length - 1);
                    break;

                case ActionButton.Qty:
                    float qty;
                    try
                    {
                        qty = (float) Convert.ToDouble(NumericZone);
                    }
                    catch (Exception)
                    {
                        NumericZone = "";
                        return;
                    }

                    if (qty <= 0)
                    {
                        NumericZone = "";
                        return;
                    }

                    if (CurrentOrder?.SelectedOrderItem != null)
                        CurrentOrder.SelectedOrderItem.Quantity = qty;
                    NumericZone = "";
                    break;

                case ActionButton.Price:
                {
                    string numericZone = NumericZone;
                    if (CurrentOrder?.OrderItems == null || CurrentOrder.OrderItems.Count == 0)
                    {
                        ToastNotification.Notify("Add products before ...", NotificationType.Warning);
                        NumericZone = "";
                        return;
                    }

                    PriceAction(ref numericZone, CurrentOrder);
                    NumericZone = numericZone;
                    break;
                }

                case ActionButton.Disc:
                {
                    string numericZone = NumericZone;
                    DiscAction(ref numericZone, CurrentOrder);
                    NumericZone = numericZone;

                    break;
                }
                case ActionButton.Payment:

                    PayementAction();
                    break;

                case ActionButton.Cmd:
                    if (CurrentOrder?.OrderItems == null || CurrentOrder.OrderItems.Count == 0)
                    {
                        ToastNotification.Notify("Add products before...", NotificationType.Warning);
                        return;
                    }

                    var stamp = DateTime.Now;

                    _printOrder = null;

                    //OrdersCollectionObserver.Commit();
                    var currentOrderObserver = OrdersCollectionObserver[CurrentOrder] as ObjectGraphMutationObserver<Order>;
                    currentOrderObserver.Commit();
                    var orderitemsCollectionObserver = currentOrderObserver[nameof(Order.OrderItems)] as CollectionMutationObserver<OrderItem>;
                    var removedItems = orderitemsCollectionObserver?.GetRemovedItems(CurrentOrder.OrderItems).ToList();
                    removedItems.ForEach(i => i.State = OrderItemState.Removed);
                    ChangesMade = currentOrderObserver.IsMutated();

                    if (ChangesMade)
                    {
                        _printOrder = OrdersCollectionObserver[CurrentOrder].GetDifference(OrderHelper.GetOrderChanges);
                        //OrdersCollectionObserver.Push();
                        currentOrderObserver.Push();
                    }

                   

                    CurrentOrder.OrderItems.AddRange(removedItems);
                    orderitemsCollectionObserver.CommitAndPushAddedItems((i) => i.State != OrderItemState.Removed);
                    CurrentOrder.State = OrderState.Ordered;
                    SaveCurrentOrder();

                    if (ChangesMade)
                    {
                        PrintDocument(PrintSource.Kitchen); 
                    }
                    break;

                case ActionButton.Table:
                    int tableNumber;
                    if (string.IsNullOrEmpty(NumericZone))
                    {
                        if (TablesViewModel != null)
                        {
                            TablesViewModel.IsFullView = true;
                        }
                        else
                        {
                            TablesViewModel = new TablesViewModel(this);
                            TablesViewModel.IsFullView = true;
                        }

                        DialogViewModel = TablesViewModel;
                        //IsDialogOpen = true;
                        return;
                    }

                    try
                    {
                        tableNumber = Convert.ToInt32(NumericZone);
                    }
                    catch (Exception)
                    {
                        ToastNotification.Notify("Table Number should be integer", NotificationType.Warning);
                        NumericZone = "";
                        return;
                    }

                    TableAction(tableNumber);
                    break;

                case ActionButton.Split:
                    IsDialogOpen = CurrentOrder?.OrderItems != null && (CurrentOrder.OrderItems.Count > 1 ||
                                                                        (CurrentOrder.OrderItems.Count == 1 && CurrentOrder.OrderItems[0].Quantity > 1));
                    if (IsDialogOpen == true)
                    {

                        SplitViewModel = new SplitViewModel(this);
                        (this.Parent as MainViewModel)?.OpenDialog(SplitViewModel).OnClose(() =>
                        {
                            SplitViewModel = null;
                        });

                    }
                    else
                    {
                        ToastNotification.Notify("Non products to split", NotificationType.Warning);
                    }

                    break;
                case ActionButton.Deliverey:
                    if (CurrentOrder == null)
                    {
                        NewOrder();
                    }

                    SetCurrentOrderTypeAndRefreshOrdersLists(OrderType.Delivery);
                    break;

                case ActionButton.Takeaway:
                    if (CurrentOrder == null)
                    {
                        NewOrder();
                    }

                    SetCurrentOrderTypeAndRefreshOrdersLists(OrderType.TakeAway);
                    break;
                case ActionButton.Served:
                    if (CurrentOrder == null)
                    {
                        return;
                    }

                    CurrentOrder.State = OrderState.Served;
                    break;
                case ActionButton.DElIVERED:
                    if (CurrentOrder == null)
                    {
                        return;
                    }

                    if (CurrentOrder.Type != OrderType.Delivery)
                    {
                        ToastNotification.Notify("Order type must be Delivery", NotificationType.Warning);
                        return;
                    }

                    if (CurrentOrder.Deliveryman == null)
                    {
                        ToastNotification.Notify("Must assign a Deliveryman to Order", NotificationType.Warning);
                        return;
                    }

                    CurrentOrder.State = OrderState.Delivered;
                    SaveCurrentOrder();
                    break;
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

            if (CurrentOrder == null ||
                CurrentOrder.OrderItems == null ||
                CurrentOrder.OrderItems.Count < 1)
            {
                ToastNotification.Notify("Add products before ...", NotificationType.Warning);
                return;
            }

            if (payedAmount < CurrentOrder.NewTotal)
            {
                ToastNotification.Notify("Payed amount lower than total", NotificationType.Warning);
                return;
            }


            CurrentOrder.GivenAmount = payedAmount;
            CurrentOrder.ReturnedAmount = CurrentOrder.NewTotal - payedAmount;
            CurrentOrder.State = OrderState.Payed;
            NumericZone = "";
            GivenAmount = CurrentOrder.GivenAmount;
            ReturnedAmount = CurrentOrder.ReturnedAmount;
            _printOrder = _currentOrder;
            SaveCurrentOrder();
            GivenAmount = 0;
            ReturnedAmount = null;
            AdditivesVisibility = false;
            PrintDocument(PrintSource.Checkout);


        }

        private void TableAction(int tableNumber)
        {
            if (tableNumber < 0)
            {
                NumericZone = "";
                return;
            }

            var status = 200;
            Table table;
            if ((table = Tables.FirstOrDefault(t => t.Number == tableNumber)) == null)


                table = StateManager.Get<Table>(tableNumber);
            if (table == null)
            {
                ToastNotification.Notify("Table not found", NotificationType.Warning);
                return;
            }

            switch (status)
            {
                case 200:
                    if (CurrentOrder == null)
                    {
                        NewOrder();
                    }

                    SetCurrentOrderTypeAndRefreshOrdersLists(OrderType.OnTable, table);
                    break;
                case 400:
                    if (ActionConfig.AllowUsingVirtualTable)
                    {
                        if (StateManager.Save<Table>(new Table {IsVirtual = true, Number = tableNumber}))
                        {
                            ToastNotification.Notify("Creation of virtual table", NotificationType.Information);
                            NewOrder();
                        }
                        else
                        {

                        }
                    }

                    ToastNotification.ErrorNotification(status);
                    break;
                default:
                    ToastNotification.ErrorNotification(status);
                    break;
            }

            NumericZone = "";
        }

        private void TableAction(Table table)
        {
            if (table == null)
            {
                return;
            }

            if (table == CurrentOrder?.Table)
            {
                SetCurrentOrderTypeAndRefreshOrdersLists(OrderType.InWaiting);
            }

            if (CurrentOrder == null)
            {
                NewOrder();
            }

            SetCurrentOrderTypeAndRefreshOrdersLists(OrderType.OnTable, table);
        }

        private Table LookForTableInOrders(int tableNumber)
        {
            if (Tables == null)
            {
                return null;
            }

            foreach (var t in Tables)
            {
                if (t.Number == tableNumber)
                {
                    return t;
                }
            }

            return null;
        }

        private void PriceAction(ref string priceStr, Order order)
        {
            decimal price;
            if (priceStr == "")
            {
                return;
            }

            if (order.OrderItems.Any((item => item.DiscountAmount > 0)))
            {
                ToastNotification.Notify("Remove discount from order items in order to change the price of the order",
                    NotificationType.Error);
                return;
            }

            try
            {
                price = Convert.ToDecimal(priceStr);
            }
            catch (Exception)
            {
                priceStr = "";
                return;
            }

            if (price < 0)
            {
                priceStr = "";
                return;
            }

            var newTotal = price;
            if (newTotal <= order.Total)
            {
                var sumItemDiscounts = order.OrderItems.Sum(item=> item.DiscountAmount);

                order.DiscountAmount = order.Total - newTotal - sumItemDiscounts; // CurrentOrder.NewTotal;
                if (order.DiscountAmount < 0 && order.Total > newTotal)
                {
                    order.OrderItems.ToList().ForEach(item => item.DiscountAmount = 0);
                    order.DiscountAmount = order.Total - newTotal;
                }
            }
            else
            {
                priceStr = string.Empty;
                ToastNotification.Notify("New price less than the total price", NotificationType.Warning);
            }

            priceStr = "";
        }

        private void DiscAction(ref string discStr, Order order)
        {
            if (order == null)
            {
                return;
            }

            if (discStr == "")
            {
                return;
            }

            if (order.OrderItems.Any((item => item.DiscountAmount > 0)))
            {
                ToastNotification.Notify("Remove discount from order items in order to apply discount on order as a whole",NotificationType.Error);
                return;
            }

            var discountPercent = 0m;
            var discount = 0m;
            var isPercentage = false;
            if (discStr.Contains("%"))
            {
                isPercentage = true;
                if (discStr.Remove(discStr.Length - 1) == "")
                {
                    return;
                }

                discountPercent = Convert.ToDecimal(discStr.Remove(discStr.Length - 1));
                if (discountPercent > 100)
                {
                    discStr = string.Empty;
                    return;
                }

                discount = order.Total * discountPercent / 100;
            }
            else
            {
                discount = Convert.ToDecimal(discStr);
            }

            if (discount < 0)
            {
                discStr = "";
                return;
            }

            if (discount > order.Total)
            {
                //Use Local to select message according to UI language
                discStr = "";
                ToastNotification.Notify("Discount bigger than total", NotificationType.Warning);
                //CurrentOrder.DiscountAmount = 0;
                return;
            }

            if (!isPercentage)
            {
                order.DiscountAmount = discount;
            }
            else
            {
                order.DiscountPercentage = discountPercent;
            }

            discStr = "";
        }

        public void NumericKeyboard(string number)
        {
            if (String.IsNullOrEmpty(number))
                return;
            if (number.Length > 1)
                return;
            var numbers = new string[] {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", "%"};
            if (!numbers.Contains(number))
                return;
            if (NumericZone == null)
                NumericZone = String.Empty;

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
                var percentStr = _numericZone.Remove(_numericZone.Length - 1, 1) + number;
                var percent = Convert.ToDecimal(percentStr);
                if (percent < 0 || percent > 100)
                {
                    if (IsRunningFromXUnit)
                    {
                        throw new Exception("Invalid value for Percentagte");
                    }
                    else
                    {
                        ToastNotification.Notify("Invalid value for Percentagte", NotificationType.Warning);
                    }
                }
                else
                {
                    NumericZone = _numericZone.Remove(_numericZone.Length - 1, 1) + number + "%";
                }

                return;
            }

            NumericZone += number;
        }

        #endregion

        #region Order Item commands

        public void AddAditive(Additive additive)
        {
            if (additive == null)
            {
                return;
            }

            if (!CurrentOrder.SelectedOrderItem.CanAddAdditives)
            {
                AdditivesVisibility = false;
                ProductsVisibility = true;
                return;
            }

            if (!CurrentOrder.SelectedOrderItem.AddAdditive(additive))
            {
                CurrentOrder.SelectedOrderItem.SelectedAdditive = null;
                CurrentOrder.SelectedOrderItem.SelectedAdditive =
                    CurrentOrder.SelectedOrderItem.Additives.Where(addv => addv.Equals(additive)).FirstOrDefault();
            }
        }

        public void RemoveAdditive(Additive additive)
        {
            if (additive is null || additive.ParentOrderItem is null)
            {
                return;
            }

            if (additive.ParentOrderItem.Additives.Any(addtv => addtv.Equals(additive)))
            {
                CurrentOrder.SelectedOrderItem = additive.ParentOrderItem;
                additive.ParentOrderItem.RemoveAdditive(additive);
            }
        }

        public void RemoveOrerItem()
        {
            if (CurrentOrder?.SelectedOrderItem == null)
            {
                return;
            }


            CurrentOrder.RemoveOrderItem(CurrentOrder.SelectedOrderItem);


            OrderItemsCollectionViewSource.View.Refresh();

            AdditivesVisibility = false;
            ProductsVisibility = true;
        }

        public void AddOneToQuantity()
        {
            if (CurrentOrder?.SelectedOrderItem == null)
            {
                return;
            }

           
            CurrentOrder.SelectedOrderItem.Quantity += 1;
        }

        public void SubtractOneFromQuantity()
        {
            if (CurrentOrder?.SelectedOrderItem == null)
            {
                return;
            }

            var orderItem = CurrentOrder.SelectedOrderItem;
            if (orderItem.Quantity <= 1)
                return;
           
            orderItem.Quantity -= 1;
        }


        public void DiscountOnOrderItem(int param)
        {
            if (String.IsNullOrEmpty(NumericZone) && param != 0)
                return;

            if (CurrentOrder?.SelectedOrderItem == null)
            {
                return;
            }

            if (CurrentOrder.DiscountAmount > 0)
            {
                ToastNotification.Notify("Set order discount to 0 in order to apply a discount to selected order item");
                NumericZone = "";
                return;
            }

            var item = CurrentOrder.SelectedOrderItem;

            if (param == 0)
            {
                item.DiscountAmount = item.Product.Price;

                return;
            }

            var discountPercent = -1m;
            var discountAmount = -1m;
            var discount = 0m;
            if (NumericZone.Contains("%"))
            {
                if (NumericZone.Length == 1)
                {
                    return;
                }

                try
                {
                    discountPercent = Convert.ToDecimal(NumericZone.Remove(NumericZone.Length - 1));
                }
                catch (Exception)
                {
                    NumericZone = string.Empty;
                    if (IsRunningFromXUnit)
                        throw;
                }

                discount = item.Total * discountPercent / 100;
            }
            else
            {
                try
                {
                    discountAmount = discount = Convert.ToDecimal(NumericZone);
                }
                catch (Exception)
                {
                    NumericZone = string.Empty;
                    if (IsRunningFromXUnit)
                        throw;
                }
            }

            NumericZone = string.Empty;
            if (item.Total < discount)
            {
                ToastNotification.Notify("Discount Greater Than Price", NotificationType.Warning);
                return;
            }

            if (discountPercent >= 0)
            {
                item.DiscountPercentage = discountPercent;
                NotifyOfPropertyChange(() => CurrentOrder);
                return;
            }

            if (discountAmount >= 0)
            {
                item.DiscountAmount = discountAmount;
                NotifyOfPropertyChange(() => CurrentOrder);
            }
        }

        public void ScanCodeBar(object sender, TextCompositionEventArgs e)
        {
            scanValue += e.Text;
        }

        public void DoneScan(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.Return )
            {
                if (!string.IsNullOrEmpty(scanValue)&& scanValue.Contains("BON-"))
                {
                    var orderNumberString =Regex.Replace(scanValue, @"\D","");
                   
                    if (int.TryParse(orderNumberString,out var  orderNumber))
                    {
                        CurrentOrder = Orders.FirstOrDefault(order => order.OrderNumber == orderNumber);
                    }

                    scanValue = "";
                }   
            }
        }

        public void AddOrderItem(Product selectedproduct)
        {
            if (selectedproduct == null)
                return;

            if (CurrentOrder == null)
            {
                NewOrder();
            }

            var item = new OrderItem(selectedproduct, 1, CurrentOrder);

 
            OrderItem oi = CurrentOrder.AddOrderItem(item, true);

            OrderItemsCollectionViewSource.View.Refresh();

            if (selectedproduct.IsPlatter && selectedproduct.IdAdditives?.Count > 0)
            {
                ShowProductAdditives(selectedproduct);
                ProductsVisibility = false;
                AdditivesVisibility = true;
            }
        }

        public void ReturnFromAdditives()
        {
            ProductsVisibility = true;
            AdditivesVisibility = false;
        }

        public void GoToAdditiveButtonsPage()
        {
            if (CurrentOrder?.SelectedOrderItem == null)
            {
                return;
            }

            var currentOrderSelectedOrderItem = CurrentOrder.SelectedOrderItem;
            
            ShowProductAdditives(currentOrderSelectedOrderItem.Product);
        }

        #endregion

        public bool IsTopDrawerOpen
        {
            get => _IsTopDrawerOpen;
            set => Set(ref _IsTopDrawerOpen, value);
        }

        public BindableCollection<Deliveryman> Delivereymen { get; set; }
        public BindableCollection<Waiter> Waiters { get; private set; }

        public Deliveryman SelectedDeliveryman
        {
            get => _selectedDeliveryman;
            set
            {
                if (CurrentOrder == null && value != null)
                {
                    NewOrder();
                }

                Set(ref _selectedDeliveryman, value);

                if (CurrentOrder != null)
                {
                    CurrentOrder.Deliveryman = value;
                }

                IsTopDrawerOpen = false;
                if (value != null)
                {
                    SetCurrentOrderTypeAndRefreshOrdersLists(OrderType.Delivery);
                }
            }
        }

        public Waiter SelectedWaiter
        {
            get => _selectedWaiter;
            set
            {
                if (CurrentOrder == null && value != null)
                {
                    NewOrder();
                }

                Set(ref _selectedWaiter, value);

                if (CurrentOrder != null)
                {
                    Set(ref _selectedWaiter, value);
                    CurrentOrder.Waiter = value;
                }


                IsTopDrawerOpen = false;
            }
        }

        public void SelectDeliveryMan(Deliveryman deliveryman)
        {
            if (SelectedDeliveryman != deliveryman)
            {
                SelectedDeliveryman = deliveryman;
            }
            else
            {
                SelectedDeliveryman = null;
            }
        }

        public void SelectWaiter(Waiter waiter)
        {
            if (SelectedWaiter != waiter)
            {
                SelectedWaiter = waiter;
            }
            else
            {
                SelectedWaiter = null;
            }
        }

        public ListKind ListKind
        {
            get => _listKind;
            set
            {
                _listKind = value;
                NotifyOfPropertyChange();
            }
        }

        public void ShowDrawer(ListKind listKind) =>AppDrawerConductor.Instance.OpenTop(this, listKind);
     

        public void Handle(AssignOrderTypeEventArgs message)
        {
        }

        private FixedDocument GenerateOrderReceipt(PrintSource source)
        {
            FixedDocument document = new FixedDocument();
            FixedPage fixedPage = new FixedPage();


            DataTemplate dt = null;
            switch (source)
            {
                case PrintSource.Checkout:
                case PrintSource.CheckoutSplit:
                    dt = Application.Current.FindResource("CustomerTicketDataTemplate") as DataTemplate;
                    break;
                case PrintSource.Kitchen:
                    dt = Application.Current.FindResource("KitchenReceiptDataTemplate") as DataTemplate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }

            var contentOfPage = new UserControl();
            contentOfPage.ContentTemplate = dt;


            contentOfPage.Content = _printOrder;
         
            var conv = new LengthConverter();

            double width = (double) conv?.ConvertFromString("8cm");

            double height = document.DocumentPaginator.PageSize.Height;
            contentOfPage.Width = width;
            document.DocumentPaginator.PageSize = new Size(width, height);

            fixedPage.Children.Add(contentOfPage);
            PageContent pageContent = new PageContent();
            ((IAddChild) pageContent).AddChild(fixedPage);

            document.Pages.Add(pageContent);
            return document;
        }

        public object GenerateContent(Order order)
        {
            DateTime? recent = CurrentOrder.OrderItems.Max(oi => oi.TimeStamp);
            if (!ChangesMade)
            {
                ToastNotification.Notify("You made no updates", NotificationType.Information);
            }

            var value = new Order(order.Orders)
            {
                OrderItems = new BindableCollection<OrderItem>(order.OrderItems.Where(oi => oi.TimeStamp == recent))
            };
            ;
            return value;
        }

        public void PrintPreview(PrintSource source)
        {
            if (CurrentOrder == null) return;

            var doc = GenerateOrderReceipt(source);
            PrintViewModel pvm = new PrintViewModel {Document = doc, PreviousScreen = this, Parent = Parent};
            (Parent as MainViewModel).ActivateItem(pvm);


        }

        public void PrintDocument(PrintSource source)
        {

            if (_printOrder == null&& source == PrintSource.Kitchen)
            {
                ToastNotification.Notify("Select an order First");
                return;
            }

            if(_printOrder == null && source == PrintSource.Checkout)
            {
                ToastNotification.Notify("Select an order First");
                return;
            }

            if (SplitViewModel?.SplittedOrder == null && source == PrintSource.CheckoutSplit)
            {
                return;
            }

            _printOrder = source switch
            {
                PrintSource.Checkout => _printOrder,
                PrintSource.CheckoutSplit => SplitViewModel?.SplittedOrder,
                _ => _printOrder
            };

            SilentPrint(source);
        }

        private void SilentPrint(PrintSource source)
        {
            FixedDocument fixedDocument = GenerateOrderReceipt(source);
            var printers = PrinterSettings.InstalledPrinters.Cast<string>().ToList();

            
            IList<PrinterItem> printerItems = null;
           
            var printerItemSetting = AppConfigurationManager.Configuration<List<PrinterItem>>("PrintSettings");
            if (source == PrintSource.Kitchen)
            {
                printerItems = printerItemSetting.Where(item => item.SelectedKitchen).ToList();
            }

            if (source == PrintSource.Checkout|| source == PrintSource.CheckoutSplit)
            {
                printerItems = printerItemSetting.Where(item => item.SelectedReceipt).ToList();
            }

            foreach (var e in printerItems)
            {
                if (printers.Contains(e.Name))
                {
                    PrintDialog dialog = new PrintDialog {PrintQueue = new PrintQueue(new PrintServer(), e.Name)};
                    try
                    {
                        dialog.PrintDocument(fixedDocument.DocumentPaginator, "Print");
                    }
                    catch (Exception)
                    {

                    }
                }
            }

        }


        public void PaginateCategories(NextOrPrevious nextOrPrevious)
        {
            if (nextOrPrevious == NextOrPrevious.Next)
            {
                if (CurrentCategoryPageIndex < CategoryPageCount - 1)
                {
                    CurrentCategoryPageIndex++;
                }
            }

            if (nextOrPrevious == NextOrPrevious.Previous)
            {
                if (CurrentCategoryPageIndex != 0)
                {
                    CurrentCategoryPageIndex--;
                }
            }

            PaginatedCategories.View.Refresh();
        }

        private void CalculateTotalPages(int itemcount)
        {
            if (itemcount % itemsPerCategoryPage == 0)
            {
                CategoryPageCount = (itemcount / itemsPerCategoryPage);
            }
            else
            {
                CategoryPageCount = (itemcount / itemsPerCategoryPage) + 1;
            }
        }

        public int CategoryPageCount
        {
            get => _categoryPageCount;
            set => Set(ref _categoryPageCount, value);
        }

        private void PaginatedCategoriesOnFilter(object sender, FilterEventArgs e)
        {
            Category category = (e.Item as Category);

            int indexOfCategory = (int) category?.Rank - 1;

            if (indexOfCategory >= itemsPerCategoryPage * CurrentCategoryPageIndex &&
                indexOfCategory < itemsPerCategoryPage * (CurrentCategoryPageIndex + 1))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        public int CurrentCategoryPageIndex
        {
            get => _currentCategoryPageIndex;
            set => Set(ref _currentCategoryPageIndex, value);
        }

        public void CloseDrawer()
        {
            CustomerViewModel?.CustomerDetailVm?.Cancel();
        }

        public override bool CanNavigate(Type navigationTargetType = null)
        {
            if (navigationTargetType == typeof(LoginViewModel))
            {
                if (Orders != null && Orders.Any(o => o.Id == null))
                {
                
                    var response = ModalDialogBox.YesNo("There are unsaved orders, Are you sure you want to logout?","Unsaved Orders!").Show();
                
                    return response;
                }
            }

            return true;
        }

        protected override void OnDeactivate(bool close)
        {
            AppConfigurationManager.Save("OrderCountModifiedDate", DateTime.Today.ToString("yyyy-MM-dd"));
            AppConfigurationManager.Save("OrderCount", orderCount);
            base.OnDeactivate(close);
        }

        public Type [] SettingsControllers => new []
        {
            typeof(CheckoutSettingsViewModel),
            typeof(CustomerSettingsViewModel),
            typeof(WaiterSettingsViewModel),
            typeof(DeliveryManSettingsViewModel),
            typeof(GeneralSettingsViewModel),
        };

        public void OnSettingsUpdated(object sender, SettingsUpdatedEventArgs e)
        {
            IsTableViewActive = false;
            IsTakeawayViewActive = false;
            IsDeliveryViewActive = false;
            IsInWaitingViewActive = true;
            WaitingViewModel.OrderViewSource.View.Refresh();

            if (sender.GetType() == typeof(CheckoutSettingsViewModel))
            {
                OnCheckoutSettingsUpdated(e);
                return;
            }

            if (sender.GetType() == typeof(CustomerSettingsViewModel))
            {
                OnCustomerSettingsUpdated(e);
                return;
            }
            if (sender.GetType() == typeof(WaiterSettingsViewModel))
            {
                OnWaiterSettingsUpdated(e);
                return;
            }
            if (sender.GetType() == typeof(DeliveryManSettingsViewModel))
            {
                OnDeliverySettingsUpdated(e);
                return;
            }
            if (sender.GetType() == typeof(GeneralSettingsViewModel))
            {
                OnGeneralSettingsUpdated(e);
                return;
            }


        }

        private void OnCheckoutSettingsUpdated(SettingsUpdatedEventArgs e)
        {
            var products = e.Settings.FirstOrDefault(o => o is IEnumerable<Product>) as IEnumerable<Product>;
            var categories = e.Settings.FirstOrDefault(o => o is IEnumerable<Category>) as IEnumerable<Category>;

            AllProducts = products.ToList();
            AllCategories = categories.ToList();
            LoadCategoryPages();
            PaginatedCategories.Source = Categories;
            PaginatedCategories.View?.Refresh();
            CurrentCategory = null;
            ShowCategoryProducts(Categories[0]);
        }

        private void OnCustomerSettingsUpdated(SettingsUpdatedEventArgs e)
        {
            var customers = e.Settings.FirstOrDefault(o => o is IEnumerable<Customer>) as IEnumerable<Customer>;
            CustomerViewModel.CustomerCollectionViewSource.Source = customers;
            CustomerViewModel.CustomerCollectionViewSource.View.Refresh();

        }

        private void OnWaiterSettingsUpdated(SettingsUpdatedEventArgs e)
        {
            var waiters = e.Settings.FirstOrDefault(o => o is IEnumerable<Waiter>) as IEnumerable<Waiter>;
            Waiters = new BindableCollection<Waiter>(waiters);

        }

        private void OnDeliverySettingsUpdated(SettingsUpdatedEventArgs e)
        {
            var deliverymen = e.Settings.FirstOrDefault(o => o is IEnumerable<Deliveryman>) as IEnumerable<Deliveryman>;
            Delivereymen = new BindableCollection<Deliveryman>(deliverymen);

        }

        private void OnGeneralSettingsUpdated(SettingsUpdatedEventArgs e)
        {
            if (e.Settings!= null&& e.Settings.Any(o => o is int))
            {
                int itemsPerCategoryPage = (int)e.Settings.First(o => o is int) ;
                if (itemsPerCategoryPage!= this.itemsPerCategoryPage)
                {
                    this.itemsPerCategoryPage = itemsPerCategoryPage;
                    LoadCategoryPages();
                }
            }
            
            var tables = StateManager.Get<Table>().ToList();
            if (Tables.Count != tables.Count)
            {
                Tables = new BindableCollection<Table>(tables);
                TablesViewModel = new TablesViewModel(this);
            }
            
        }

        public void SetOrderPropToNull(OrderProp prop)
        {
            if (CurrentOrder!= null)
            {
                CurrentOrder.GetType().GetProperty(prop.ToString()).SetValue(CurrentOrder,null);
            }
        }

        public void SetSplitOrderNumber()
        {
            if (SplitViewModel?.SplittedOrder == null) return;
            
            SplitViewModel.SplittedOrder.OrderNumber = orderCount;
            orderCount++;
        }
    }

    public enum OrderProp
    {
        Customer,
        Deliveryman,
        Waiter,
        Table
    }
}