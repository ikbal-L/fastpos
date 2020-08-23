using Caliburn.Micro;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;

namespace PosTest.ViewModels
{
    public class CheckoutSettingsViewModel : Screen
    {
        //[Import(typeof(IProductService))]
        private IProductService _productsService;
        //[Import(typeof(ICategoryService))]
        private ICategoryService _categoriesService;
        //[Import(typeof(IOrderService))]
        private IOrderService _orderService;
 
        private bool _IsCategoryDetailsDrawerOpen;
        private bool _IsProductDetailsDrawerOpen;

        private bool _IsDeleteCategoryDialogOpen;
        private bool _IsDeleteProductDialogOpen;
        private Category _selectedCategory;
        private ICollectionView _filtredProducts;
        private Product _selectedProduct;
        private BindableCollection<Product> _currentProducts;
        private BindableCollection<Category> _currentCategory;
        private int _productPageSize;
        private int _categpryPageSize;
        private CollectionViewSource productsViewSource;
        private Product _selectedFreeProduct;
        private Product _clipboardProduct;

        public CheckoutSettingsViewModel()
        {

        }
        public CheckoutSettingsViewModel(int productPageSize, int categoryPageSize,
            IProductService productsService,
            ICategoryService categoriesService,
            IOrderService orderService,
            IWaiterService waiterService,
            IDelivereyService delivereyService) : this()
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
            _productPageSize = productPageSize;
            _categpryPageSize = categoryPageSize;
            _productsService = productsService;
            _categoriesService = categoriesService;
            _orderService = orderService;

            int getProductsStatusCode = 0;
            var allProducts = _productsService.GetAllProducts(ref getProductsStatusCode);
            if (getProductsStatusCode != 200)
            {
                ToastNotification.ErrorNotification(getProductsStatusCode);
                return;
            }
            AllProducts = allProducts.Cast<Product>().ToList();
            ProductPageSize = productPageSize;
            CategoryPageSize = categoryPageSize;
            AllCategories = RetrieveCategories(AllProducts).Cast<Category>().ToList();
            LoadCategoryPages();
            CurrentProducts = new BindableCollection<Product>();
            var freeProds = AllProducts.Where(p => p.Category == null);
            FreeProducts = new BindableCollection<Product>(freeProds);
            //productsViewSource = new CollectionViewSource();
            //NotAffectedToAnyCategoryProducts.Filter = (o) => (o as Product).CategorieId == null;
            //GenarateRanksForProducts();

        }

        //private void NotAffectedToAnyCategoryProductFilter(object sender, FilterEventArgs e)
        //{
        //    Product product = e.Item as Product;
        //    if (product != null)
        //    {
        //        // Filter out products with price 25 or above
        //        if (product.CategorieId != null)
        //        {
        //            e.Accepted = true;
        //        }
        //        else
        //        {
        //            e.Accepted = false;
        //        }
        //    }
        //}

        public BindableCollection<Product> FreeProducts { get; set; }

        void GenarateRanksForProducts()
        {
            bool existsNumber(int[] tab, int value, int end)
            {
                for (int i = 0; i < end; i++)
                {
                    if (tab[i] == value)
                    {
                        return true;
                    }
                }
                return false;
            }

            foreach (var cat in AllCategories)
            {
                var random = new Random();
                var catProducts = AllProducts.Where(p => p.Category == cat);
                var t = new int[20];
                int i = 0, end = 1;
                Console.WriteLine("-------------------- " + cat.Name);
                foreach (var prod in catProducts)
                {
                    var r = random.Next(1, 21);
                    while (existsNumber(t, r, end))
                    {
                        r = random.Next(1, 21);
                    }
                    prod.Rank = r;
                    Console.Write(r + " ");
                    t[i++] = r;
                    end++;
                }
                

            }
            int j = 1;
            foreach (var prod in AllProducts)
            {
                Console.WriteLine(j++ +" Done   --> " + prod.Rank);
                var code = _productsService.UpdateProduct(prod);
                    Console.WriteLine(code);
                if (code!=200)
                {

                    Console.WriteLine(code);
                }

            }
        }
        
