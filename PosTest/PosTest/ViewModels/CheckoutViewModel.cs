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
        private static readonly bool IsRunningFromXUnit =
                   AppDomain.CurrentDomain.GetAssemblies().Any(
                       a => a.FullName.StartsWith("XUnitTesting"));

        private IProductService _productsService;
        private ICategorieService _categoriesService;
        private Category _currantCategory;
        private bool _productsVisibility;
        private string _numericZone;

        private  ICollectionView _productsPage { get; set; }
        private  ICollectionView _additvesPage { get; set; }
        private int _pageNumber=0;
        private bool _canExecuteNext;
        private bool _canExecutePrevious;
        private Order _currentOrder;
        private bool _additivesVisibility;

        public CheckoutViewModel() 
        {
            Orders = new BindableCollection<Order>();
            CurrentOrder = new Order();
            Orders.Add(CurrentOrder);
        }

        public CheckoutViewModel(int pageSize, IProductService productsService, ICategorieService categoriesService) : this()
        {
            _productsService = productsService;
            _categoriesService = categoriesService;
            //currentOrderitem = new BindableCollection<OrdreItem>();
            
            AllRequestedProducts = productsService.GetAllProducts();
            FilteredProducts = CollectionViewSource.GetDefaultView(AllRequestedProducts);
            MaxProductPageSize = pageSize;
            ProductsVisibility = true;
            AdditivesVisibility = false;
            CategorieFiltering("Home");
            Categories = new BindableCollection<Category>(categoriesService.GetAllCategory());
            InitCategoryColors();
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
                //NotifyOfPropertyChange(() => CanSayHello);
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

        //public BindableCollection<Product> ProductsPage { get; set; }

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


        public BindableCollection<Category> Categories { get; set; }

        public BindableCollection<Order> Orders { get; set; }

        public Order CurrentOrder
        {
            get { return _currentOrder; }
            set
            {
                _currentOrder = value;            
                NotifyOfPropertyChange(() => _currentOrder);
                               
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

        public void ReturnFromAdditives()
        {
            ProductsVisibility = true;
            AdditivesVisibility = false;
        }
         public void PaginateProducts(NextOrPrevious nextOrprevious)
        {          
            if(nextOrprevious == NextOrPrevious.First)
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

            CanExecutePrevious = _pageNumber==1 ? false : true;
        }
        internal void InitCategoryColors()
        {
            foreach (var category in Categories)
                category.BackgroundString =  DefaultColors.Category_DefaultBackground.ToString();
        }

        public void Close()
        {
            CategoryTabViewModel categoryTabViewModel = new CategoryTabViewModel(30, _productsService, _categoriesService);
            categoryTabViewModel.Parent = this.Parent;


            (this.Parent as Conductor<object>).ActivateItem(categoryTabViewModel);

            
            /*LoginViewModel toActivateViewModel = new LoginViewModel();
            toActivateViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(toActivateViewModel);*/
        }

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
            CurrentOrder.SelectedOrderItem.RemoveAdditive(additive);
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
                } //discount = Convert.ToDecimal(NumericZone);
            }
                //decimal discount =0;
            

            NumericZone = string.Empty;
            if (item.Total < discount)
            {
                if (IsRunningFromXUnit)
                {
                    throw new Exception("Discount Greater Than Price");
                }
                else
                {
                    ToastNotification("Discount Greater Than Price");
                    return;
                }
            }

            if (discountPercent>0)
            {
                item.DiscountPercentatge = discountPercent;

            }
            if (discount>0)
            {
                item.DiscountAmount = discount;

            }
            //CurrentOrder.NewTotal = CurrentOrder.NewTotal + oldDiscount - discount;
        }
        
        public void GoToAdditiveButtonsPage(OrderItem oitem)
        {
            AdditivesVisibility = true;
            ProductsVisibility = false;
            AdditivesPage = CollectionViewSource.GetDefaultView((oitem.Product as Platter).Additives);
        }
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
                
                //Console.WriteLine(Products.Where(p => p.CategorieId == CurrantCategorie.Id).Count());
                //Console.WriteLine(Products.Count());
                //Products =  Products.Where(p => p.CategorieId == CurrantCategorie.Id).ToList();
                
                //Products = Products;
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

        public void AddOrderItem(Product selectedproduct)
        {
            //Product selectedproduct = (Product)sender;
            //Console.WriteLine();
            if (selectedproduct == null)
                return;
            
            CurrentOrder.AddItem(product: selectedproduct, unitPrice: selectedproduct.Price, setSelected: true, quantity: 1);

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

        public void NumericKeyboard(string number)
        {
            if (String.IsNullOrEmpty(number))
                return;
            if (number.Length > 1)
                return;
            var numbers = new string[]{ "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "." , "%"};
            if ( !numbers.Contains(number) )
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
                        ToastNotification("Invalid value for Percentagte");
                    }
                }
                else
                {
                    NumericZone = _numericZone.Remove(_numericZone.Length - 1, 1) + number + "%";

                }
                return;
            }

            NumericZone += number ;
        }

        public void ActionKeyboard(ActionButton cmd)
        {
            if (String.IsNullOrEmpty(NumericZone))
                return;
            switch (cmd)
            {
                case ActionButton.Del:
                    NumericZone = String.IsNullOrEmpty(NumericZone) ? String.Empty : NumericZone.Remove(NumericZone.Length-1); 
                    break;

                case ActionButton.Qty:
                    int qty;
                    try
                    {
                        qty = Convert.ToInt32(NumericZone);
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
                    if (CurrentOrder.SelectedOrderItem != null)
                        CurrentOrder.SelectedOrderItem.Quantity = qty;
                    NumericZone = ""; 
                    break;

                case ActionButton.Price:
                    decimal price;
                    try
                    {
                        price = Convert.ToDecimal(NumericZone);
                    }
                    catch (Exception)
                    {
                        NumericZone = "";
                        return;
                    }
                    if (price < 0)
                    {
                        NumericZone = "";
                        return;
                    }
                    //CurrentOrder.NewTotal
                    var newTotal = price;
                    if (newTotal <= CurrentOrder.Total)
                    {
                        CurrentOrder.DiscountAmount = CurrentOrder.Total - newTotal;// CurrentOrder.NewTotal;
                    }
                    else
                    {
                        NumericZone = string.Empty;
                        ToastNotification("New price less than the total price");
                    }
                    NumericZone = "";
                    break;

                case ActionButton.Disc:
                    var discountPercent = 0m;
                    var discount = 0m;
                    var isPercentage = false;
                    if (NumericZone.Contains("%"))
                    {
                        isPercentage = true;
                        discountPercent = Convert.ToDecimal(NumericZone.Remove(NumericZone.Length-1));
                        if (discountPercent>100)
                        {
                            NumericZone = string.Empty;
                            return;
                        }
                        discount = CurrentOrder.NewTotal * discountPercent / 100;
                    }
                    else
                    {
                        discount = Convert.ToDecimal(NumericZone);
                    }
                    if (discount < 0)
                    {
                        NumericZone = "";
                        return;
                    }

                                       
                    if (discount > CurrentOrder.Total) 
                    {
                        //Use Local to select message according to UI language
                        NumericZone = "";
                        ToastNotification("Discount bigger than total");
                        //CurrentOrder.DiscountAmount = 0;
                        return;
                    }
                    if (!isPercentage)
                    {
                        CurrentOrder.DiscountAmount = discount;

                    }
                    else
                    {
                        CurrentOrder.DiscountPercentage = discountPercent;

                    }
                    //CurrentOrder.NewTotal = CurrentOrder.Total - CurrentOrder.DiscountAmount;
                    NumericZone = ""; 
                    break;

                case ActionButton.Payment:
                    var payedAmount = Convert.ToDecimal(NumericZone);
                    if (payedAmount < 0)
                    {
                        NumericZone = "";
                        return;
                    }
                                      
                    if (payedAmount < CurrentOrder.NewTotal) 
                    {
                        NumericZone = "";
                        //Use Local to select message according to UI language
                        ToastNotification("Payed amount lower than total");
                        //CurrentOrder.DiscountAmount = 0;
                        return;
                    }

                    CurrentOrder.GivenAmount = payedAmount;
                    CurrentOrder.ReturnedAmount = CurrentOrder.NewTotal - payedAmount;
                    NumericZone = "";
                    SaveCurrentOrderAndPassOrderToKitchen();
                    break;
            }
        }

        private void SaveCurrentOrderAndPassOrderToKitchen()
        {

        }

        private void ToastNotification(string message)
        {
            Notifier notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomLeft,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
            if (!IsRunningFromXUnit)
            {
                notifier.ShowError(message);
            }
            else
            {
                throw new Exception(message);
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
        Del,
        Qty,
        Price,
        Disc,
        Payment
    }

}
