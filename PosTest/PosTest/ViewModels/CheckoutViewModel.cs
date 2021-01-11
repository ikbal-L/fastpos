﻿using Caliburn.Micro;
using PosTest.Enums;
using PosTest.Events;
using PosTest.Helpers;
using PosTest.ViewModels.SubViewModel;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Printing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Newtonsoft.Json;
using ServiceLib.Service;
using Table = ServiceInterface.Model.Table;
using System.Drawing.Printing;

namespace PosTest.ViewModels
{
    public class CheckoutViewModel : Screen, IHandle<AssignOrderTypeEventArgs>
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
        private DelivereyViewModel _delivereyViewModel;
        private TakeawayViewModel _takeAwayViewModel;
        private CustomerViewModel _customerViewModel;


        private Order _currentOrder;

        //private Order _displayedOrder;
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
        private Dictionary<int, OrderItem> _diff;
        private Order _printOrder;
        private Category _currentCategory;

        #endregion

        #region Constructors

        public CheckoutViewModel()
        {
        }

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

        public CheckoutViewModel(int pageSize
            //,
            //IProductService productsService,
            //ICategoryService categoriesService,
            //IOrderService orderService,
            //IWaiterService waiterService,
            //IDelivereyService delivereyService,
            //ICustomerService customerService
        ) : this()
        {
            _diff = new Dictionary<int, OrderItem>();
            MaxProductPageSize = pageSize;
            //_productsService = productsService;
            //_categoriesService = categoriesService;
            //_orderService = orderService;
            //_customerService = customerService;


            StateManager.Fetch();
            StateManager.Associate<Additive,Product>();
            StateManager.Associate<Product,Category>();

            //var (deliverymenStatusCode, deliveryMen) = delivereyService.GetAllDeliverymen();
            var deliveryMen = StateManager.Get<Deliveryman>();



            //var (waiterStatusCode, waiter) = waiterService.GetAllWaiters();
            var waiter = StateManager.Get<Waiter>();

            //var (tablesStatusCode, tables) = _orderService.GeAllTables();
            var tables = StateManager.Get<Table>();


            //var (customerStatusCode,customers) = _customerService.GetAllCustomers();
            var customers = StateManager.Get<Customer>();

            //var (orderStatusCode, unprocessedOrders) = _orderService.GetAllOrders(unprocessed: true);
            OrderState[] filteredTypes = {OrderState.Payed, OrderState.Canceled, OrderState.Removed};
            //var unprocessedOrders = StateManager.Get<Order>();
            var unprocessedOrders = StateManager.Get<Order>();


            var unprocessedTableOrders = unprocessedOrders.Where(uo => uo.Type == OrderType.OnTable).ToList();
            foreach (var table in tables.ToList())
            {
                var order = unprocessedTableOrders.FirstOrDefault(uo => uo.TableId == table.Id);
                if (order != null)
                {
                    
                    order.Table = table;
                    order.OrderItems.ToList().ForEach(oi=>oi.Additives
                    = new BindableCollection<Additive>());
                }
            }
            
            Orders = new BindableCollection<Order>(unprocessedOrders);

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

            //var ((catStatusCode, prodStatusCode), (categories, products)) =
            //    _categoriesService.GetAllCategoriesAndProducts();
            var products = StateManager.Get<Product>();
            var categories = StateManager.Get<Category>();
            //if (catStatusCode != 200 && prodStatusCode != 200) return;
            AllProducts = products.ToList();
            AllCategories = categories.ToList();


            LoadCategoryPages();


            PaginatedCategories = new CollectionViewSource();
            PaginatedCategories.Source = Categories;
            CurrentCategoryPageIndex = 0;
            itemsPerCategoryPage = 5;
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
            DelivereyViewModel = new DelivereyViewModel(this);
            WaitingViewModel = new WaitingViewModel(this);
            CustomerViewModel = new CustomerViewModel(this/*, customerService*/);
            TablesViewModel = new TablesViewModel(this);
            CurrentCategory = Categories[0];
            ShowCategoryProducts(CurrentCategory);
        }