        public List<Product> AllProducts { get;  }
        public List<Category> AllCategories { get;  }
        public int ProductPageSize { get; private set; }
        public int CategoryPageSize { get; private set; }

        public BindableCollection<Product> CurrentProducts
        {
            get => _currentProducts;
            set
            {
                Set(ref _currentProducts, value);
            }
        }
        public BindableCollection<Category> CurrentCategories
        {
            get => _currentCategory;
            set
            {
                Set(ref _currentCategory, value);
            }
        }

        //public CollectionViewSource ProductsViewSource { get; set; }
        //public ICollectionView FiltredProducts
        //{
        //    get => _filtredProducts;
        //    set
        //    {
        //        _filtredProducts = value;
        //        NotifyOfPropertyChange();
        //    }
        //}

        public Product SelectedProduct 
        { 
            get => _selectedProduct;
            set
            {
                Set(ref _selectedProduct, value);
            } 
        }
        public Product SelectedFreeProduct
        { 
            get => _selectedFreeProduct;
            set
            {
                Set(ref _selectedFreeProduct, value);
            } 
        }
        public Product ClipboardProduct
        { 
            get => _clipboardProduct;
            set
            {
                Set(ref _clipboardProduct, value);
            } 
        }
        public Category SelectedCategory 
        { 
            get => _selectedCategory;
            set
            {
                Set(ref _selectedCategory, value);
            } 
        }
        public bool IsCategoryDetailsDrawerOpen
        {
            get => _IsCategoryDetailsDrawerOpen;
            set => Set(ref _IsCategoryDetailsDrawerOpen, value);
        }
        public bool IsProductDetailsDrawerOpen
        {
            get => _IsProductDetailsDrawerOpen;
            set => Set(ref _IsProductDetailsDrawerOpen, value);
        }

        public bool IsDeleteCategoryDialogOpen
        {
            get => _IsDeleteCategoryDialogOpen;
            set => Set(ref _IsDeleteCategoryDialogOpen, value);
        }

        public bool IsDeleteProductDialogOpen
        {
            get => _IsDeleteProductDialogOpen;
            set => Set(ref _IsDeleteProductDialogOpen, value);
        }

        void LoadCategoryPages()
        {
            var comparer = new Comparer<Category>();
            var categories = new List<Category>(AllCategories);
            categories.Sort(comparer);
            CurrentCategories = new BindableCollection<Category>();
            int nbpage = categories.Count / _categpryPageSize + categories.Count % _categpryPageSize == 0 ? 0 : 1;
            nbpage = nbpage == 0 ? 1 : nbpage;
            var size = nbpage * _categpryPageSize;
            int icat = 0;
            for (int i = 1; i <= size; i++)
            {
                var cat = categories[icat];
                if (cat.Rank == i)
                {
                    CurrentCategories.Add(cat);
                    icat++;
                }
                else
                {
                    CurrentCategories.Add(new Category { Rank = i });
                }
            }
            //CurrentCategories.
        }

        public void ShowCategoryProducts(Category category)
        {
            var filteredProducts = AllProducts.Where(p => p.Category == category);
            CurrentProducts.Clear();
            var comparer = new Comparer<Product>();
            var listOfFliteredProducts = filteredProducts.ToList();  //new List<Category>(AllCategories);
            listOfFliteredProducts.Sort(comparer);
            SelectedCategory = category;
            var iprod = 0;
            for (int i = 1; i <= _productPageSize; i++)
            {
                Product prod = null;
                 
                if (iprod < listOfFliteredProducts.Count)
                {
                    prod = listOfFliteredProducts[iprod];
                }
                else
                {
                    prod = null;
                }
                if (prod != null && prod.Rank == i)
                {
                    CurrentProducts.Add(prod);
                    iprod++;
                }
                else
                {
                    CurrentProducts.Add(new Product { Rank = i }) ;
                }
            }
        }

