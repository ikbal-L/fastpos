using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Reactive.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
using FastPosFrontend.Configurations;
using FastPosFrontend.Enums;
using FastPosFrontend.EventManagement;
using FastPosFrontend.Events;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using FastPosFrontend.sse;
using FastPosFrontend.ViewModels.Settings;
using FastPosFrontend.ViewModels.Settings.Customer;
using FastPosFrontend.ViewModels.SubViewModel;
using LaunchDarkly.EventSource;
using Netina.Stomp.Client;
using Newtonsoft.Json;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;
using Utilities.Extensions;
using Utilities.Mutation.Observers;
using Table = ServiceInterface.Model.Table;

namespace FastPosFrontend.ViewModels
{
    [NavigationItem(
        title: /*Constants.Navigation.Checkout*/"", 
        target: typeof(CheckoutViewModel),"",
        keepAlive: true, isDefault: true,isQuickNavigationEnabled:true)]
    public class CheckoutViewModel : LazyScreen, IHandle<AssignOrderTypeEventArgs>,ISettingsListener
    {
        #region Private fields

        private static readonly bool IsRunningFromXUnit =
            AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.StartsWith("XUnitTesting"));

        private static string scanValue;
        private BindableCollection<Additive> _additvesPage;
       
        private BindableCollection<Product> _productsPage;
        private WarningViewModel _warningViewModel;
        private TablesViewModel _tablesViewModel;
        private WaitingViewModel _waitingViewModel;
        private DeliveryViewModel _deliveryViewModel;
        private TakeawayViewModel _takeAwayViewModel;
        private CustomerViewModel _customerViewModel;
        private Order _currentOrder;

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
        private int itemsPerCategoryPage;
        private int _categoryPageCount;
        private int _currentCategoryPageIndex;
        private Order _printOrder;
        private Category _currentCategory;
        private bool _isInWaitingViewActive = true;
        private bool _isTableViewActive;
        private bool _isTakeawayViewActive;
        private bool _isDeliveryViewActive;
        private ProductLayoutConfiguration _productLayout;
        private bool _isOrderInfoShown;
        private readonly DispatcherTimer _orderInfoCloseTimer;
        private string _lastModifiedOrderPropertyName;
        private Table _selectedOrderTable;
        private SplitViewModel _splitViewModel;



        #endregion

        #region Constructors


        public CollectionViewSource OrdersCollectionViewSource { get; set; }
        public CheckoutViewModel() : base()
        {
            LockOrderCommand = new DelegateCommandBase(LockOrder);
            ActionKeyboardCommand = new DelegateCommandBase(ActionKeyboard);
         
           
            SetupEmbeddedCommandBar();
            SetupEmbeddedStatusBar();

            _productLayout = ConfigurationManager.Get<PosConfig>().ProductLayout;

            CurrentCategoryPageIndex = 0;
            var configuration = ConfigurationManager.Get<PosConfig>().General;


            if (configuration != null)
            {
                itemsPerCategoryPage = configuration.CategoryPageSize;
            }
        }


        private void SetupEmbeddedCommandBar()
        {
            EmbeddedCommandBar = new EmbeddedCommandBarViewModel(this, "CheckoutLeftCommandBar");
        }

        private void SetupEmbeddedStatusBar()
        {
            EmbeddedRightCommandBar = new EmbeddedCommandBarViewModel(this, "CheckoutStatusBar");
        }

        protected override void Setup()
        {
            _orderRepo  = StateManager.GetService<Order, IOrderRepository>();
            var categories = StateManager.GetAsync<Category>();
            var unprocessedOrders = StateManager.GetAsync<Order>(predicate: "unprocessed");
            _data = new NotifyAllTasksCompletion(categories, unprocessedOrders);
        }