        private void CurrentOrder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentOrder.OrderItems))
            {
                OrderItemsCollectionViewSource.Source = CurrentOrder.OrderItems;
                //OrderItemsCollectionViewSource.View.Refresh();
            }
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

        public List<Category> AllCategories { get; set; }

        #endregion

        #region Properties

        public bool IsDialogOpen
        {
            get => _IsDialogOpen;
            set => Set(ref _IsDialogOpen, value);
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

        //public Order DisplayedOrder
        //{
        //    get => _displayedOrder;
        //    set
        //    {
        //        _displayedOrder = value;
        //        NotifyOfPropertyChange();
        //    }
        //}

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

            DelivereyViewModel.SelectedOrder = DelivereyViewModel.Orders.Cast<Order>().Where(o => o == CurrentOrder)
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
                    //_displayedOrder = value;
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
            set
            {
                _customerViewModel = value;
                NotifyOfPropertyChange();
            }
        }

        public DelivereyViewModel DelivereyViewModel
        {
            get => _delivereyViewModel;
            set
            {
                _delivereyViewModel = value;
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

        public Table SelectedTable
        {
            get => _selectedTable;
            set
            {
                TableAction(value);
                Set(ref _selectedTable, value);
            }
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

        public bool? SaveOrder(ref Order order)
        {
            if (IsRunningFromXUnit)
            {
                return null;
            }

            bool resp;
            try
            {
                resp = StateManager.Save<Order>(order);
            }
            catch (AggregateException)
            {
                ToastNotification.Notify("Check your server connection");
                return null;
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
                        CurrentOrder.State == OrderState.Removed)
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
        }

        public void ShowOrder(Order order)
        {
            if (order == null || order == CurrentOrder) return;


            CurrentOrder?.SaveScreenState(CurrentCategory, AdditivesPage, ProductsVisibility, AdditivesVisibility);

            CurrentOrder = order;
            //DisplayedOrder = order;

            if (order.ShownCategory == null && order.Id != null)
            {
                order.ShownCategory = CurrentCategory;
                order.ProductsVisibility = ProductsVisibility;
                order.AdditivesVisibility = AdditivesVisibility;
            }

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
            if (CurrentOrder != null)
            {
                CurrentOrder.SaveScreenState(CurrentCategory, AdditivesPage, ProductsVisibility, AdditivesVisibility);
            }

            CurrentOrder = new Order(this.Orders);
            OrderItemsCollectionViewSource.Source = CurrentOrder?.OrderItems;
            //OrderItemsCollectionViewSource.View.Refresh();
            //CurrentOrder.PropertyChanged += CurrentOrder_PropertyChanged;
            //DisplayedOrder = CurrentOrder;
            Orders.Add(CurrentOrder);
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
                }
                else
                {
                    if (orderType == OrderType.TakeAway)
                    {
                        SelectedDeliveryman = null;
                    }

                    CurrentOrder.Table = null;
                    SelectedTable = null;
                }
            }


            DelivereyViewModel.OrderViewSource.View.Refresh();
            WaitingViewModel.OrderViewSource.View.Refresh();
            TakeAwayViewModel.OrderViewSource.View.Refresh();
            DelivereyViewModel.NotifyOfPropertyChange(() => DelivereyViewModel.OrderCount);
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
            if (CurrentOrder != null)
            {
                CurrentOrder.SaveScreenState(CurrentCategory, AdditivesPage, ProductsVisibility, AdditivesVisibility);
            }

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
            CurrentOrder = null;
            SetCurrentOrderTypeAndRefreshOrdersLists(null);
        }

        public void CancelOrder()
        {
            if (CurrentOrder == null)
            {
                return;
            }

            WarningViewModel = new WarningViewModel("Are you sure to delete this Order?", "Check", "Ok", "Close", "No",
                o => CancelOrderAction(o), this, () => IsDialogOpen = false);
            DialogViewModel = WarningViewModel;
            IsDialogOpen = true;
        }


        public void CancelOrderAction(object param)
        {
            var checkoutViewModel = param as CheckoutViewModel;
            if (checkoutViewModel.CurrentOrder.State == null)
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

        public void CloseCommand()
        {
            LoginViewModel loginvm = new LoginViewModel();
            loginvm.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(loginvm);
        }

        IEnumerable<Category> RetrieveCategories(IEnumerable<Product> products)
        {
            var categories = new HashSet<Category>();
            foreach (var p in products)
            {
                categories.Add(p.Category);
                if (p.Category.Products == null)
                {
                    p.Category.Products = new List<Product>();
                }

                if (!p.Category.Products.Any(pr => pr == p))
                {
                    p.Category.Products.Add(p);
                }
            }

            return categories;
        }

        public bool ChangesMade { get; set; }

        void LoadCategoryPages()
        {
            var comparer = new Comparer<Category>();
            var retrieveCategories = AllCategories;
            var categories = new List<Category>(retrieveCategories.Where(c => c.Rank != null));
            categories.Sort(comparer);
            Categories = new BindableCollection<Category>();
            var maxRank = (int) categories.Max(c => c.Rank);
            int _categpryPageSize = 10;
            int nbpage = (maxRank / _categpryPageSize) + (maxRank % _categpryPageSize == 0 ? 0 : 1);
            nbpage = nbpage == 0 ? 1 : nbpage;
            var size = nbpage * _categpryPageSize;

            RankedItemsCollectionHelper.LoadPagesFilled(source: categories, target: Categories, size: size);
        }

        public void ShowCategoryProducts(Category category)
        {
            if (category == CurrentCategory && !(ProductsPage.Count == 0 || ProductsPage == null)) return;

            ProductsPage.Clear();
            ProductsVisibility = true;
            if (category?.Id == null) return;
            var filteredProducts = AllProducts.Where(p => p.Category == category && p.Rank != null);

            var comparer = new Comparer<Product>();
            var listOfFliteredProducts = filteredProducts.ToList();
            listOfFliteredProducts.Sort(comparer);
            CurrentCategory = category;
            RankedItemsCollectionHelper.LoadPagesNotFilled(source: listOfFliteredProducts, target: ProductsPage,
                size: MaxProductPageSize, parameter: category);
        }

        public void ShowProductAdditives(Platter product)
        {
            AdditivesPage?.Clear();
            if (product == null) return;
            var comparer = new Comparer<Additive>();
            var additives = product.Additives.ToList();
            additives.Sort(comparer);

            RankedItemsCollectionHelper.LoadPagesNotFilled(source: additives, target: AdditivesPage,
                size: MaxProductPageSize);
        }

        #region Filtering and pagination

        //public void CategorieFiltering(object param)
        //{
        //    AdditivesVisibility = false;
        //    ProductsVisibility = true;
        //    if (param is Category)
        //    {
        //        Category category = param as Category;
        //        category.BackgroundString = DefaultColors.Category_SelectedBackground.ToString();
        //        if (CurrentCategory != null)
        //            CurrentCategory.BackgroundString = DefaultColors.Category_DefaultBackground.ToString();
        //        CurrentCategory = category;
        //        FilteredProducts.Filter = (p) => (p as Product).CategoryId.Equals(_currantCategory.Id);

        //        PaginateProducts(NextOrPrevious.First);
        //    }
        //    else
        //        if (param is string)
        //        if ((param as string).ToUpperInvariant().Equals("Home".ToUpperInvariant()))
        //        {
        //            FilteredProducts.Filter = null;

        //            if (CurrentCategory != null)
        //                CurrentCategory.BackgroundString = DefaultColors.Category_DefaultBackground.ToString();

        //            CurrentCategory = null;
        //            var muchInDemanadProducts = AllProducts.Where(p => p.IsMuchInDemand == true).ToList();
        //            FilteredProducts = CollectionViewSource.GetDefaultView(muchInDemanadProducts);
        //            PaginateProducts(NextOrPrevious.First);
        //        }
        //        else // param == "ALL"
        //        {
        //            FilteredProducts = CollectionViewSource.GetDefaultView(AllProducts);
        //            PaginateProducts(NextOrPrevious.First);
        //        }
        //}

        //public void ProductFiltering(string text)
        //{
        //    Console.WriteLine("Param=" + text);
        //}

        //public void PaginateProducts(NextOrPrevious nextOrprevious)
        //{
        //    if (nextOrprevious == NextOrPrevious.First)
        //    {
        //        _pageNumber = 0;
        //        nextOrprevious = NextOrPrevious.Next;
        //        CanExecuteMext = false;
        //        CanExecutePrevious = false;
        //    }

        //    if (FilteredProducts.Cast<Product>().Count() <= MaxProductPageSize)
        //    {
        //        ProductsPage = FilteredProducts;
        //        _pageNumber = 1;
        //        CanExecuteMext = false;
        //        CanExecutePrevious = false;
        //        return;
        //    }

        //    var listProducts = FilteredProducts.Cast<Product>().ToList();
        //    int lastIndexForThisPage = (_pageNumber + 1) * MaxProductPageSize;
        //    if (nextOrprevious == NextOrPrevious.Next)
        //    {
        //        if (listProducts.Count > lastIndexForThisPage)
        //        {
        //            listProducts = listProducts.GetRange(_pageNumber * MaxProductPageSize, MaxProductPageSize);
        //            CanExecuteMext = true;
        //        }
        //        else
        //        {
        //            listProducts = listProducts.GetRange(_pageNumber * MaxProductPageSize, listProducts.Count - (_pageNumber * MaxProductPageSize));
        //            CanExecuteMext = false;
        //        }
        //        _pageNumber++;
        //    }
        //    else
        //    {
        //        if ((_pageNumber - 2) * MaxProductPageSize < 0)
        //            return;
        //        listProducts = listProducts.GetRange((_pageNumber - 2) * MaxProductPageSize, MaxProductPageSize);
        //        _pageNumber--;
        //        CanExecuteMext = true;

        //    }
        //    ProductsPage = CollectionViewSource.GetDefaultView(listProducts);

        //    CanExecutePrevious = _pageNumber == 1 ? false : true;
        //}

        #endregion

        #region Command Buttons' Actions

        public void ActionKeyboard(ActionButton cmd)
        {
            if (string.IsNullOrEmpty(NumericZone) &&
                cmd != ActionButton.Split &&
                cmd != ActionButton.Cmd &&
                cmd != ActionButton.Deliverey &&
                cmd != ActionButton.Takeaway &&
                cmd != ActionButton.Table &&
                cmd != ActionButton.Served &&
                cmd != ActionButton.Del)
            {
                ToastNotification.Notify("Enter the required value before ..");
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
                    if (CurrentOrder.Type == OrderType.InWaiting)
                    {
                        ToastNotification.Notify("Set order type first");
                        return;
                    }

                    PayementAction();
                    break;

                case ActionButton.Cmd:
                    if (CurrentOrder == null || CurrentOrder.OrderItems == null || CurrentOrder.OrderItems.Count == 0)
                    {
                        ToastNotification.Notify("Add products before ...");
                        return;
                    }

                    var stamp = DateTime.Now;


                    _printOrder = null;

                    var b1 = OrderManagementHelper.StampAndSetOrderItemState(stamp, CurrentOrder.OrderItems, _diff);

                    var b2 = OrderManagementHelper.StampAdditives(stamp, CurrentOrder.OrderItems);
                    ChangesMade = b1 || b2;

                    _printOrder = OrderManagementHelper.GetChangesFromOrder(CurrentOrder, _diff);
                    CurrentOrder.State = OrderState.Ordered;
                    _diff.Clear();
                    SaveCurrentOrder();
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
                        ToastNotification.Notify("Table Number should be integer");
                        NumericZone = "";
                        return;
                    }

                    TableAction(tableNumber);
                    break;

                case ActionButton.Split:
                    IsDialogOpen = CurrentOrder != null
                                   && CurrentOrder.OrderItems != null
                                   && (CurrentOrder.OrderItems.Count > 1 ||
                                       (CurrentOrder.OrderItems.Count == 1 && CurrentOrder.OrderItems[0].Quantity > 1));
                    if (IsDialogOpen == true)
                    {
                        DialogViewModel = new SplitViewModel(this);
                    }
                    else
                    {
                        ToastNotification.Notify("Non products to split");
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
                ToastNotification.Notify("Add products before ...");
                return;
            }

            if (payedAmount < CurrentOrder.NewTotal)
            {
                ToastNotification.Notify("Payed amount lower than total");
                return;
            }

            CurrentOrder.GivenAmount = payedAmount;
            CurrentOrder.ReturnedAmount = CurrentOrder.NewTotal - payedAmount;
            CurrentOrder.State = OrderState.Payed;
            NumericZone = "";
            GivenAmount = CurrentOrder.GivenAmount;
            ReturnedAmount = CurrentOrder.ReturnedAmount;
            SaveCurrentOrder();
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
            if ((table = Tables.FirstOrDefault(t=>t.Number==tableNumber)) == null)
               
                //TODO introduce table by id method in table repository 
                table =StateManager.Get<Table>(tableNumber);
            if (table == null)
            {
                ToastNotification.Notify("Table not found");
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
                        
                        if (StateManager.Save<Table>(new Table { IsVirtual = true, Number = tableNumber }))
                        {
                            ToastNotification.Notify("Creation of virtual table", 1);
                            NewOrder();
                        }
                        else
                        {
                            //ToastNotification.ErrorNotification(status2);
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
                var sumItemDiscounts = 0m;
                order.OrderItems.ToList().ForEach(item => sumItemDiscounts += item.DiscountAmount);
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
                ToastNotification.Notify("New price less than the total price");
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
                ToastNotification.Notify("Discount bigger than total");
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
                        ToastNotification.Notify("Invalid value for Percentagte");
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

            if (!CurrentOrder.SelectedOrderItem.AddAdditives(additive))
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
            if (CurrentOrder.SelectedOrderItem == null)
            {
                return;
            }

            if (CurrentOrder.SelectedOrderItem.TimeStamp==null)
            {
                if (_diff.ContainsKey(CurrentOrder.SelectedOrderItem.GetHashCode()))
                {
                    _diff.Remove(CurrentOrder.SelectedOrderItem.GetHashCode());
                }
            }
            
            CurrentOrder.RemoveOrderItem(CurrentOrder.SelectedOrderItem);
            if (CurrentOrder.SelectedOrderItem != null)
            {
                OrderManagementHelper.TrackItemForChange(CurrentOrder.SelectedOrderItem, _diff);
            }

            OrderItemsCollectionViewSource.View.Refresh();

            AdditivesVisibility = false;
            ProductsVisibility = true;
        }

        public void AddOneToQuantity()
        {
            if (CurrentOrder.SelectedOrderItem == null)
            {
                return;
            }

            OrderManagementHelper.TrackItemForChange(CurrentOrder.SelectedOrderItem, _diff);
            CurrentOrder.SelectedOrderItem.Quantity += 1;
        }

        public void SubtractOneFromQuantity()
        {
            if (CurrentOrder.SelectedOrderItem == null)
            {
                return;
            }

            var orderItem = CurrentOrder.SelectedOrderItem;
            if (orderItem.Quantity <= 1)
                return;
            OrderManagementHelper.TrackItemForChange(orderItem, _diff);
            orderItem.Quantity -= 1;
        }


        public void DiscountOnOrderItem(int param)
        {
            if (String.IsNullOrEmpty(NumericZone) && param != 0)
                return;

            if (CurrentOrder.SelectedOrderItem == null)
            {
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
                ToastNotification.Notify("Discount Greater Than Price");
                return;
            }

            if (discountPercent >= 0)
            {
                item.DiscountPercentage = discountPercent;
            }

            if (discountAmount >= 0)
            {
                item.DiscountAmount = discountAmount;
                NotifyOfPropertyChange(() => CurrentOrder);
            }
        }

        public void scanCodeBar(object sender, TextCompositionEventArgs e)
        {
            scanValue += e.Text;
        }

        public void doneScan(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ToastNotification.Notify(scanValue);

                if (scanValue.Contains("BON"))
                {
                    CurrentOrder.State = OrderState.Ordered;
                    NotifyOfPropertyChange(() => CurrentOrder);
                }

                scanValue = "";
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

            var item = new OrderItem(selectedproduct, 1, selectedproduct.Price, CurrentOrder);

            var fetch = CurrentOrder.OrderItems.FirstOrDefault(i =>
                i.ProductId == selectedproduct.Id && i.TimeStamp != null);
            if (fetch != null)
            {
                OrderManagementHelper.TrackItemForChange(fetch, _diff);
            }

            //CurrentOrder.AddOrderItem(product: selectedproduct, unitPrice: selectedproduct.Price, setSelected: true,
            //    quantity: 1);
            OrderItem oi = CurrentOrder.AddOrderItem(item, true);
            if ((fetch == null || fetch.State == OrderItemState.Removed))
            {
                OrderManagementHelper.TrackItemForChange(oi, _diff);
            }
            //OrderItemsCollectionViewSource.View.Refresh();

            if (selectedproduct is Platter && (selectedproduct as Platter).Additives != null)
            {
                ShowProductAdditives(selectedproduct as Platter);
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
            if (CurrentOrder.SelectedOrderItem == null)
            {
                return;
            }

            var oitem = CurrentOrder.SelectedOrderItem;
            AdditivesVisibility = true;
            ProductsVisibility = false;
            ShowProductAdditives(oitem.Product as Platter);
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

                if (CurrentOrder != null && CurrentOrder.Type ==OrderType.OnTable)
                {
                    Set(ref _selectedWaiter, value);
                    CurrentOrder.Waiter = value;
                }


                IsTopDrawerOpen = false;
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

        public void ShowDrawer(ListKind listKind)
        {
            ListKind = listKind;
            IsTopDrawerOpen = true;
        }

        public void Handle(AssignOrderTypeEventArgs message)
        {
        }

        private FixedDocument GenerateOrderReceipt()
        {
            FixedDocument document = new FixedDocument();
            FixedPage fixedPage = new FixedPage();
            DataTemplate dt = Application.Current.FindResource("CustomerTicketDataTemplate") as DataTemplate;

            var contentOfPage = new UserControl();
            contentOfPage.ContentTemplate = dt;
            //contentOfPage.Content = CurrentOrder;
            //contentOfPage.Content = GenerateContent(CurrentOrder);
            contentOfPage.Content = _printOrder;
            _diff.Clear();
            var conv = new LengthConverter();

            //double width = (double)conv.ConvertFromString("8cm");
            //double height = document.DocumentPaginator.PageSize.Height;

            //document.DocumentPaginator.PageSize = new Size(width, height);

            fixedPage.Width = contentOfPage.Width;
            fixedPage.Height = contentOfPage.Height;
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
                ToastNotification.Notify("You made no updates");
            }

            var value = new Order(order.Orders)
            {
                OrderItems = new BindableCollection<OrderItem>(order.OrderItems.Where(oi => oi.TimeStamp == recent))
            };
            ;
            return value;
        }

        public void PrintPreview()
        {
            if (CurrentOrder == null) return;

            var doc = GenerateOrderReceipt();
            PrintViewModel pvm = new PrintViewModel() {Document = doc, PreviousScreen = this};
            pvm.Parent = this.Parent;
            (this.Parent as MainViewModel).ActivateItem(pvm);

            //var xpsDoc = GenerateXpsDocument($"customerReceipt{DateTime.Now.ToFileTime()}");
            //WriteXpsDocument(doc, xpsDoc);

            //PrintDocument();
        }


        public XpsDocument GenerateXpsDocument(string name)
        {
            //XpsDocument document = new XpsDocument($"/Printing/{name}.xps",FileAccess.ReadWrite);
            XpsDocument document = new XpsDocument($"{name}.xps", FileAccess.ReadWrite);
            return document;
        }

        public void WriteXpsDocument(FixedDocument fixedDocument, XpsDocument xpsDocument)
        {
            XpsDocumentWriter xpsDocumentWriter = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
            xpsDocumentWriter.Write(fixedDocument);
            xpsDocument.Close();
        }

        public void PrintDocument()
        {
            FixedDocument fixedDocument = GenerateOrderReceipt();
         //    var printer=  PrinterSettings.InstalledPrinters.Cast<string>().ToList();
            PrintDialog dialog = new PrintDialog();
            dialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
            dialog.PrintDocument(fixedDocument.DocumentPaginator, "Print");
            /* printdocumentimageablearea area = null;
             xpsdocumentwriter writer = printqueue.createxpsdocumentwriter(ref area);
             size size = new size(area.mediasizewidth, area.mediasizeheight);
             fixeddocument.documentpaginator.pagesize = size;
             writer.write(fixeddocument.documentpaginator);*/
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
    }
}