        public void SelectProdcut(Product product)
        {
            SelectedProduct = product;
           
        }

        public void SelectCategory(Category category)
        {
            SelectedCategory = category;
           
        }

        public void RemoveProductFromCategory()
        {
            var index = CurrentProducts.IndexOf(SelectedProduct);
            var rank = SelectedProduct.Rank;
            var freeProd = SelectedProduct;
            SelectedProduct.Rank = null;
            SelectedProduct.Category = null;
            CurrentProducts[index] = new Product { Rank = rank };
            SelectedProduct = null;
            //var freeProds = AllProducts.Where(p => p.CategorieId==null);
            //FreeProducts.Clear();
            FreeProducts.Add(freeProd);
        }

        public void AttachProductToCategory()
        {
            if (SelectedFreeProduct == null || SelectedProduct == null)
            {
                ToastNotification.Notify("Select Both free product and category product");
                return;
            }

            var index = CurrentProducts.IndexOf(SelectedProduct);
            SelectedFreeProduct.Rank = SelectedProduct.Rank;
            SelectedFreeProduct.Category = SelectedCategory;
            
            if (SelectedProduct.Category != null)
            {
                SelectedProduct.Rank = null;
                SelectedProduct.Category = null;
                FreeProducts.Add(SelectedProduct);
            }
            CurrentProducts[index] = SelectedFreeProduct;
            FreeProducts.Remove(SelectedFreeProduct);
        }

        public void CopyProduct()
        {
            ClipboardProduct = SelectedProduct;
            //if (SelectedProduct is Platter)
            //{
            //    ClipboardProduct = new Platter(SelectedProduct as Platter);
            //}
            //else
            //{
            //    ClipboardProduct = new Product(SelectedProduct);
            //}
        }

        public void PasteProduct()
        {
            if (ClipboardProduct == null)
            {
                ToastNotification.Notify("Copy a product before");
                return;
            }
            if (SelectedProduct == null)
            {
                ToastNotification.Notify("Selcect a zone to copy in");
                return;
            }

            var index = CurrentProducts.IndexOf(SelectedProduct);
            var product = ClipboardProduct is Platter ? new Platter(ClipboardProduct as Platter) : new Product(ClipboardProduct);
            product.Rank = SelectedProduct.Rank;
            product.Category = SelectedCategory;
            if (SelectedProduct.Category != null)
            {
                SelectedProduct.Rank = null;
                SelectedProduct.Category = null;
                FreeProducts.Add(SelectedProduct);
            }
            CurrentProducts[index] = product;
        }

        public void MoveProductTo()
        {
            if (SelectedProduct == null || SelectedProduct.Category == null)
            {
                ToastNotification.Notify("Selcect a product to move");
                return;
            }
            ClipboardProduct = SelectedProduct;
            //if (SelectedProduct is Platter)
            //{
            //    ClipboardProduct = new Platter(SelectedProduct as Platter);
            //}
            //else
            //{
            //    ClipboardProduct = new Product(SelectedProduct);
            //}
        }

        public void DetailsCategory()
        {
            IsCategoryDetailsDrawerOpen = true;
        }

        public void DetailsProduct()
        {
            IsProductDetailsDrawerOpen = true;
        }     
        public void DeleteCategory()
        {
            IsDeleteCategoryDialogOpen = true;
   
        }    
        
        public void DeleteProduct()
        {
            IsDeleteProductDialogOpen = true;
   
        }


    }

    class Comparer<T> : IComparer<T> where T : Ranked
    {
        public int Compare(T x, T y)
        {
            if (x.Equals(y))
            {
                return 0;
            }
            if (x.Rank > y.Rank)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
    
}