        public async override void Initialize()
        {
            var deliveryMen = StateManager.GetAll<Deliveryman>();
            var waiter = StateManager.GetAll<Waiter>();
            var tables = StateManager.GetAll<Table>();
            var customers = StateManager.GetAll<Customer>();
            var unprocessedOrders = StateManager.GetAll<Order>().ToList();
            var categories = StateManager.GetAll<Category>();
            var products = StateManager.GetAll<Product>();
            //unprocessedOrders.ForEach(o => o.PropertyChanged += CurrentOrder_PropertyChanged);
            Orders = new BindableCollection<Order>(unprocessedOrders);
            
            OrdersCollectionObserver = new CollectionMutationObserver<Order>(Orders,true,true);
            OrdersCollectionViewSource = new CollectionViewSource() { Source  = Orders };

            var recentOrdersPageRetreiver = new PageRetriever<Order>(RetriveRecentOrdersPage);
            RecentOrders = new Paginator<Order>(recentOrdersPageRetreiver);

            RecentUnpaidOrders = new CollectionPaginator<Order>(Orders);

            RecentUnpaidOrders.PaginationCollectionViewSource.SortDescriptions.Add(new SortDescription() { PropertyName = nameof(Order.OrderTime),Direction = ListSortDirection.Descending});
            
            
            ProductsPage = new BindableCollection<Product>();
            AdditivesPage = new BindableCollection<Additive>();
            Waiters = new BindableCollection<Waiter>(waiter);
            Delivereymen = new BindableCollection<Deliveryman>(deliveryMen);
            Tables = new BindableCollection<Table>(tables);
            Customers = new BindableCollection<Customer>(customers);
            OrderItemsCollectionViewSource = new CollectionViewSource();
            OrderItemsCollectionViewSource.Filter += (_,e)=> e.Accepted = 
            e.Item is OrderItem orderItem 
            && orderItem.State != OrderItemState.Removed; ;

            Task.Run(CalculateOrderElapsedTime);
          

            AllProducts = products.ToList();
            AllCategories = categories.ToList();

            LoadCategoryPages();

            PaginatedCategories = new CollectionViewSource {Source = Categories};

            PaginatedCategories.Filter += PaginatedCategoriesOnFilter;

            CalculateTotalPages(Categories.Count);

            foreach (var table in Tables)
            {
                table.AllOrders = Orders;
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

            DrawerManager.Instance.InitTop(this, "CheckoutWaiterDrawer", this, tag: ListKind.Waiter);
            DrawerManager.Instance.InitTop(this, "CheckoutDeliverymanDrawer", this, tag: ListKind.Delivery);
            DrawerManager.Instance.InitTop(this, "CheckoutTableDrawer", this, tag: ListKind.Table);
            DrawerManager.Instance.InitTop(this, "CheckoutCustomerDrawer", this, tag: ListKind.Customer);
            DrawerManager.Instance.InitBottom(this, "CheckoutOrderTabinationDrawer", this);

            var baseUrl = ConfigurationManager.Get<PosConfig>()?.Url;
            eventManager = new EventManagement.EventManager();
            eventManager.OnEvent<Order>(EventType.CREATE_ORDER,OnOrderCreated);
            eventManager.OnEvent<Order>(EventType.UPDATE_ORDER,OnOrderUpdated);
            eventManager.OnEvent<Order>(EventType.CANCEL_ORDER,OnOrderCanceled);
            eventManager.OnEvent<Order>(EventType.PAY_ORDER,OnOrderPayed);
            eventManager.OnEvent<List<long>>(EventType.LOCK_ORDER,OnOrderLocked);
            eventManager.OnEvent<List<long>>(EventType.UNLOCK_ORDER,OnOrderLocked);
            eventManager.OnConnectionClosed(ConnectionClosedHandler);
            await eventManager.ConnectAsync();
            await eventManager.ListenAsync<Message<Order>>("/topic/messages");
            await eventManager.ListenAsync<Message<List<long>>>("/topic/messages/locks",receiveFullMessage:true);
        }

        private Page<Order> RetriveRecentOrdersPage(int pageIndex, int pageSize)
        {
            var orderFilter = new OrderFilter()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                OrderBy = "orderTime",
                SortOrder = SortOrder.Desc,
                States = {  OrderState.Payed }
            };
            var result = _orderRepo.GetByCriterias(orderFilter);
            StateManager.Instance.Map(result.Elements);
            //result.Elements.ForEach(e => e.PropertyChanged += CurrentOrder_PropertyChanged);
            
            return result;
        }

        private void OnOrderLocked(IMessage<List<long>> message)
        {            
            RunOnTheMainThread(() =>
            {
                var orders = Orders.Where(o => o.Id != null && message.Content.Contains(o.Id.Value)).ToList();
                foreach (var order in orders)
                {
                    if (order != null && message.Type == "Lock.Order")
                    {
                        order.IsLocked = true;
                        order.LockedBy = message.Source;
                    
                    }
                    if (order != null && message.Type == "Unlock.Order")
                    {
                        order.IsLocked = false;
                        order.LockedBy = null;
                   
                    }
                }
            });
        }

        private void OnOrderPayed(Order incoming)
        {
            var orderToRemove = Orders.FirstOrDefault(o => o.Id == incoming?.Id);
            Orders.Remove(orderToRemove);
            OrdersCollectionObserver.UnObserve(orderToRemove);
            if (CurrentOrder == orderToRemove) CurrentOrder = null;
            UpdateOrderTabinationOnMainThread(incoming.Type.Value);
        }

        private void OnOrderCanceled(Order incoming)
        {
            var orderToRemove = Orders.FirstOrDefault(o => o.Id == incoming?.Id);
            Orders.Remove(orderToRemove);
            OrdersCollectionObserver.UnObserve(orderToRemove);
            if (CurrentOrder == orderToRemove) CurrentOrder = null;
            UpdateOrderTabinationOnMainThread(incoming.Type.Value);
        }

        private void OnOrderUpdated(Order incoming)
        {
            var updatedOrder = Orders.FirstOrDefault(o => o.Id == incoming.Id);
            var previousOrderType = updatedOrder?.Type;
            
            RunOnTheMainThread(() => 
            { 
                updatedOrder?.UpdateOrderFrom(incoming);
                if (updatedOrder is not null)
                {
                    HandleOrderChanges(updatedOrder);
                    if (incoming.State == OrderState.Ordered || incoming.State == OrderState.PaidModified)
                    {
                        PrintKitchenReciept(updatedOrder);
                    }
                }
            });


           


            UpdateOrderTabinationOnMainThread(incoming.Type.Value, previousOrderType);
        }

        private void UpdateOrderTabination(OrderType currentOrderType,OrderType? previousOrderType = null,int? currentTableNumber = null, int? perviousTableNumber = null)
        {
            UpdateOrdersTab(currentOrderType, currentTableNumber, perviousTableNumber);
            if (previousOrderType.HasValue)
            {
                UpdateOrdersTab(previousOrderType.Value, currentTableNumber, perviousTableNumber);
            }

        }

        private void UpdateOrderTabination(Order currentOrderState,Order? previousOrderState = null)
        {
            UpdateOrderTabination(currentOrderState.Type.Value, previousOrderState?.Type.Value,currentTableNumber:currentOrderState?.Table?.Number);
        }

