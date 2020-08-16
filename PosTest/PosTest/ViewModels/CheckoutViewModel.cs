using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServiceInterface.Model;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Media;
using ServiceInterface.StaticValues;
using System.Windows;
using System.ComponentModel.Composition;
using ServiceInterface.Interface;
using ToastNotifications;
using ToastNotifications.Position;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using PosTest.ViewModels.SubViewModel;
using System.Threading;

namespace PosTest.ViewModels
{
    public class CheckoutViewModel : Screen
    {
        public void OrderSelectionChanged(Order order)
        {

        }
        #region private fields
        private static readonly bool IsRunningFromXUnit =
                   AppDomain.CurrentDomain.GetAssemblies().Any(
                       a => a.FullName.StartsWith("XUnitTesting"));
        [Import(typeof(IProductService))]
        private IProductService _productsService;
        [Import(typeof(ICategoryService))]
        private ICategoryService _categoriesService;
        [Import(typeof(IOrderService))]
        private IOrderService _orderService;
        private Category _currantCategory;
        private bool _productsVisibility;
        private string _numericZone;
        private int _pageNumber=0;
        private bool _canExecuteNext;
        private bool _canExecutePrevious;
        private Order _currentOrder;
        private bool _additivesVisibility;
        private ICollectionView _productsPage;
        private ICollectionView _additvesPage;

        private bool _IsDialogOpen;
        private SplitViewModel _splitViewModel;
        private Order _displayedOrder;
        private TablesViewModel _tablesViewModel;
        private INotifyPropertyChanged _dialogViewModel;
        private decimal givenAmount;
        private decimal _returnedAmount;
        private BindableCollection<Table> _tables;
        private Table _selectedTable;
        private INotifyPropertyChanged _listedOrdersViewModel;
        private WaitingViewModel _waitingViewModel;
        private DelivereyViewModel _delivereyViewModel;
        private TakeawayViewModel _takeAwayViewModel;
        #endregion

