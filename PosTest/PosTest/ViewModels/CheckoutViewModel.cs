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

namespace PosTest.ViewModels
{
    public class CheckoutViewModel : Screen
    {
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
        }

        public CheckoutViewModel(int pageSize, IProductService productsService, 
            ICategoryService categoriesService,
            IOrderService orderService) : this()
        {
            _productsService = productsService;
            _categoriesService = categoriesService;
            _orderService = orderService;

            int getProductsStatusCode = 0,
                getCategoriesStatusCode = 0;
            AllRequestedProducts = _productsService.GetAllProducts(ref getProductsStatusCode);
            FilteredProducts = CollectionViewSource.GetDefaultView(AllRequestedProducts);
            MaxProductPageSize = pageSize;
            ProductsVisibility = true;
            AdditivesVisibility = false;
            CategorieFiltering("Home");
            Categories = new BindableCollection<Category>(_categoriesService.GetAllCategories(ref getCategoriesStatusCode));
            InitCategoryColors();
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
        public ICollection<Product> AllRequestedProducts { get; set; }

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
                NotifyOfPropertyChange();
            }
        }

        public BindableCollection<Category> Categories { get; set; }

        public BindableCollection<Order> Orders { get; set; }

        public Order CurrentOrder
        {
            get { return _currentOrder; }
            set
            {
                _currentOrder = value;            
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

        public INotifyPropertyChanged DialogViewModel
        {
            get => _dialogViewModel;
            set
            {
                _dialogViewModel = value;
                NotifyOfPropertyChange();
            }
        }

        
        #endregion


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
                    if (CurrentOrder.State == OrderState.Payed)
                    {
                        RemoveCurrentOrder();
                    }
                    else
                    {
                        SetNullCurrentOrder(); ;
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
            CurrentOrder = new Order();
            DisplayedOrder = CurrentOrder;
            Orders.Add(CurrentOrder);
            CurrentOrder.OrderTime = DateTime.Now;
        }

        public void SetNullCurrentOrder()
        {
            if (CurrentOrder != null)
            {
                CurrentOrder.SaveScreenState(CurrentCategory, AdditivesPage, ProductsVisibility, AdditivesVisibility);
            }
            CategorieFiltering("Home");
            CurrentOrder = null;
            DisplayedOrder = null;
        }

        public void RemoveCurrentOrder()
        {
            if (CurrentOrder==null || Orders==null)
            {
                return;
            }

            Orders.Remove(CurrentOrder);
            CurrentOrder = null;
        }

        public void CancelOrder()
        {
            if (CurrentOrder == null)
            {
                return;
            }
            if (CurrentOrder.OrderItems.Count == 0)
            {
                return;
            }
            CurrentOrder.State = OrderState.Canceled;
            SaveCurrentOrder();
            RemoveCurrentOrder();
            var i = Orders.IndexOf(CurrentOrder);

        }
        #endregion

        public void Close()
        {
            CategoryTabViewModel categoryTabViewModel = new CategoryTabViewModel(30, _productsService, _categoriesService);
            categoryTabViewModel.Parent = this.Parent;


            (this.Parent as Conductor<object>).ActivateItem(categoryTabViewModel);
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
                        var muchInDemanadProducts = AllRequestedProducts.Where(p => p.IsMuchInDemand == true).ToList();
                        FilteredProducts = CollectionViewSource.GetDefaultView(muchInDemanadProducts);
                        PaginateProducts(NextOrPrevious.First);
                    }
                    else // param == "ALL"
                    {
                        FilteredProducts = CollectionViewSource.GetDefaultView(AllRequestedProducts);
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
                cmd != ActionButton.Table &&
                cmd != ActionButton.Del)
            {
                ToastNotification.Notify("Enter the required vlue before ..");
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
                    TableAction();
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
            if (CurrentOrder == null)
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
            SaveCurrentOrder();
        }

        private void TableAction()
        {
            int tableNumber;
            if (string.IsNullOrEmpty(NumericZone))
            {
                IsDialogOpen = true;
                DialogViewModel = TablesViewModel ?? new TablesViewModel(this);
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
            if (tableNumber < 0)
            {
                NumericZone = "";
                return;
            }

            var status = 0;
            var table = _orderService.GetTableByNumber(tableNumber, ref status);
            switch (status)
            {
                case 200:
                    if (CurrentOrder == null)
                    {
                        NewOrder();
                    }
                    CurrentOrder.Table = table;
                    table.AddOrder(CurrentOrder);
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
                order.DiscountAmount = order.Total - newTotal;// CurrentOrder.NewTotal;
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
                discount = order.NewTotal * discountPercent / 100;
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

        public void RemoveOrerItem(OrderItem item)
        {
            CurrentOrder.RemoveOrderItem(item);
            AdditivesVisibility = false;
            ProductsVisibility = true;
        }

        public void AddOneToQuantity(OrderItem item)
        {
            item.Quantity += 1;
        }

        public void SubtractOneFromQuantity(OrderItem item)
        {
            if (item.Quantity <= 1)
                return;
            item.Quantity -= 1;
        }

        public void DiscountOnOrderItem(OrderItem item)
        {
            if (String.IsNullOrEmpty(NumericZone))
                return;
            var oldDiscount = item.DiscountAmount;
            var discountPercent = 0m;
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
                    discount = Convert.ToDecimal(NumericZone);
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
                if (IsRunningFromXUnit)
                {
                    throw new Exception("Discount Greater Than Price");
                }
                else
                {
                    ToastNotification.Notify("Discount Greater Than Price");
                    return;
                }
            }

            if (discountPercent >= 0)
            {
                item.DiscountPercentatge = discountPercent;

            }
            if (discount >=0)
            {
                item.DiscountAmount = discount;

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

        public void GoToAdditiveButtonsPage(OrderItem oitem)
        {
            AdditivesVisibility = true;
            ProductsVisibility = false;
            AdditivesPage = CollectionViewSource.GetDefaultView((oitem.Product as Platter).Additives);
        }


        #endregion

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
        Validate = 10
    }
}