        public void UpdateOrderTabinationOnMainThread(OrderType currentOrderType, OrderType? previousOrderType = null, int? currentTableNumber = null, int? perviousTableNumber = null)
        {
            RunOnTheMainThread(() =>
            {

                UpdateOrderTabination(currentOrderType,previousOrderType,currentTableNumber,perviousTableNumber);

            });


        }

        private void UpdateOrdersTab(OrderType orderType, int? currentTableNumber, int? perviousTableNumber)
        {
            if (orderType == OrderType.InWaiting)
            {
                WaitingViewModel.Orders.Refresh();
                WaitingViewModel.NotifyOfPropertyChange(() => WaitingViewModel.OrderCount);
                return;
            }

            if (orderType == OrderType.TakeAway)
            {
                TakeAwayViewModel.Orders.Refresh();
                TakeAwayViewModel.NotifyOfPropertyChange(() => TakeAwayViewModel.OrderCount);
                return;
            }

            if (orderType == OrderType.Delivery)
            {
                DeliveryViewModel.Orders.Refresh();
                DeliveryViewModel.NotifyOfPropertyChange(() => DeliveryViewModel.OrderCount);
                return;
            }

            if (orderType == OrderType.OnTable)
            {
                if (currentTableNumber.HasValue)
                {
                    UpdateTableOrdersView(currentTableNumber.Value);
                }

                if (perviousTableNumber.HasValue)
                {
                    UpdateTableOrdersView(perviousTableNumber.Value);
                }
            }
        }

        public void UpdateTableOrdersView(int tableNumber)
        {
            var table = TablesViewModel.Tables.First(t => t.Number == tableNumber);
            table?.Orders?.Refresh();
            TablesViewModel.NotifyOfPropertyChange(nameof(TablesViewModel.OrderCount));
        }

        private void OnOrderCreated(Order incoming)
        {
            if (incoming.TableId.HasValue)
            {
                RunOnTheMainThread(() => incoming.Table = StateManager.GetById<Table>(incoming.TableId.Value));   
            }

            if (incoming.DeliverymanId.HasValue)
            {
                incoming.Deliveryman = StateManager.GetById<Deliveryman>(incoming.DeliverymanId.Value);
                 
            }

            if (incoming.WaiterId.HasValue)
            {
                incoming.Waiter = StateManager.GetById<Waiter>(incoming.WaiterId.Value);
                
            }

            if (incoming.CustomerId.HasValue)
            {
                incoming.Customer = StateManager.GetById<Customer>(incoming.CustomerId.Value);
            }

            StateManager.Instance.MapItem(incoming);

            Orders.Add(incoming);
            UpdateOrderTabinationOnMainThread(incoming.Type.Value);
            
            if (incoming.State == OrderState.Ordered)
            {
                HandleOrderChanges(incoming);
                RunOnTheMainThread(() =>
                {
                    PrintKitchenReciept(incoming);
                });
            }
            OrdersCollectionObserver.ObserveItem(incoming);
            return;
        }

        private static void RunOnTheMainThread(System.Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(action);
        }

        public CollectionMutationObserver<Order> OrdersCollectionObserver { get; set; }

       
        private void CurrentOrder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
      
            if (e.PropertyName == nameof(Order.Table)&& CurrentOrder?.Table == null)
            {
                SelectedTable = null;
            }
            if (e.PropertyName == nameof(Order.Customer)||e.PropertyName == nameof(Order.Type)
            ||e.PropertyName == nameof(Order.Waiter)||e.PropertyName == nameof(Order.Table)|| e.PropertyName == nameof(Order.Deliveryman))
            {
      
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

            if (e.PropertyName == nameof(Order.NewTotal))
            {
                if (CurrentOrder!= null)
                {
                    if (CurrentOrder.State == OrderState.Payed|| CurrentOrder.State == OrderState.PaidModified)
                    {
                        CurrentOrderTotal = CurrentOrder.NewTotal - (CurrentOrder.PreModifyNewTotal?? CurrentOrder.NewTotal);
                    }
                    else
                    {
                        CurrentOrderTotal = CurrentOrder.NewTotal;

                    }
                }
            }

            if (e.PropertyName == nameof(Order.IsModifiable))
            {
                NotifyOfPropertyChange(nameof(CanModifyCurrentOrder));
                
            }
            if (e.PropertyName == nameof(Order.State))
            {
                NotifyOfPropertyChange(nameof(IsCurrentOrderPayed));
                NotifyOfPropertyChange(nameof(CanModifyCurrentOrder));
            }
        }

        public ProductLayoutConfiguration ProductLayout
        {
            get => _productLayout;
            set => Set(ref _productLayout, value);
        }

        private void OrderItemsCollectionViewSourceOnFilter(object sender, FilterEventArgs e)
        {

            
           
        }

        
        public List<Category> AllCategories { get; set; }

        #endregion

        private bool _isEditingPayedOrderEnabled;

        public bool IsEditingPayedOrderEnabled
        {
            get { return _isEditingPayedOrderEnabled; }
            set { Set(ref _isEditingPayedOrderEnabled, value); }
        }

        public bool CanSplitOrder
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

        public int MaxProductPageSize => _productLayout.NumberOfProducts;

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

        //TODO Selection display of Order Tabination
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
        public ObservableCollection<Order> Orders { get; set; }
        public BindableCollection<Customer> Customers { get; set; }