        #region Constructors
        public CheckoutViewModel() 
        {
            Orders = new BindableCollection<Order>();
            if (IsRunningFromXUnit)
            {
                CurrentOrder = new Order();
                Orders.Add(CurrentOrder);
            }
            TakeAwayViewModel = new TakeawayViewModel(this);
            DelivereyViewModel = new DelivereyViewModel(this);
            WaitingViewModel = new WaitingViewModel(this);
            Task.Run(CalculateOrderElapsedTime);
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

                            o.ElapsedTime = lastStateTime!=null? DateTime.Now - lastStateTime.StateTime : DateTime.Now- o.OrderTime;
                        }
                    }
                    catch (Exception)
                    {
                        
                    }                    
                }
                Thread.Sleep(30000);
            }
        }
        public CheckoutViewModel(int pageSize, IProductService productsService, 
            ICategoryService categoriesService,
            IOrderService orderService) : this()
        {
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

            _productsService = productsService;
            _categoriesService = categoriesService;
            _orderService = orderService;

            int getProductsStatusCode = 0,
                getCategoriesStatusCode = 0;
            AllProducts = _productsService.GetAllProducts(ref getProductsStatusCode);
            FilteredProducts = CollectionViewSource.GetDefaultView(AllProducts);
            MaxProductPageSize = pageSize;
            ProductsVisibility = true;
            AdditivesVisibility = false;
            CategorieFiltering("Home");
            Categories = new BindableCollection<Category>(RetrieveCategories(AllProducts)); //(_categoriesService.GetAllCategories(ref getCategoriesStatusCode));
            var status = 0;
            var tables = _orderService.GeAlltTables(ref status);
            Tables = new BindableCollection<Table>(tables);
            foreach (var table in Tables)
            {
                table.AllOrders = Orders;
                table.AllTables = Tables;
            }
            TablesViewModel = new TablesViewModel(this);
            //InitCategoryColors();
        }
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
            get { return _currantCategory; }
            set
            {
                _currantCategory = value;
                NotifyOfPropertyChange(() => CurrentCategory);
            }
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

        public ICollectionView ProductsPage
        {
            get => _productsPage;
            set
            {
                //_productsPage = value;
                _productsPage = value;
                NotifyOfPropertyChange(() => ProductsPage);
            }
        }

       public ICollectionView AdditivesPage
        {
            get => _additvesPage;
            set
            {
                //_productsPage = value;
                _additvesPage = value;
                NotifyOfPropertyChange(() => AdditivesPage);
            }
        }

        public Order DisplayedOrder
        {
            get => _displayedOrder;
            set
            {
                _displayedOrder = value;
                
                //SetSelectedInListedOrdersDisplayedOrder();
                NotifyOfPropertyChange();
            }
        }

        private void SetSelectedInListedOrdersDisplayedOrder()
        {
            var table = Tables.Where(t => t.Orders.Contains(DisplayedOrder)).FirstOrDefault();
            if(table != null)
            {
                table.SelectedOrder = DisplayedOrder;
                TablesViewModel.SelectedTable = table;
            }
            
            foreach (var t in TablesViewModel.Tables.Where(tb => !tb.Orders.Contains(DisplayedOrder)))
            {
                t.SelectedOrder = null;
                if (TablesViewModel.SelectedTable == t)
                {
                    TablesViewModel.SelectedTable = null;
                }
            }

            DelivereyViewModel.SelectedOrder = DelivereyViewModel.Orders.Cast<Order>().Where(o => o == DisplayedOrder).FirstOrDefault();

            TakeAwayViewModel.SelectedOrder = TakeAwayViewModel.Orders.Cast<Order>().Where(o => o == DisplayedOrder).FirstOrDefault();
            
            WaitingViewModel.SelectedOrder= WaitingViewModel.Orders.Cast<Order>().Where(o => o == DisplayedOrder).FirstOrDefault();
            
        }

        public BindableCollection<Category> Categories { get; set; }

        public BindableCollection<Order> Orders { get; set; }

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
                    _displayedOrder = value;
                }
                SetSelectedInListedOrdersDisplayedOrder();
                NotifyOfPropertyChange(() => CurrentOrder);
                               
            }
        }

        public string NumericZone 
        { 
            get => _numericZone;
            set
            {
                _numericZone = value;
                NotifyOfPropertyChange(() => NumericZone);
            } 
        }

        //public SplitViewModel SplitViewModel
        //{
        //    get => _splitViewModel;
        //    set
        //    {
        //        _splitViewModel = value;
        //        NotifyOfPropertyChange(() => SplitViewModel);
        //    }
        //}

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
                TableAction(value.Number);
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

        public INotifyPropertyChanged ListedOrdersViewModel
        {
            get => _listedOrdersViewModel;
            set
            {
                _listedOrdersViewModel = value;
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


        public decimal ReturnedAmount
        {
            get => _returnedAmount;
            set 
            { 
                _returnedAmount = value; 
                NotifyOfPropertyChange(() => ReturnedAmount); 
            }
        }


        internal void InitCategoryColors()
        {
            /*foreach (var category in Categories)
                category.BackgroundString =  DefaultColors.Category_DefaultBackground.ToString();*/
        }

        #region Order Commands
        public int? SaveOrder(Order order)
        {
            if (IsRunningFromXUnit)
            {
                return null;
            }
            int resp;
            try
            {
                var status = 0;
                if (order.Id == null)
                {
                    order.Id = _orderService.GetIdmax(ref status) + 1;
                    resp = _orderService.SaveOrder(order);
                    if (resp != 200)
                    {
                        order.Id = null;
                    }
                }
                else
                {
                    resp = _orderService.UpdateOrder(order);
                }
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
            var resp = SaveOrder(CurrentOrder);
            switch (resp)
            {
                case 200:
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

                case null: break;

                default:
                    ToastNotification.ErrorNotification((int)resp);
                    break;
            }

        }
        public void ShowOrder(Order order)
        {
            if (order==null)
            {
                return;
            }
            if (CurrentOrder != null)
            {
                CurrentOrder.SaveScreenState(CurrentCategory, AdditivesPage, ProductsVisibility, AdditivesVisibility);
            }
            CurrentOrder = order;
            DisplayedOrder = order;
            CurrentCategory = CurrentOrder.ShownCategory;
            if (_currantCategory == null)
            {
                CategorieFiltering("Home");
            }
            else
            {
                CategorieFiltering(CurrentCategory);
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
            CategorieFiltering("Home");
            CurrentOrder = new Order(this.Orders);
            DisplayedOrder = CurrentOrder;
            Orders.Add(CurrentOrder);
            //CurrentOrder.OrderTime = DateTime.Now;
            SetCurrentOrderTypeAndRefreshOrdersLists(OrderType.InWaiting);
            GivenAmount = 0;
            ReturnedAmount = 0;
        }

        FilterEventHandler TableOrdersFilter;
        private void SetCurrentOrderTypeAndRefreshOrdersLists(OrderType? orderType, Table table=null)
        {

            if (CurrentOrder != null)
            {
                CurrentOrder.Type = orderType;
                if (orderType == OrderType.OnTable)
                {
                    CurrentOrder.Table = table;
                }
                else
                {
                    CurrentOrder.Table = null;
                }
            }
            
            DelivereyViewModel.OrderViewSource.Filter -= DelivereyViewModel.OrderTypeFilter;
            DelivereyViewModel.OrderViewSource.Filter += DelivereyViewModel.OrderTypeFilter;
            WaitingViewModel.OrderViewSource.Filter -= WaitingViewModel.OrderTypeFilter;
            WaitingViewModel.OrderViewSource.Filter += WaitingViewModel.OrderTypeFilter;
            TakeAwayViewModel.OrderViewSource.Filter -= TakeAwayViewModel.OrderTypeFilter;
            TakeAwayViewModel.OrderViewSource.Filter += TakeAwayViewModel.OrderTypeFilter;

            DelivereyViewModel.NotifyOfPropertyChange(() => DelivereyViewModel.OrderCount);
            WaitingViewModel.NotifyOfPropertyChange(() => WaitingViewModel.OrderCount);
            TakeAwayViewModel.NotifyOfPropertyChange(() => TakeAwayViewModel.OrderCount);
            
            foreach (var t in Tables)
            {
                TableOrdersFilter = (s, e) =>
                {
                    Order order = e.Item as Order;
                    if (order != null)
                    {
                        if (order.Table == t)
                        {
                            e.Accepted = true;
                        }
                        else
                        {
                            e.Accepted = false;
                        }
                    }
                };
                t.OrderViewSource.Filter -= TableOrdersFilter;
                t.OrderViewSource.Filter += TableOrdersFilter;
            }
            TablesViewModel.TablesViewSource.Filter -= TablesViewModel.TablesFilter;
            TablesViewModel.TablesViewSource.Filter += TablesViewModel.TablesFilter;
            //TablesViewModel.RefreshTables();
            SetSelectedInListedOrdersDisplayedOrder();
            TablesViewModel.NotifyOfPropertyChange(() => TablesViewModel.OrderCount);
        }

        public void NewTotalToNumericZone()
        {
            if (CurrentOrder == null || CurrentOrder.NewTotal==0)
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
            CategorieFiltering("Home");
            _currentOrder = null;
            DisplayedOrder = null;
            SetSelectedInListedOrdersDisplayedOrder();
        }

        public void RemoveCurrentOrderForOrdersList()
        {
            if (CurrentOrder==null || Orders==null)
            {
                return;
            }
            Orders.Remove(CurrentOrder);

            CurrentOrder = null;
            SetCurrentOrderTypeAndRefreshOrdersLists(null);
            //TablesViewModel.TablesViewSource.Filter += TablesViewModel.TablesFilter;
            //saved in the DB as removed or canceled and remove it for Orders List
        }

        public void CancelOrder()
        {
            if (CurrentOrder == null)
            {
                return;
            }
            /*if (CurrentOrder.OrderItems.Count == 0)
            {
                RemoveCurrentOrder();
                return;
            }*/
            if (CurrentOrder.State == null)
            {
                CurrentOrder.State = OrderState.Removed;
            }
            else
            {
                CurrentOrder.State = OrderState.Canceled;
            }
            SaveCurrentOrder();
            //RemoveCurrentOrder();
            //var i = Orders.IndexOf(CurrentOrder);

        }
        #endregion

        public void CloseCommand()
        {
            LoginViewModel loginvm = new LoginViewModel();
            loginvm.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(loginvm);
        }

        public void SelectListedOrders(ListedOrdersType type)
        {
            switch (type)
            {
                case ListedOrdersType.Takeaway:
                    ListedOrdersViewModel = TakeAwayViewModel;
                    break;
                case ListedOrdersType.Delivery:
                    ListedOrdersViewModel = DelivereyViewModel;
                    break;
                case ListedOrdersType.Tables:
                    TablesViewModel.IsFullView = false;
                    ListedOrdersViewModel = TablesViewModel;
                    break;
                case ListedOrdersType.Waiting:
                    ListedOrdersViewModel = WaitingViewModel;
                    break;
                default:
                    break;
            }
        }

        #region Filtering and pagination
        public void CategorieFiltering(object param)
        {
            AdditivesVisibility = false;
            ProductsVisibility = true;
            if(param is Category)
            { 
                Category category = param as Category;
                category.BackgroundString = DefaultColors.Category_SelectedBackground.ToString();
                if(CurrentCategory != null)
                    CurrentCategory.BackgroundString = DefaultColors.Category_DefaultBackground.ToString();
                CurrentCategory = category;
                FilteredProducts.Filter = (p) => (p as Product).CategorieId.Equals(_currantCategory.Id);

                PaginateProducts(NextOrPrevious.First);
            }
            else  
                if(param is string)
                    if( (param as string).ToUpperInvariant().Equals("Home".ToUpperInvariant()))
                    {
                        FilteredProducts.Filter = null;

                        if (CurrentCategory != null)
                            CurrentCategory.BackgroundString = DefaultColors.Category_DefaultBackground.ToString();

                        CurrentCategory = null;
                        var muchInDemanadProducts = AllProducts.Where(p => p.IsMuchInDemand == true).ToList();
                        FilteredProducts = CollectionViewSource.GetDefaultView(muchInDemanadProducts);
                        PaginateProducts(NextOrPrevious.First);
                    }
                    else // param == "ALL"
                    {
                        FilteredProducts = CollectionViewSource.GetDefaultView(AllProducts);
                        PaginateProducts(NextOrPrevious.First);
                    }
        }

        public void ProductFiltering(string text)
        {
            Console.WriteLine("Param=" + text);
        }

        public void PaginateProducts(NextOrPrevious nextOrprevious)
        {
            if (nextOrprevious == NextOrPrevious.First)
            {
                _pageNumber = 0;
                nextOrprevious = NextOrPrevious.Next;
                CanExecuteMext = false;
                CanExecutePrevious = false;
            }

            if (FilteredProducts.Cast<Product>().Count() <= MaxProductPageSize)
            {
                ProductsPage = FilteredProducts;
                _pageNumber = 1;
                CanExecuteMext = false;
                CanExecutePrevious = false;
                return;
            }

            var listProducts = FilteredProducts.Cast<Product>().ToList();
            int lastIndexForThisPage = (_pageNumber + 1) * MaxProductPageSize;
            if (nextOrprevious == NextOrPrevious.Next)
            {
                if (listProducts.Count > lastIndexForThisPage)
                {
                    listProducts = listProducts.GetRange(_pageNumber * MaxProductPageSize, MaxProductPageSize);
                    CanExecuteMext = true;
                }
                else
                {
                    listProducts = listProducts.GetRange(_pageNumber * MaxProductPageSize, listProducts.Count - (_pageNumber * MaxProductPageSize));
                    CanExecuteMext = false;
                }
                _pageNumber++;
            }
            else
            {
                if ((_pageNumber - 2) * MaxProductPageSize < 0)
                    return;
                listProducts = listProducts.GetRange((_pageNumber - 2) * MaxProductPageSize, MaxProductPageSize);
                _pageNumber--;
                CanExecuteMext = true;

            }
            ProductsPage = CollectionViewSource.GetDefaultView(listProducts);

            CanExecutePrevious = _pageNumber == 1 ? false : true;
        }
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
                    NumericZone = String.IsNullOrEmpty(NumericZone) ? String.Empty : NumericZone.Remove(NumericZone.Length-1); 
                    break;

                case ActionButton.Qty:
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
                    PayementAction();
                    break;

                case ActionButton.Cmd:
                    if (CurrentOrder == null || CurrentOrder.OrderItems == null || CurrentOrder.OrderItems.Count == 0)
                    {
                        ToastNotification.Notify("Add products before ...");
                        return;
                    }
                    CurrentOrder.State = OrderState.Ordered;
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
                        IsDialogOpen = true;
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
                           (CurrentOrder.OrderItems.Count==1 && CurrentOrder.OrderItems[0].Quantity>1));
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
                    SetCurrentOrderTypeAndRefreshOrdersLists(OrderType.Takeaway);
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
                CurrentOrder.OrderItems.Count <1)
            {
                ToastNotification.Notify("Add products before ...");
                return;
            }

            if (payedAmount < CurrentOrder.NewTotal)
            {
                //NumericZone = "";
                //Use Local to select message according to UI language
                ToastNotification.Notify("Payed amount lower than total");
                //CurrentOrder.DiscountAmount = 0;
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
            if((table = LookForTableInOrders(tableNumber))==null)
                table = _orderService.GetTableByNumber(tableNumber, ref status);
            switch (status)
            {
                case 200:
                    if (CurrentOrder == null)
                    {
                        NewOrder();
                    }
                    SetCurrentOrderTypeAndRefreshOrdersLists(OrderType.OnTable, table);
                    //case of table transfer
                    //if (CurrentOrder.Table!= null && TablesViewModel != null)
                    //{
                    //    var count = CurrentOrder.Table.RemoveOrder(CurrentOrder);
                    //    if (count == 0)
                    //    {
                    //        TablesViewModel.RemoveTable(CurrentOrder.Table);
                    //    }
                    //}
                    //CurrentOrder.Table = table;
                    //table.AddOrder(CurrentOrder);
                    //if (TablesViewModel == null)
                    //{
                    //    TablesViewModel = new TablesViewModel(this);
                    //}
                    //TablesViewModel.Add(table);
                    _selectedTable = table;
                    NotifyOfPropertyChange(() => SelectedTable);

                    break;
                case 400:
                    if (ActionConfig.AllowUsingVirtualTable)
                    {
                        var status2 = _orderService.SaveTable(new Table { IsVirtual = true, Number = tableNumber });
                        if (status2 == 200)
                        {
                            ToastNotification.Notify("Creation of virtual table", 1);
                            NewOrder();
                        }
                        else
                        {
                            ToastNotification.ErrorNotification(status2);
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
            /*foreach (var o in Orders)
            {
                if (o.Table != null && o.Table.Number == tableNumber)
                {
                    return o.Table;
                } 
            }
*/          return null;
        }

        private void PriceAction(ref string priceStr, Order order)
        {
            decimal price;
            if (priceStr=="")
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
            //CurrentOrder.NewTotal
            var newTotal = price;
            if (newTotal <= order.Total)
            {
                var sumItemDiscounts = 0m;
                order.OrderItems.ToList().ForEach(item => sumItemDiscounts += item.DiscountAmount);
                order.DiscountAmount = order.Total - newTotal - sumItemDiscounts;// CurrentOrder.NewTotal;
                if (order.DiscountAmount<0 && order.Total > newTotal)
                {
                    order.OrderItems.ToList().ForEach(item => item.DiscountAmount=0);
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
            //CurrentOrder.NewTotal = CurrentOrder.Total - CurrentOrder.DiscountAmount;
            discStr = "";
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
                                    CurrentOrder.SelectedOrderItem.Additives.
                                        Where(addv => addv.Equals(additive)).
                                        FirstOrDefault();
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
                additive.ParentOrderItem.Additives.Remove(additive);
            }
        }

        public void RemoveOrerItem()
        {
            if (CurrentOrder.SelectedOrderItem == null)
            {
                return;
            }
            
             CurrentOrder.RemoveOrderItem(CurrentOrder.SelectedOrderItem);

            AdditivesVisibility = false;
            ProductsVisibility = true;
        }

        public void AddOneToQuantity()
        {
            if (CurrentOrder.SelectedOrderItem == null)
            {
                return;
            }
            CurrentOrder.SelectedOrderItem.Quantity += 1;
        }

        public void SubtractOneFromQuantity()
        {
            if (CurrentOrder.SelectedOrderItem == null)
            {
                return;
            }

            var item = CurrentOrder.SelectedOrderItem;
            if (item.Quantity <= 1)
                return;
            item.Quantity -= 1;
        }

        public void DiscountOnOrderItem()
        {
            if (String.IsNullOrEmpty(NumericZone))
                return;

            if (CurrentOrder.SelectedOrderItem == null)
            {
                return;
            }

            var item = CurrentOrder.SelectedOrderItem;
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
                item.DiscountPercentatge = discountPercent;
            }
            if (discountAmount >= 0)
            {
                item.DiscountAmount = discountAmount;
            }
        }

        public void AddOrderItem(Product selectedproduct)
        {
            //Product selectedproduct = (Product)sender;
            //Console.WriteLine();
            if (selectedproduct == null)
                return;
            
            if (CurrentOrder == null)
            {
                NewOrder();
                /*CurrentOrder = new Order();
                Orders.Add(CurrentOrder); 
                CurrentOrder.OrderTime = DateTime.Now;*/
            }

            CurrentOrder.AddOrderItem(product: selectedproduct, unitPrice: selectedproduct.Price, setSelected: true, quantity: 1);

            if (selectedproduct is Platter && (selectedproduct as Platter).Additives != null)
            {
                AdditivesPage = CollectionViewSource.GetDefaultView((selectedproduct as Platter).Additives);
                ProductsVisibility = false;
                AdditivesVisibility = true;
            }

            //Console.WriteLine(CurrentOrder.Items.Count);
            //currentOrderitem = new BindableCollection<OrdreItem>(CurrentOrder.Items);
            //Console.WriteLine(selectedproduct+"  |  "+ currentOrderitem);


        }

        public void ReturnFromAdditives()
        {
            ProductsVisibility = true;
            AdditivesVisibility = false;
        }

        public void GoToAdditiveButtonsPage()
        {
            if (CurrentOrder.SelectedOrderItem== null)
            {
                return;
            }
            var oitem = CurrentOrder.SelectedOrderItem;
            AdditivesVisibility = true;
            ProductsVisibility = false;
            AdditivesPage = CollectionViewSource.GetDefaultView((oitem.Product as Platter).Additives);
        }


        #endregion

        private bool _IsTopDrawerOpen;
        public bool IsTopDrawerOpen {
            get => _IsTopDrawerOpen;
            set => Set(ref _IsTopDrawerOpen, value);
        }

        public void ShowTablesCommand()
        {
            IsTopDrawerOpen = true;
        }



        }


    static class ToastNotification
    {
        // https://github.com/rafallopatka/ToastNotifications
        private static readonly bool IsRunningFromXUnit =
                  AppDomain.CurrentDomain.GetAssemblies().Any(
                      a => a.FullName.StartsWith("XUnitTesting"));
        private static Notifier _notifier; 
        private static Notifier Instance 
        {
            
            get => _notifier ?? 
                (_notifier = new Notifier(cfg =>
                    {
                        cfg.PositionProvider = new WindowPositionProvider(
                            parentWindow: Application.Current.MainWindow,
                            corner: Corner.BottomLeft,
                            offsetX: 10,
                            offsetY: 10);

                        cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                            notificationLifetime: TimeSpan.FromSeconds(1.5),
                            maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                        cfg.Dispatcher = Application.Current.Dispatcher;
                    }));
        }
        public static void Notify(string message, int type=-1)
        {
            Notifier notifier = Instance;
            //    new Notifier(cfg =>
            //{
            //    cfg.PositionProvider = new WindowPositionProvider(
            //        parentWindow: Application.Current.MainWindow,
            //        corner: Corner.BottomLeft,
            //        offsetX: 10,
            //        offsetY: 10);

            //    cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
            //        notificationLifetime: TimeSpan.FromSeconds(3),
            //        maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            //    cfg.Dispatcher = Application.Current.Dispatcher;
            //});
            if (!IsRunningFromXUnit)
            {
                if (type == -1)
                {
                    notifier.ShowError(message);
                }
                else if (type == 1)
                {
                    notifier.ShowInformation(message);
                }
            }
            else
            {
                if (type == -1)
                {
                    throw new Exception(message);
                }
            }
        }

        public static void ErrorNotification(int respStatusCode)
        {
            switch (respStatusCode)
            {
                case 401: ToastNotification.Notify("Database insertion error " + respStatusCode.ToString()); break;
                case 402: ToastNotification.Notify("Id exists in the database " + respStatusCode.ToString()); break;
                case 403: ToastNotification.Notify("Database access error " + respStatusCode.ToString()); break;
                case 404: ToastNotification.Notify("Bad request " + respStatusCode.ToString()); break;
                case 405: ToastNotification.Notify("Authentification error " + respStatusCode.ToString()); break;
                case 406: ToastNotification.Notify("Authentification error " + respStatusCode.ToString()); break;
                case 407: ToastNotification.Notify("Authentification error " + respStatusCode.ToString()); break;
                case 408: ToastNotification.Notify("Database error in Annex_id " + respStatusCode.ToString()); break;
                case 409: ToastNotification.Notify("Config Database error " + respStatusCode.ToString()); break;
                case 410: ToastNotification.Notify(" User or password error" + respStatusCode.ToString()); break;
            }
        }

    }

    public enum NextOrPrevious
    {
        Next,
        Previous,
        First
    }

    public enum ActionButton
    {
        Del=0,
        Qty=1,
        Price=2,
        Disc=3,
        Cmd=4,
        Payment=5,
        Split=6,
        Table=7,
        SplittedPayment=8,
        Itemprice = 9,
        Validate = 10,
        Deliverey = 11,
        Takeaway = 12,
        Served = 13
    }

    public enum ListedOrdersType
    {
        Takeaway,
        Delivery,
        Tables,
        Waiting
    }
}