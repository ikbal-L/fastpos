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
using System.Collections.Generic;

namespace PosTest.ViewModels
{
    public class CheckoutViewModel : Screen
    {
        
        private  ICollectionView _productsPage { get; set; }
        private int _pageNumber=0;
        private bool _canExecuteNext;
        private bool _canExecutePrevious;

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

        public Category CurrantCategory
        {
            get { return _currantCategory; }
            set
            {
                _currantCategory = value;
                NotifyOfPropertyChange(() => _currantCategory);
                //NotifyOfPropertyChange(() => CanSayHello);
            }
        }
        public int MaxProductPageSize { get; set; }

        //public BindableCollection<Product> ProductsPage { get; set; }

        public ICollectionView ProductsPage
        {
            get => _productsPage;// new BindableCollection<Product>(_productsPage.SourceCollection.Cast<Product>()); 
            set
            {
                //_productsPage = value;
                _productsPage = value;
                NotifyOfPropertyChange(() => ProductsPage);
            }
        }

        public BindableCollection<Category> Categories { get; set; }

        public BindableCollection<Order> Orders { get; set; }



        //BindableCollection<OrdreItem> currentOrderitem;
        /*public Order CurrentOrder
        {
            get;
            set;

        }*/

        private Order currentOrder;

        public Order CurrentOrder
        {
            get { return currentOrder; }
            set
            {
                currentOrder = value;            
                NotifyOfPropertyChange(() => currentOrder);
                //NotifyOfPropertyChange(() => currentOrderitem);
                //NotifyOfPropertyChange(() => CanSayHello);                
            }
        }

        public CheckoutViewModel()
        {
            //currentOrderitem = new BindableCollection<OrdreItem>();
            Orders = new BindableCollection<Order>();
            CurrentOrder = new Order();
            CurrentOrder.OrderItems = new BindableCollection<OrderItem>();
            Orders.Add(CurrentOrder);
            //CurrentOrder = Orders[0];
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

        public void CategorieFiltering(object param)
        {
  
            if(param is Category)
            { 
                Category category = param as Category;
                category.BackGroundColor = DefaultColors.Category_SelectedBackground;
                if(CurrantCategory != null)
                    CurrantCategory.BackGroundColor = DefaultColors.Category_DefaultBackground;
                CurrantCategory = category;
                FilteredProducts.Filter = CatFilter;
                PaginateProducts(NextOrPrevious.First);
                
                //Console.WriteLine(Products.Where(p => p.CategorieId == CurrantCategorie.Id).Count());
                //Console.WriteLine(Products.Count());
                //Products =  Products.Where(p => p.CategorieId == CurrantCategorie.Id).ToList();
                
                //Products = Products;
            }
            else  
                if(param is string)
                    if((param as string).ToUpperInvariant().Equals("Home".ToUpperInvariant()))
                    {
                        FilteredProducts.Filter = null;
                        if (CurrantCategory != null)
                            CurrantCategory.BackGroundColor = DefaultColors.Category_DefaultBackground;
                        CurrantCategory = null;
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

            CurrentOrder.AddItem(selectedproduct, selectedproduct.Price, 1);

            //Console.WriteLine(CurrentOrder.Items.Count);
            //currentOrderitem = new BindableCollection<OrdreItem>(CurrentOrder.Items);
            //Console.WriteLine(selectedproduct+"  |  "+ currentOrderitem);


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