        public Order CurrentOrder
        {
            get { return _currentOrder; }
            set
            {
                if (value == null)
                {
                    if (_currentOrder!= null)
                    {
                        _currentOrder.PropertyChanged -= CurrentOrder_PropertyChanged; 
                    }
                    SetNullCurrentOrder();
                }
                else
                {
                    if (_currentOrder!= value)
                    {
                        if (_currentOrder!= null)
                        {
                            _currentOrder.PropertyChanged -= CurrentOrder_PropertyChanged; 
                        }
                        _currentOrder = value;

                        if (_currentOrder != null)
                        {
                            _currentOrder.PropertyChanged += CurrentOrder_PropertyChanged;
                        }
                    }
                }
                IsEditingPayedOrderEnabled = false;
                SetSelectedInListedOrdersDisplayedOrder();
                OrderItemsCollectionViewSource.Source = CurrentOrder?.OrderItems;
                NotifyOfPropertyChange(() => CurrentOrder);
                NotifyOfPropertyChange(() => CanModifyCurrentOrder);
                NotifyOfPropertyChange(() => IsCurrentOrderPayed);
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

        private CashOutViewModel _cashOutViewModel;

        public CashOutViewModel CashOutViewModel
        {   
            get { return _cashOutViewModel; }
            set { Set(ref _cashOutViewModel, value); }
        }

        private DeliveryCheckoutViewModel _deliveryCheckoutViewModel;

        public DeliveryCheckoutViewModel DeliveryCheckoutViewModel
        {
            get { return _deliveryCheckoutViewModel; }
            set { Set(ref _deliveryCheckoutViewModel , value); }
        }

        public Table SelectedTable
        {
            get => _selectedTable;
            set
            {
                Set(ref _selectedTable, value);

                if (value!= CurrentOrder?.Table)
                {
                    TableAction(value);
                }
                
            }
        }

        public Table SelectedOrderTable
        {
            get => _selectedOrderTable;
            set => Set(ref _selectedOrderTable, value);
        }

        public ObservableCollection<Table> Tables { get; set; }
        
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

        public decimal GivenAmount
        {
            get => givenAmount;
            set
            {
                givenAmount = value;
                NotifyOfPropertyChange(() => GivenAmount);
            }
        }

        private decimal _currentOrderTotal;
        private StompClient client;
        private EventManagement.EventManager eventManager;
        private IOrderRepository _orderRepo;

        public decimal CurrentOrderTotal
        {
            get { return _currentOrderTotal; }
            set { Set(ref _currentOrderTotal , value); }
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

        public ICommand LockOrderCommand { get; set; }

        public ICommand ActionKeyboardCommand { get; set; }

        #region Order Commands

        public bool SaveOrder(ref Order order)
        {
            return StateManager.Save(order);
        }

     

        private void SaveCurrentOrder(Action<Order> onOrderSaved = null)
        {

            if (StateManager.Save(CurrentOrder))
            {

                onOrderSaved?.Invoke(CurrentOrder);
                OrderState[] removalStates = { OrderState.Payed, OrderState.Canceled, OrderState.Removed, OrderState.Delivered, OrderState.Credit };
                if (removalStates.Any(s => s == CurrentOrder.State))
                {
                    RemoveCurrentOrderForOrdersList();
                };
            }
            else
            {
                ToastNotification.Notify("Unable to save order");
            }
          
            CurrentOrder = null;
            AdditivesVisibility = false;
            ProductsVisibility = true;
        }

        public void ShowOrder(Order order)
        {
            if (order == null) return;
            NumericZone = string.Empty;
            ReturnedAmount = null;

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

                if (CurrentOrder.State == OrderState.Payed || CurrentOrder.State == OrderState.PaidModified)
                {
                    CurrentOrderTotal = CurrentOrder.NewTotal - (CurrentOrder.PreModifyNewTotal ?? CurrentOrder.NewTotal);
                }
                else
                {
                    CurrentOrderTotal = CurrentOrder.NewTotal;

                }
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

            if (CurrentOrder!= null)
            {
                CurrentOrder.SelectedOrderItem = null; 
            }
        }

        public void NewOrder()
        {

            AdditivesVisibility = false;
            ProductsVisibility = true;
            CurrentOrder?.SaveScreenState(CurrentCategory, AdditivesPage, ProductsVisibility, AdditivesVisibility);

            CurrentOrder = new Order(Orders) ;
            

            OrderItemsCollectionViewSource.Source = CurrentOrder?.OrderItems;

            CurrentOrder.PropertyChanged += CurrentOrder_PropertyChanged;

            Orders.Add(CurrentOrder);

            OrdersCollectionObserver.ObserveItem(CurrentOrder);

            SetCurrentOrderTypeAndRefreshOrdersLists(OrderType.InWaiting);
            GivenAmount = 0;
            CurrentOrderTotal = CurrentOrder.NewTotal;
            ReturnedAmount = null;
            NumericZone = string.Empty;
            SelectedDeliveryman = null;
            SelectedWaiter = null;
            CustomerViewModel.SelectedCustomer = null;
            
        }

        public void LockOrder()
        {


            if (CurrentOrder== null) return;

            var action = CurrentOrder.IsLocked ? "unlock" : "lock";
            var baseUrl = ConfigurationManager.Get<PosConfig>().Url;
            RestApi api = new RestApi(baseUrl);
            var url = api.Resource("locks", $"{action}/order", CurrentOrder.Id);
            var res = GenericRest.RestPost("",url);
            if (res.IsSuccessful&& !CurrentOrder.IsLocked)
            {
                CurrentOrder.IsLocked = true;
                CurrentOrder.LockedBy = Thread.CurrentPrincipal.Identity.Name;
                return;
            }

            if (res.IsSuccessful&& CurrentOrder.IsLocked)
            {
                CurrentOrder.IsLocked = false;
                CurrentOrder.LockedBy = null;
                return;
            }

            ToastNotification.Notify("Something happened!");
        }

        public void ConnectionClosedHandler()
        {
            //if (!Orders.Any(o => o.IsLocked)) return;
            var baseUrl = ConfigurationManager.Get<PosConfig>().Url;
            RestApi api = new RestApi(baseUrl);
            var url = api.Resource("locks", "unlockAll/order");
            var res = GenericRest.RestPost("", url);
            if (res.IsSuccessful)
            {
                var ids = JsonConvert.DeserializeObject<List<long>>(res.Content);
                RunOnTheMainThread(() =>
                {
                    foreach (var order in Orders)
                    {
                        if (order.Id.HasValue&& ids.Contains(order.Id.Value))
                        {
                            order.IsLocked = false;
                            order.LockedBy = null;
                        }
                    }
                });
                return;
            }
            RunOnTheMainThread(()=> ToastNotification.Notify("Something happened!"));
        }

        public void LockOrder(object obj)
        {
            LockOrder();
        }

        private void SetCurrentOrderTypeAndRefreshOrdersLists(OrderType? orderType, Table table = null)
        {
            var previousOrderType = CurrentOrder?.Type;
            int? currentTableNumber = null;
            int? previousTableNumber = CurrentOrder.Table?.Number;
            if (CurrentOrder != null)
            {
                
                CurrentOrder.Type = orderType;
                if (orderType == OrderType.OnTable)
                {
                    CurrentOrder.Table = table;
                    currentTableNumber = CurrentOrder.Table?.Number;
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
                    _selectedTable = null;
                }
            }

            UpdateOrderTabination(orderType.Value, previousOrderType, currentTableNumber, previousTableNumber);

            SetSelectedInListedOrdersDisplayedOrder();
        }

        public void NewTotalToNumericZone()
        {
            if (CurrentOrder == null || CurrentOrder.NewTotal == 0)
            {
                return;
            }
            NumericZone = CurrentOrderTotal.ToString();
        }

        public void SetNullCurrentOrder()
        {
            CurrentOrder?.SaveScreenState(CurrentCategory, AdditivesPage, ProductsVisibility, AdditivesVisibility);
            
            _currentOrder = null;
            SelectedDeliveryman = null;
            SelectedWaiter = null;
            CustomerViewModel.SelectedCustomer = null;
            SetSelectedInListedOrdersDisplayedOrder();
        }

        public void PreviousOrder()
        {
            var index = Orders.IndexOf(CurrentOrder)-1;
            if (index>=0)
            {
                CurrentOrder = Orders[index]; 
            }
        }
        public void NextOrder()
        {
            var index = Orders.IndexOf(CurrentOrder) + 1;
            if (index<Orders.Count)
            {
                CurrentOrder = Orders[index];
            }
        }

        public void RemoveCurrentOrderForOrdersList()
        {
            if (CurrentOrder == null || Orders == null) {
                return;
            };
            var orderToRemove = CurrentOrder;
            Orders.Remove(orderToRemove);

            _ = OrdersCollectionObserver?.UnObserve(orderToRemove);

            orderToRemove.PropertyChanged -= CurrentOrder_PropertyChanged;
            UpdateOrderTabination(orderToRemove);
            CurrentOrder = null;
          
        }

        

        public void CancelOrder()
        {
            if (CurrentOrder == null)
            {
                ToastNotification.Notify("No order to remover / cancel!",NotificationType.Information);
                return;
            }

            
            if (CurrentOrder?.OrderNumber == null)
            {
                RemoveCurrentOrderForOrdersList();   
                return;
            }

            var main = Parent as MainViewModel;
            main?.OpenDialog(
                DefaultDialog
                    .New("Are you sure you want perform this action?")
                    .Title("Cancel Order")
                    .Ok(o =>
                    {
                        CancelOrderAction(this);
                        main.CloseDialog();
                    })
                    .Cancel(o =>
                    {
                        main.CloseDialog();
                    }));
        }

        public void CancelOrderAction(object param)
        {
            CurrentOrder.State = OrderState.Canceled;
            if (StateManager.Save(CurrentOrder))
            {
              
                PrintKitchenCancelReciept();
                RemoveCurrentOrderForOrdersList();
            }
            CanSplitOrder = false;
        }

        #endregion

        public bool ChangesMade { get; set; }

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
                            var lastStateTime = o.OrderStates?.LastOrDefault();

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


        void LoadCategoryPages()
        {
            var comparer = new Comparer<Category>();
            var retrieveCategories = AllCategories;
            var categories = new List<Category>(retrieveCategories.Where(c => c.Rank != null));
            categories.Sort(comparer);
            Categories = new BindableCollection<Category>();
            var maxRank =  categories.Max(c => c.Rank)??0;
            int _categpryPageSize = ConfigurationManager.Get<PosConfig>().General.CategoryPageSize;
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
                size: 30);
            AdditivesVisibility = true;
            ProductsVisibility = false;
        }


        #region Command Buttons' Actions
        public void ActionKeyboard(object obj)
        {
            ActionButton? cmd = null;
            if (obj is ActionButton a)
            {
                cmd = a;
            }
            if ( obj is string enumString&& Enum.TryParse(enumString,true, out ActionButton e))
            {
                cmd = e;   
            }
            if (cmd.HasValue)
            {
                ActionKeyboard(cmd.Value); 
            }
        }
        public void ActionKeyboard(ActionButton cmd)
        {
            if (cmd == ActionButton.CopyToNumericZone)
            {
                if (CurrentOrder != null) NumericZone = CurrentOrderTotal + "";
                return;
            }

            if (string.IsNullOrEmpty(NumericZone) &&
                cmd != ActionButton.Split &&
                cmd != ActionButton.Cmd &&
                cmd != ActionButton.Deliverey &&
                cmd != ActionButton.Takeaway &&
                cmd != ActionButton.Table &&
                cmd != ActionButton.Served &&
                cmd != ActionButton.Del && 
                cmd != ActionButton.DElIVERED&&
                cmd != ActionButton.Credit)
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
                    ChangeQtyAction();
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
                    PassOrderToKitchenAction();
                    break;

                case ActionButton.Table:

                    if (string.IsNullOrEmpty(NumericZone)) return;



                    if (!int.TryParse(NumericZone, out var tableNumber))
                    {
                        ToastNotification.Notify("Table Number should be integer", NotificationType.Warning);
                        NumericZone = "";
                        return;
                    }
                    TableAction(tableNumber);

                    break;

                case ActionButton.Split:
                    var nonRemovedOrderItemCount = CurrentOrder?.OrderItems.Count(oi => oi.State != OrderItemState.Removed);

                    CanSplitOrder = CurrentOrder?.OrderItems != null && nonRemovedOrderItemCount > 1 ||
                                                                        (nonRemovedOrderItemCount == 1 && CurrentOrder.OrderItems.FirstOrDefault(oi=>oi.State!= OrderItemState.Removed)?.Quantity > 1);
                    if (!CanSplitOrder)
                    {
                        ToastNotification.Notify("Non products to split", NotificationType.Warning); return;
                    }

                    SplitViewModel = new SplitViewModel(this);
                    (Parent as MainViewModel)?.OpenDialog(SplitViewModel).OnClose(() =>
                    {
                        SplitViewModel = null;
                    });

                    break;
                case ActionButton.Deliverey:
                    if (CurrentOrder == null) NewOrder();
                    SetCurrentOrderTypeAndRefreshOrdersLists(OrderType.Delivery);
                    break;

                case ActionButton.Takeaway:
                    if (CurrentOrder == null) NewOrder();
                    SetCurrentOrderTypeAndRefreshOrdersLists(OrderType.TakeAway);
                    break;
               
                case ActionButton.DElIVERED:
                    DeliveredAction();
                    break;

                case ActionButton.Credit:
                    CreditAction();
                    break;
            }
        }

        private void ShowTables()
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

            return;
        }

