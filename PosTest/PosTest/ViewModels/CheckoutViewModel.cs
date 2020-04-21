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

namespace PosTest.ViewModels
{
    public class CheckoutViewModel : Screen
    {
        

        private  ICollectionView _productsPage { get; set; }
        private  ICollectionView _additvesPage { get; set; }
        private int _pageNumber=0;
        private bool _canExecuteNext;
        private bool _canExecutePrevious;
        private Order _currentOrder;
        private Visibility _additivesVisibility;

        public CheckoutViewModel(int pageSize, IProductService productService, ICategorieService categorieService)
        {
            //currentOrderitem = new BindableCollection<OrdreItem>();
            Orders = new BindableCollection<Order>();
            CurrentOrder = new Order();
            Orders.Add(CurrentOrder);
            AllRequestedProducts = productService.GetAllProducts();
            FilteredProducts = CollectionViewSource.GetDefaultView(AllRequestedProducts);
            MaxProductPageSize = pageSize;
            ProductsVisibility = Visibility.Visible;
            AdditivesVisibility = Visibility.Collapsed;
            CategorieFiltering("Home");
            Categories = new BindableCollection<Category>(categorieService.GetAllCategory());
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

        private Category _currantCategory;
        private Visibility _productsVisibility;
        private string _numericZone;

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
        public Visibility AdditivesVisibility 
        { 
            get => _additivesVisibility;
            set 
            {
                _additivesVisibility = value;
                NotifyOfPropertyChange(nameof(AdditivesVisibility));
            } 
        }

       public Visibility ProductsVisibility 
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
            ProductsVisibility = Visibility.Visible;
            AdditivesVisibility = Visibility.Collapsed;
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
                category.BackGroundColor = DefaultColors.Category_DefaultBackground;
        }

        public void Close()
        {
            LoginViewModel toActivateViewModel = new LoginViewModel();
            toActivateViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(toActivateViewModel);
        }

        public void AddAditive(Additive additive)
        {
            CurrentOrder.SelectedOrdernItem.AddAdditives(additive);
           // NotifyOfPropertyChange(nameof(CurrentOrdernItem));

        } 

        public void RemoveAdditive(Additive additive)
        {
            CurrentOrder.SelectedOrdernItem.RemoveAdditives(additive);
           // NotifyOfPropertyChange(nameof(CurrentOrdernItem));

        }

        public void DeleteOrerItem(OrderItem item)
        {
            CurrentOrder.DeleteOrderItem(item);
            AdditivesVisibility = Visibility.Collapsed;
            ProductsVisibility = Visibility.Visible;
        }

        public void AddAdditives(OrderItem oitem)
        {
            AdditivesVisibility = Visibility.Visible;
            ProductsVisibility = Visibility.Collapsed;
            AdditivesPage = AdditivesPage = CollectionViewSource.GetDefaultView((oitem.Product as Platter).Additives);
        }
        public void CategorieFiltering(object param)
        {
            AdditivesVisibility = Visibility.Collapsed;
            ProductsVisibility = Visibility.Visible;
            if(param is Category)
            { 
                Category category = param as Category;
                category.BackGroundColor = DefaultColors.Category_SelectedBackground;
                if(CurrentCategory != null)
                    CurrentCategory.BackGroundColor = DefaultColors.Category_DefaultBackground;
                CurrentCategory = category;
                FilteredProducts.Filter = CatFilter;
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
                            CurrentCategory.BackGroundColor = DefaultColors.Category_DefaultBackground;

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

            
            CurrentOrder.AddItem(selectedproduct, selectedproduct.Price, true, 1);

            if (selectedproduct is Platter && (selectedproduct as Platter).Additives != null)
            {
                AdditivesPage = CollectionViewSource.GetDefaultView((selectedproduct as Platter).Additives);
                ProductsVisibility = Visibility.Collapsed;
                AdditivesVisibility = Visibility.Visible;
            }

            //Console.WriteLine(CurrentOrder.Items.Count);
            //currentOrderitem = new BindableCollection<OrdreItem>(CurrentOrder.Items);
            //Console.WriteLine(selectedproduct+"  |  "+ currentOrderitem);


        }

        public void NumericKeyboard(string number)
        {
            if (NumericZone == null)
                NumericZone = String.Empty;
            NumericZone += number ;
        }

        public void ActionKeyboard(string cmd)
        {
            if (String.IsNullOrEmpty(NumericZone))
                return;
            switch (cmd)
            {
                case "Del":
                    NumericZone = String.IsNullOrEmpty(NumericZone) ? String.Empty : NumericZone.Remove(NumericZone.Length-1); break;
                case ".":
                    NumericZone = NumericZone.Contains(".") ? NumericZone : NumericZone + "."; break;
                case "Qty": NumericZone = ""; break;
            }
        }
        private bool CatFilter(object item)
        {
            Product p = item as Product;
            return p.CategorieId.Equals(_currantCategory.Id);
        }

    }

    public enum NextOrPrevious
    {
        Next,
        Previous,
        First
    }

}
