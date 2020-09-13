using Caliburn.Micro;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

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
        private Product _toMoveProduct;
        private Category _categoryOfSelectFreeProduct;

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
            ToSaveUpdate = new BindableCollection<object>();
            ToSaveUpdate.CollectionChanged += ToSaveUpdateChanged;
            //productsViewSource = new CollectionViewSource();
            //NotAffectedToAnyCategoryProducts.Filter = (o) => (o as Product).CategorieId == null;
            //GenarateRanksForProducts();

        }

        private void ToSaveUpdateChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var product = e.NewItems;
            }

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
        public BindableCollection<object> ToSaveUpdate { get; set; }
        public BindableCollection<Product> ToDeletge { get; set; }

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
                Console.WriteLine(j++ + " Done   --> " + prod.Rank);
                var code = _productsService.UpdateProduct(prod);
                Console.WriteLine(code);
                if (code != 200)
                {

                    Console.WriteLine(code);
                }

            }
        }

        public List<Product> AllProducts { get; }
        public List<Category> AllCategories { get; }
        public int ProductPageSize
        {
            get => _productPageSize;
            set
            {
                Set(ref _productPageSize, value);
            }
        }

        public int CategoryPageSize
        {
            get => _categpryPageSize;
            set
            {
                Set(ref _categpryPageSize, value);
            }
        }

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
                if (_selectedFreeProduct != null && 
                    !string.IsNullOrEmpty(_selectedFreeProduct.Name) && 
                    SelectedFreeProductIsChanged)
                {
                    ToSaveUpdate.Add(_selectedFreeProduct);
                    SelectedFreeProductIsChanged = false;
                }
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
        public Product ProductToMove
        {
            get => _toMoveProduct;
            set
            {
                Set(ref _toMoveProduct, value);
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
        public Category CategoryOfSelectFreeProduct
        {
            get => _categoryOfSelectFreeProduct;
            set
            {
                Set(ref _categoryOfSelectFreeProduct, value);
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
        public bool SelectedFreeProductIsChanged { get; private set; }

        private static void LoadPages<T>(List<T> source, BindableCollection<T> dest, int size) where T : Ranked, new()
        {
            int icat = 0;
            for (int i = 1; i <= size; i++)
            {
                T cat = default;
                if (icat < source.Count)
                {
                    cat = source[icat];
                }
                else
                {
                    cat = default;
                }

                if (cat != null && cat.Rank == i)
                {
                    dest.Add(cat);
                    icat++;
                }
                else
                {
                    dest.Add(new T { Rank = i });
                }

            }
        }

         void LoadCategoryPages()
        {
            var comparer = new Comparer<Category>();
            var categories = new List<Category>(AllCategories);
            categories.Sort(comparer);
            CurrentCategories = new BindableCollection<Category>();
            var maxRank = (int)categories.Max(c => c.Rank);
            int nbpage = (maxRank / _categpryPageSize) + (maxRank % _categpryPageSize == 0 ? 0 : 1);
            nbpage = nbpage == 0 ? 1 : nbpage;
            var size = nbpage * _categpryPageSize;

            LoadPages(categories, CurrentCategories, size);

            //int icat = 0;
            //for (int i = 1; i <= size; i++)
            //{
            //    Category cat = null;
            //    if (icat < categories.Count)
            //    {
            //        cat = categories[icat];
            //    }
            //    else
            //    {
            //        cat = null;
            //    }

            //    if (cat != null && cat.Rank == i)
            //    {
            //        CurrentCategories.Add(cat);
            //        icat++;
            //    }
            //    else
            //    {
            //        CurrentCategories.Add(new Category { Rank = i });
            //    }
            //}
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
            LoadPages(listOfFliteredProducts, CurrentProducts, ProductPageSize);
            //var iprod = 0;
            //for (int i = 1; i <= _productPageSize; i++)
            //{
            //    Product prod = null;
                 
            //    if (iprod < listOfFliteredProducts.Count)
            //    {
            //        prod = listOfFliteredProducts[iprod];
            //    }
            //    else
            //    {
            //        prod = null;
            //    }
            //    if (prod != null && prod.Rank == i)
            //    {
            //        CurrentProducts.Add(prod);
            //        iprod++;
            //    }
            //    else
            //    {
            //        CurrentProducts.Add(new Product { Rank = i }) ;
            //    }
            //}
        }

        public void SelectProdcut(Product product)
        {
            SelectedProduct = product;
            if (ProductToMove !=null)
            {
                var index = CurrentProducts.IndexOf(ProductToMove);
                var prod = new Product { Rank = ProductToMove.Rank };
                if (SelectedProduct.Equals(ProductToMove))
                {
                    ProductToMove = null;
                    return;
                }
                PutProductInCellOf(SelectedProduct, ProductToMove);
                CurrentProducts[index] = prod;
                ProductToMove = null;
                return;
            }

            if (SelectedFreeProduct != null)
            {
                AttachProductToCategory();
            }
           
        }

        public void SelectCategory(Category category)
        {
            SelectedCategory = category;
           
        }

        public void ProductChanged(Product product)
        {
            SelectedFreeProductIsChanged = true;
        }
        public void RemoveProductFromCategory()
        {
            if (SelectedProduct == null)
            {
                return;
            }
            var index = CurrentProducts.IndexOf(SelectedProduct);
            var rank = SelectedProduct.Rank;
            var freeProd = SelectedProduct;
            SelectedProduct.Rank = null;
            SelectedProduct.Category = null;
            CurrentProducts[index] = new Product { Rank = rank };
            //var freeProds = AllProducts.Where(p => p.CategorieId==null);
            //FreeProducts.Clear();
            FreeProducts.Add(freeProd);
            SelectedFreeProduct = null;
        }

        public void AttachProductToCategory()
        {
            if (SelectedFreeProduct == null || SelectedProduct == null)
            {
                ToastNotification.Notify("Select Both free product and category product");
                return;
            }

            //var index = CurrentProducts.IndexOf(SelectedProduct);
            //SelectedFreeProduct.Rank = SelectedProduct.Rank;
            //SelectedFreeProduct.Category = SelectedCategory;
            
            //if (SelectedProduct.Category != null)
            //{
            //    SelectedProduct.Rank = null;
            //    SelectedProduct.Category = null;
            //    FreeProducts.Add(SelectedProduct);
            //}
            //CurrentProducts[index] = SelectedFreeProduct;
            PutProductInCellOf(SelectedProduct, SelectedFreeProduct);
            FreeProducts.Remove(SelectedFreeProduct);
        }

        public void CopyProduct()
        {
            if (SelectedProduct.Category == null)
            {
                ToastNotification.Notify("We can't copy empty product");
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

        private void PutProductInCellOf(Product sourceProduct, Product desProduct)
        {
            var index = CurrentProducts.IndexOf(sourceProduct);
            
            desProduct.Rank = sourceProduct.Rank;
            desProduct.Category = SelectedCategory;
            if (sourceProduct.Category != null)
            {
                sourceProduct.Rank = null;
                sourceProduct.Category = null;
                FreeProducts.Add(sourceProduct);
            }
            CurrentProducts[index] = desProduct;
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
            if (ClipboardProduct.Equals(SelectedProduct))
            {
                return;
            }
            var product = ClipboardProduct is Platter ? new Platter(ClipboardProduct as Platter) : new Product(ClipboardProduct);
            PutProductInCellOf(SelectedProduct, product);

            //var index = CurrentProducts.IndexOf(SelectedProduct);
            //var product = ClipboardProduct is Platter ? new Platter(ClipboardProduct as Platter) : new Product(ClipboardProduct);
            //product.Rank = SelectedProduct.Rank;
            //product.Category = SelectedCategory;
            //if (SelectedProduct.Category != null)
            //{
            //    SelectedProduct.Rank = null;
            //    SelectedProduct.Category = null;
            //    FreeProducts.Add(SelectedProduct);
            //}
            //CurrentProducts[index] = product;
        }

        public void MoveProductTo()
        {
            if (SelectedProduct == null || SelectedProduct.Category == null)
            {
                ToastNotification.Notify("Selcect a product to move");
                return;
            }
            ProductToMove = SelectedProduct;
            
            //if (SelectedProduct is Platter)
            //{
            //    ClipboardProduct = new Platter(SelectedProduct as Platter);
            //}
            //else
            //{
            //    ClipboardProduct = new Product(SelectedProduct);
            //}
        }

        public void NewFreeProduct()
        {
            FreeProducts.Add(SelectedFreeProduct = new Platter());
        }

        public void CopyFreeProduct()
        {
            if (SelectedFreeProduct == null)
            {
                return;
            }
            var product = new Platter(SelectedFreeProduct as Platter) ;
        }

        public void CotegoryOfSelectFreeProductChanged(Category category)
        {
            ShowCategoryProducts(category);
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

        public void Close()
        {
            LoginViewModel loginvm = new LoginViewModel();
            loginvm.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(loginvm);
        }

        //private Point startPoint;
        public void FreeProductsList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            //startPoint = e.GetPosition(null);
        }

        public void FreeProductsList_MouseMove(object sender, MouseEventArgs e)
        {
            ToastNotification.Notify(sender.ToString(),1);
            // Get the current mouse position
            //Point mousePos = e.GetPosition(null);
            //Vector diff = startPoint - mousePos;
            //if (!(e.OriginalSource is ListBoxItem) || !(e.LeftButton == MouseButtonState.Pressed))
            //{
            //    Console.WriteLine((e.OriginalSource as DependencyObject).GetType().ToString());
            //    return;
            //}

            if (e.LeftButton == MouseButtonState.Pressed /*&&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)*/)
            {
                // Get the dragged ListViewItem
                ListBox listBox = sender as ListBox;
                ListBoxItem listBoxItem =
                    FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                // Find the data behind the ListViewItem
                Product product;
                if (listBoxItem != null)
                {
                    product = (Product)listBox.ItemContainerGenerator.
                                        ItemFromContainer(listBoxItem);
                    DataObject dragData = new DataObject("FreeProduct", product);
                    DragDrop.DoDragDrop(listBoxItem, dragData, DragDropEffects.Move);
                }
                //else
                //{
                //    DataObject dragData = new DataObject("FreeProduct", null);
                //}
               

                // Initialize the drag & drop operation
            }
        }


       

        public void ProductsList_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            //Point mousePos = e.GetPosition(null);
            //Vector diff = startPoint - mousePos;
            //if (!(e.OriginalSource is ListBoxItem) || !(e.LeftButton == MouseButtonState.Pressed))
            //{
            //    Console.WriteLine((e.OriginalSource as DependencyObject).GetType().ToString());
            //    return;
            //}

            if (e.LeftButton == MouseButtonState.Pressed /*&&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)*/)
            {
                // Get the dragged ListViewItem
                ListBox listBox = sender as ListBox;
                ListBoxItem listBoxItem =
                    FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                // Find the data behind the ListViewItem
                Product product;
                if (listBoxItem != null)
                {
                    product = (Product)listBox.ItemContainerGenerator.
                                        ItemFromContainer(listBoxItem);
                    if (product == null || product.Name == null)
                    {
                        return;
                    }
                    DataObject dragData = new DataObject("Product", product);
                    DragDrop.DoDragDrop(listBoxItem, dragData, DragDropEffects.Move);
                }
               

                // Initialize the drag & drop operation
            }
        }


        

        public void FreeProductsList_TouchDown(object sender, TouchEventArgs e)
        {

            ToastNotification.Notify(e.OriginalSource.ToString());
            //if (e.Handled)
            //{
            //    return;
            //}
            //// Get the current mouse position
            ////Point mousePos = e.GetPosition(null);
            ////Vector diff = startPoint - mousePos;


            ////if (e.LeftButton == MouseButtonState.Pressed &&
            ////    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
            ////    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))

            // Get the dragged ListViewItem
            ListBox listView = sender as ListBox;
            ListBoxItem listBoxItem =
                FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

            // Find the data behind the ListViewItem
            Product product = null;
            if (listBoxItem != null)
            {
                product = (Product)listView.ItemContainerGenerator.
                ItemFromContainer(listBoxItem);
                DataObject dragData = new DataObject("FreeProduct", product);
                DragDrop.AddQueryContinueDragHandler(listBoxItem, DragContrinueHandler);
                DragDrop.DoDragDrop(listBoxItem, dragData, DragDropEffects.Move);
            }

            //using (System.IO.StreamWriter file =
            //    new System.IO.StreamWriter(@"WriteLines2.txt"))
            //{

            //    file.WriteLine(product.Name);


            //    // Initialize the drag & drop operation
            //}
            
        }


        public void ProductsList_TouchDown(object sender, TouchEventArgs e)
        {
            //if (e.Handled)
            //{
            //    return;
            //}
            //// Get the current mouse position
            ////Point mousePos = e.GetPosition(null);
            ////Vector diff = startPoint - mousePos;


            ////if (e.LeftButton == MouseButtonState.Pressed &&
            ////    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
            ////    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))

            // Get the dragged ListViewItem
            ListBox listView = sender as ListBox;
            ListBoxItem listBoxItem =
                FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

            // Find the data behind the ListViewItem
            Product product = null;
            if (listBoxItem != null)
            {
                product = (Product)listView.ItemContainerGenerator.
                ItemFromContainer(listBoxItem);
                DataObject dragData = new DataObject("Product", product);
                DragDrop.AddQueryContinueDragHandler(listBoxItem, DragContrinueHandler);
                DragDrop.DoDragDrop(listBoxItem, dragData, DragDropEffects.Move);
            }

            //using (System.IO.StreamWriter file =
            //    new System.IO.StreamWriter(@"WriteLines2.txt"))
            //{

            //    file.WriteLine(product.Name);


            //    // Initialize the drag & drop operation
            //}
            ToastNotification.Notify("Touch event fired-- Product list ");
        }
        public void DragContrinueHandler(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action == DragAction.Continue && e.KeyStates != DragDropKeyStates.LeftMouseButton)
            {
                //_dragdropWindow.Close();
            }
        }

        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        public void ProductsList_DragEnter(object sender, DragEventArgs e)
        {
            if ( (!e.Data.GetDataPresent("FreeProduct") && !e.Data.GetDataPresent("Product")) ||
                    sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }
        public void ProductsList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("FreeProduct") ||
                e.Data.GetDataPresent("Product"))
            {
                
                ListBox listView = sender as ListBox;
                ListBoxItem listBoxItem =
                   FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                if (listBoxItem == null)
                {
                    return;
                }
                // Find the data behind the ListViewItem
                Product product = (Product)listView.ItemContainerGenerator.
                    ItemFromContainer(listBoxItem);

                Product productSrc;

                if (e.Data.GetDataPresent("FreeProduct"))
                {
                    productSrc = e.Data.GetData("FreeProduct") as Product;
                }
                else
                {
                    productSrc = e.Data.GetData("Product") as Product;
                }

                if (productSrc == null || productSrc.Name == null) return;

                if(productSrc.Rank == null)
                {
                    SelectedFreeProduct = productSrc;
                    SelectedProduct = product;
                    AttachProductToCategory();
                }
                else
                {
                    ProductToMove = productSrc;
                    SelectedProduct = product;
                    var index = CurrentProducts.IndexOf(ProductToMove);
                    var prod = new Product { Rank = ProductToMove.Rank };
                    if (SelectedProduct.Equals(ProductToMove))
                    {
                        ProductToMove = null;
                        return;
                    }
                    PutProductInCellOf(SelectedProduct, ProductToMove);
                    CurrentProducts[index] = prod;
                    ProductToMove = null;
                }
            }
        }
        public void FreeProductsList_Drop(object sender, DragEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }
            if (e.Data.GetDataPresent("Product"))
            {
                Product contact = e.Data.GetData("Product") as Product;
                ListBox listView = sender as ListBox;
                ListBoxItem listBoxItem =
                   FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                // Find the data behind the ListViewItem
                var productSrc = e.Data.GetData("Product") as Product;
                if (productSrc == null) return;
                if (string.IsNullOrEmpty(productSrc.Name)) 
                    return;
                if (productSrc.Rank == null)
                {
                    return;
                }


                SelectedProduct = productSrc;
                RemoveProductFromCategory();
                
            }
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