        private void ChangeQtyAction()
        {
            float qty;
            try
            {
                qty = (float)Convert.ToDouble(NumericZone);
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
        }

        private void PassOrderToKitchenAction()
        {
            if (CurrentOrder?.OrderItems == null || CurrentOrder.OrderItems.Count == 0)
            {
                ToastNotification.Notify("Add products before...", NotificationType.Warning);
                return;
            }

            HandleCurrentOrderChanges();



            if (CurrentOrder.State == OrderState.Payed|| CurrentOrder.State == OrderState.PaidModified)
            {
                CurrentOrder.State = OrderState.PaidModified;
            }
            else
            {
                CurrentOrder.State = OrderState.Ordered;
            }

            if (!Orders.Contains(CurrentOrder))
            {
                Orders.Add(CurrentOrder);
            }



            SaveCurrentOrder(PrintKitchenReciept);
        }

        public void PrintKitchenReciept(Order order)
        {
            if (ChangesMade)
            {
                _printOrder.Id = order.Id;
                _printOrder.OrderNumber = order.OrderNumber;
                _printOrder.OrderCode = order.OrderCode;
                PrintDocument(PrintSource.Kitchen);
            }
        }

        public void PrintKitchenCancelReciept()
        {
            _printOrder = CurrentOrder;
            PrintDocument(PrintSource.KitchenCancel);
        }

        private void HandleOrderChanges(Order order)
        {
            _printOrder = null;


            if (OrdersCollectionObserver.IsObserving(order))
            {
                var currentOrderObserver = OrdersCollectionObserver[order] as DeepMutationObserver<Order>;
                currentOrderObserver.Commit();
                var orderitemsCollectionObserver = currentOrderObserver[nameof(Order.OrderItems)] as CollectionMutationObserver<OrderItem>;
                var removedItems = orderitemsCollectionObserver?.GetRemovedItems(order.OrderItems).ToList();
                removedItems.ForEach(i => i.State = OrderItemState.Removed);
                ChangesMade = currentOrderObserver.IsMutated();

                if (ChangesMade)
                {
                    _printOrder = OrdersCollectionObserver[order].GetDifference(OrderHelper.GetOrderChanges);
                    currentOrderObserver.Push();
                }

                order.OrderItems.AddRange(removedItems);
                orderitemsCollectionObserver.CommitAndPushAddedItems((i) => i.State != OrderItemState.Removed);
                return;
            }
            ChangesMade = true;
            _printOrder = order;
        }

        private void HandleCurrentOrderChanges()
        {
            HandleOrderChanges(CurrentOrder);
        }

        private void CreditAction()
        {
            if (CurrentOrder == null)
            {
                return;
            }

            if (CurrentOrder.Customer == null)
            {

                var result = ModalDialogBox.OkCancel(this, "CheckoutCustomerDialogContent", "Customer",
                    (o) =>
                    {
                        return CurrentOrder.Customer != null;
                    }).Show();

                if (!result) return;


            }

            CurrentOrder.State = OrderState.Credit;
            //SaveCurrentOrder(o=>RaiseOrderEvent(EventType.PAY_ORDER,o));
            SaveCurrentOrder();
        }

        private void DeliveredAction()
        {
            if (CurrentOrder == null) return;

            if (CurrentOrder.Type != OrderType.Delivery && CurrentOrder.Type != OrderType.InWaiting)
            {
                ToastNotification.Notify("Order type must be Delivery", NotificationType.Warning);
                return;
            }

            if (CurrentOrder.Type == OrderType.InWaiting)
            {
                SetCurrentOrderTypeAndRefreshOrdersLists(OrderType.Delivery);
            }


            if (CurrentOrder.Deliveryman == null)
            {

                var result = ModalDialogBox.OkCancel(this, "CheckoutDeliverymanDialogContent", "Deliveryman", (o) =>
                {
                    return CurrentOrder.Deliveryman != null;
                }).Show();

                if (!result) return;
            }

            CurrentOrder.State = OrderState.Delivered;
            //SaveCurrentOrder(o=>RaiseOrderEvent(EventType.PAY_ORDER,o));
            SaveCurrentOrder();
        }

        private void PayementAction()
        {

            if (!decimal.TryParse(NumericZone,out var payedAmount))
            {
                return;
            }
            if (payedAmount < 0 && CurrentOrderTotal>=0)
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

            if (payedAmount < CurrentOrderTotal)
            {
                ToastNotification.Notify("Payed amount lower than total", NotificationType.Warning);
                return;
            }


            CurrentOrder.GivenAmount = payedAmount>0?payedAmount:0;
            CurrentOrder.ReturnedAmount = CurrentOrderTotal - payedAmount;
            CurrentOrder.State = OrderState.Payed;
            CurrentOrder.PreModifyNewTotal = CurrentOrder.NewTotal;
            IsEditingPayedOrderEnabled = false;

            ReturnedAmount = CurrentOrder.ReturnedAmount;
            _printOrder = _currentOrder;

            SaveCurrentOrder();
            GivenAmount = 0;
            
            AdditivesVisibility = false;
            PrintDocument(PrintSource.CheckoutPay);


        }

        private void RaiseOrderEvent(string eventType ,Order order)
        {
            eventManager.Publish(eventType, order);
        }

        private void TableAction(int tableNumber)
        {
            if (tableNumber < 0)
            {
                NumericZone = "";
                return;
            }

           
            Table table;
            if ((table = Tables.FirstOrDefault(t => t.Number == tableNumber)) == null)


           
            if (table == null)
            {
                ToastNotification.Notify("Table not found", NotificationType.Warning);
                    NumericZone = "";
                    return;
            }

            SelectedTable = table;

            

            NumericZone = "";
        }

        private void TableAction(Table table)
        {
            if (table == null)
            {
                CurrentOrder.Table = null;
                SetCurrentOrderTypeAndRefreshOrdersLists(OrderType.InWaiting, table);
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
                discStr = "";
                ToastNotification.Notify("Discount bigger than total", NotificationType.Warning);

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
            if (string.IsNullOrEmpty(number))
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

        public void AddAditive(Additive additive,string modifier)
        {
            if (additive == null) return;

            if (!CurrentOrder.SelectedOrderItem.CanAddAdditives)
            {
                AdditivesVisibility = false;
                ProductsVisibility = true;
                return;
            }

            if (!CurrentOrder.SelectedOrderItem.AddAdditive(additive,modifier))
            {
                CurrentOrder.SelectedOrderItem.SelectedAdditive = additive;
            }
        }

        public void RemoveAdditive(OrderItemAdditive orderItemAdditive)
        {
            orderItemAdditive.OrderItem.RemoveAdditive(orderItemAdditive);
           
        }

        public void RemoveOrerItem()
        {
            if (CurrentOrder?.SelectedOrderItem == null) return;

            CurrentOrder.RemoveOrderItem(CurrentOrder.SelectedOrderItem);

            OrderItemsCollectionViewSource.View.Refresh();

            AdditivesVisibility = false;
            ProductsVisibility = true;
        }

        public void AddOneToQuantity()
        {
            if (CurrentOrder?.SelectedOrderItem == null) return;

            CurrentOrder.SelectedOrderItem.Quantity += 1;
        }

        public void SubtractOneFromQuantity()
        {
            if (CurrentOrder?.SelectedOrderItem == null) return;

            var orderItem = CurrentOrder.SelectedOrderItem;
            if (orderItem.Quantity <= 1) return;

            orderItem.Quantity -= 1;
        }


        public void DiscountOnOrderItem(int param)
        {
            if (string.IsNullOrEmpty(NumericZone) && param != 0) return;

            if (CurrentOrder?.SelectedOrderItem == null) return;

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
                if (NumericZone.Length == 1) return;


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
                    if (IsRunningFromXUnit) throw;

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
            if (selectedproduct == null) return;


            if (CurrentOrder == null) NewOrder();


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
            if (CurrentOrder?.SelectedOrderItem == null) return;

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
                if (CurrentOrder == null && value != null) NewOrder();


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
                if (CurrentOrder == null && value != null) NewOrder();

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

        public void SelectTable(Table table)
        {
            if (SelectedTable != table)
            {
                SelectedTable = table;
            }
            else
            {
                SelectedTable = null;
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

        public void ShowDrawer(ListKind listKind) =>DrawerManager.Instance.OpenTop(this, listKind);
     

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
                case PrintSource.CheckoutPrint:
                case PrintSource.CheckoutSplit:
                case PrintSource.CheckoutPay:
                    dt = Application.Current.FindResource("CustomerTicketDataTemplate") as DataTemplate;
                    break;
                case PrintSource.Kitchen:
                    dt = Application.Current.FindResource("KitchenReceiptDataTemplate") as DataTemplate;
                    break;

                case PrintSource.KitchenCancel:
                    dt = Application.Current.FindResource("KitchenCancelReceiptDataTemplate") as DataTemplate;
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

        public void PrintDocument(PrintSource source)
        {

            if (_printOrder == null&& source == PrintSource.Kitchen)
            {
                ToastNotification.Notify("Select an order First");
                return;
            }

            if(CurrentOrder == null && source == PrintSource.CheckoutPrint)
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
                PrintSource.CheckoutPrint => CurrentOrder,
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

            var printerItemSetting = ConfigurationManager.Get<PosConfig>().Printing.Printers;
            if (source == PrintSource.Kitchen|| source == PrintSource.KitchenCancel)
            {
                printerItems = printerItemSetting?.Where(item => item.SelectedKitchen).ToList();
            }

            if (source == PrintSource.CheckoutPrint || source == PrintSource.CheckoutSplit || source == PrintSource.CheckoutPay)
            {
                printerItems = printerItemSetting?.Where(item => item.SelectedReceipt).ToList();
            }

            NewMethod(fixedDocument, printers, printerItems);

        }

        private static void NewMethod(FixedDocument fixedDocument, List<string> printers, IList<PrinterItem> printerItems)
        {
            if (printerItems != null)
            {
                foreach (var pi in printerItems)
                {
                    if (printers.Contains(pi.Name))
                    {
                        PrintDialog dialog = new PrintDialog { PrintQueue = new PrintQueue(new PrintServer(), pi.Name) };
                        try
                        {
                            dialog.PrintDocument(fixedDocument.DocumentPaginator, "Print");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, $"Error {ex.HResult}", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
            {
                ToastNotification.Notify("Configure Printers");
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
                ConnectionClosedHandler();
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
            var configuration = e.Settings.FirstOrDefault(o => o is ProductLayoutConfiguration) as ProductLayoutConfiguration;
            ProductLayout = configuration;

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
            NotifyOfPropertyChange(nameof(Waiters));

        }

        private void OnDeliverySettingsUpdated(SettingsUpdatedEventArgs e)
        {
            var deliverymen = e.Settings.FirstOrDefault(o => o is IEnumerable<Deliveryman>) as IEnumerable<Deliveryman>;
            Delivereymen = new BindableCollection<Deliveryman>(deliverymen);
            NotifyOfPropertyChange(nameof(Delivereymen));

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
            
            var tables = StateManager.GetAll<Table>().ToList();
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
                //CurrentOrder.GetType().GetProperty(prop.ToString()).SetValue(CurrentOrder,null);
                switch (prop)
                {
                    case OrderProp.Customer:
                        CurrentOrder!.Customer = null;
                        break;
                    case OrderProp.Deliveryman:
                        SelectedDeliveryman = null;
                        break;
                    case OrderProp.Waiter:
                        SelectedWaiter = null;
                        break;
                    case OrderProp.Table:
                        SelectedTable = null;
                        break;

                    default:
                        break;
                }
            }
        }



        public void AddExpense()
        {
            CashOutViewModel = new CashOutViewModel();
            var parent = Parent as MainViewModel;
            parent?.OpenDialog(CashOutViewModel)
                .OnClose(() =>
                {
                    if (CashOutViewModel != null)
                    {

                        CashOutViewModel = null;
                    }
                });
        }

        public void EditPayedOrder()
        {
            
            IsEditingPayedOrderEnabled = !IsEditingPayedOrderEnabled;
            NumericZone = string.Empty;

            if (IsEditingPayedOrderEnabled)
            {
                if (!OrdersCollectionObserver.IsObserving(CurrentOrder))
                {
                    OrdersCollectionObserver.ObserveItem(CurrentOrder);
                }


                if (CurrentOrder?.PreModifyNewTotal== null)
                {
                    CurrentOrder.PreModifyNewTotal = CurrentOrder.NewTotal;
                }
            }
            else
            {
                OrdersCollectionObserver?.UnObserve(CurrentOrder);
                CurrentOrder = null;
            }
            NotifyOfPropertyChange(nameof(CanModifyCurrentOrder));

        }

        public bool CanModifyCurrentOrder { 
            get
            {
                if (CurrentOrder != null)
                {
                    if (CurrentOrder.State == OrderState.Payed)
                    {
                        return CurrentOrder.IsModifiable && IsEditingPayedOrderEnabled;
                    }
                    return CurrentOrder.IsModifiable;
                }
                return true;
            }
        }

        public bool IsCurrentOrderPayed => CurrentOrder?.State == OrderState.Payed;

        public Paginator<Order> RecentOrders { get; private set; }
        public CollectionPaginator<Order> RecentUnpaidOrders { get; private set; }

        public void OnRecentOrdersPopupOpened()
        {
            RecentOrders.Reload();
        }

        public void ViewOrderTabs()
        {
            DrawerManager.Instance.OpenBottom(this);
        }

        public void NavigateToDeliveryCheckout()
        {
            var nav = (Parent as MainViewModel)?.Navigator;
            var navItem = nav?.QuickNavigationItems.FirstOrDefault(i => i.Target == typeof(DeliveryCheckoutViewModel));
            if (nav!= null)
            {
                nav.SelectedNavigationItem = navItem; 
            }

        }

        public void NavigateToCreditCheckout()
        {
            var nav = (Parent as MainViewModel)?.Navigator;
            var navItem = nav?.QuickNavigationItems.FirstOrDefault(i => i.Target == typeof(CreditCheckoutViewModel));
            if (nav != null)
            {
                nav.SelectedNavigationItem = navItem;
            }
        }

        public void RefundOrder()
        {
            CurrentOrder.State = OrderState.Refunded;
            StateManager.Save(CurrentOrder);
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