using Caliburn.Micro;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using PosTest.Helpers;
using MaterialDesignThemes.Wpf;
using System.Threading.Tasks;
using PosTest.ViewModels.SubViewModel;

namespace PosTest.ViewModels
{
    public class CheckoutSettingsViewModel : Screen
    {
        private IProductService _productsService;
        private ICategoryService _categoriesService;


        private bool _IsProductDetailsDrawerOpen;
        private bool _IsDeleteCategoryDialogOpen;
        private bool _IsDeleteProductDialogOpen;
        private Category _selectedCategory;
        private Product _selectedProduct;
        private BindableCollection<Product> _currentProducts;
        private BindableCollection<Category> _currentCategory;
        private int _productPageSize;
        private int _categpryPageSize;
        private Product _selectedFreeProduct;
        private Product _clipboardProduct;
        private Product _toMoveProduct;
        private Category _categoryOfSelectFreeProduct;
        private Category _selectedFreeCategory;
        private Category _categoryToMove;
        private bool _isFlipped;
        private EditCategoryViewModel _editCategoryViewModel;
        private bool _isCategory;
        private EditProductViewModel _editProductViewModel;

        public CheckoutSettingsViewModel()
        {
        }

        public CheckoutSettingsViewModel(int productPageSize, int categoryPageSize,
            IProductService productsService,
            ICategoryService categoriesService) : this()
        {
            _productPageSize = productPageSize;
            _categpryPageSize = categoryPageSize;
            _productsService = productsService;
            _categoriesService = categoriesService;

            ProductPageSize = productPageSize;
            CategoryPageSize = categoryPageSize;
            int catStatusCode = 0, prodStatusCode = 0;

            (IEnumerable<Category> categories, IEnumerable<Product> products) =
                _categoriesService.GetAllCategoriesAndProducts(ref catStatusCode, ref prodStatusCode);
            if (catStatusCode != 200 && prodStatusCode != 200) return;
            AllProducts = products.ToList();
            AllCategories = categories.ToList();

            LoadCategoryPages();
            CurrentProducts = new BindableCollection<Product>();
            var freeProds = AllProducts.Where(p => p.Rank == null);
            var freeCategories = AllCategories.Where(c => c.Rank == null);

            FreeProducts = new BindableCollection<Product>(freeProds);
            FreeCategories = new BindableCollection<Category>(freeCategories);

            ToSaveUpdate = new BindableCollection<object>();
            ToSaveUpdate.CollectionChanged += ToSaveUpdateChanged;
            SelectedProduct = new Product();
            SelectedProduct.PropertyChanged += (sender, args) => { Save(); };
            IsFlipped = false;
            IsCategory = false;
            
        }

        private void EditProductViewModel_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => IsSaveEnabled);
        }

        private void EditCategoryViewModel_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => IsSaveEnabled);
        }

        private void ToSaveUpdateChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var product = e.NewItems;
            }
        }

        public BindableCollection<Product> FreeProducts { get; set; }
        public BindableCollection<Category> FreeCategories { get; set; }
        public BindableCollection<object> ToSaveUpdate { get; set; }
        public BindableCollection<Product> ToDeletge { get; set; }

        public EditCategoryViewModel EditCategoryViewModel
        {
            get => _editCategoryViewModel;
            set => Set(ref _editCategoryViewModel, value);
        }

        public EditProductViewModel EditProductViewModel
        {
            get => _editProductViewModel;
            set => Set(ref _editProductViewModel, value);
        }

        public bool IsSaveEnabled
        {
            get
            {
                if (IsCategory)
                {
                    if (EditCategoryViewModel!=null)
                    {
                        return !EditCategoryViewModel.HasErrors;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (EditProductViewModel != null)
                    {
                        return !EditProductViewModel.HasErrors;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

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
                foreach (var prod in catProducts)
                {
                    var r = random.Next(1, 21);
                    while (existsNumber(t, r, end))
                    {
                        r = random.Next(1, 21);
                    }

                    prod.Rank = r;
                    t[i++] = r;
                    end++;
                }
            }

            int j = 1;
            foreach (var prod in AllProducts)
            {
                var code = _productsService.UpdateProduct(prod);
                if (code != 200)
                {
                }
            }
        }

        public List<Product> AllProducts { get; }
        public List<Category> AllCategories { get; }

        public int ProductPageSize
        {
            get => _productPageSize;
            set { Set(ref _productPageSize, value); }
        }

        public int CategoryPageSize
        {
            get => _categpryPageSize;
            set { Set(ref _categpryPageSize, value); }
        }

        public BindableCollection<Product> CurrentProducts
        {
            get => _currentProducts;
            set => Set(ref _currentProducts, value);
        }

        public BindableCollection<Category> CurrentCategories
        {
            get => _currentCategory;
            set => Set(ref _currentCategory, value);
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                Set(ref _selectedProduct, value);
                if (EditProductViewModel != null)
                {
                    this.EditProductViewModel.Source = this.SelectedProduct;
                    NotifyOfPropertyChange(() => EditProductViewModel);
                }

                ;
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

        public Category SelectedFreeCategory
        {
            get => _selectedFreeCategory;
            set
            {
                if (_selectedFreeCategory != null &&
                    !string.IsNullOrEmpty(_selectedFreeCategory.Name) &&
                    SelectedFreeCategoryIsChanged)
                {
                    ToSaveUpdate.Add(_selectedFreeProduct);
                    SelectedFreeProductIsChanged = false;
                }

                Set(ref _selectedFreeCategory, value);
            }
        }

        public bool SelectedFreeCategoryIsChanged { get; private set; }

        public Product ClipboardProduct
        {
            get => _clipboardProduct;
            set { Set(ref _clipboardProduct, value); }
        }

        public Product ProductToMove
        {
            get => _toMoveProduct;
            set { Set(ref _toMoveProduct, value); }
        }

        public Category CategoryToMove
        {
            get => _categoryToMove;
            set { Set(ref _categoryToMove, value); }
        }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                Set(ref _selectedCategory, value);
                if (EditCategoryViewModel != null)
                {
                    this.EditCategoryViewModel.Source = this.SelectedCategory;
                    NotifyOfPropertyChange(() => EditCategoryViewModel);
                }
            }
        }

        public Category CategoryOfSelectFreeProduct
        {
            get => _categoryOfSelectFreeProduct;
            set { Set(ref _categoryOfSelectFreeProduct, value); }
        }

        public bool IsFlipped
        {
            get => _isFlipped;
            set => Set(ref _isFlipped, value);
        }

        public bool IsCategory
        {
            get => _isCategory;
            set => Set(ref _isCategory, value);
        }

        public SnackbarMessageQueue MessageQueue { get; } = new SnackbarMessageQueue();

        private void Notify(string Message)
        {
            Task.Factory.StartNew(() =>
                MessageQueue.Enqueue(Message, null, null, null, false, true, TimeSpan.FromSeconds(1)));
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


        void LoadCategoryPages()
        {
            var comparer = new Comparer<Category>();
            var categories = new List<Category>(AllCategories.Where(c => c.Rank != null));
            categories.Sort(comparer);
            CurrentCategories = new BindableCollection<Category>();
            var maxRank = (int) categories.Max(c => c.Rank);
            int nbpage = (maxRank / _categpryPageSize) + (maxRank % _categpryPageSize == 0 ? 0 : 1);
            nbpage = nbpage == 0 ? 1 : nbpage;
            var size = nbpage * _categpryPageSize;

            RankedItemsCollectionHelper.LoadPagesFilled(source: categories, target: CurrentCategories, size: size);
        }

        public void ShowCategoryProducts(Category category)
        {
            CurrentProducts.Clear();
            if (category.Id == null) return;
            var filteredProducts = AllProducts.Where(p => p.Category == category && p.Rank != null);

            var comparer = new Comparer<Product>();
            var listOfFliteredProducts = filteredProducts.ToList();
            listOfFliteredProducts.Sort(comparer);
            SelectedCategory = category;
            RankedItemsCollectionHelper.LoadPagesFilled(source: listOfFliteredProducts, target: CurrentProducts,
                size: ProductPageSize, parameter: category);
        }

        public void SelectProdcut(Product product, MouseEventArgs e)
        {
            SelectedProduct = product;
            if (ProductToMove != null)
            {
                var index = CurrentProducts.IndexOf(ProductToMove);
                var prod = new Product {Rank = ProductToMove.Rank};
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
            var freep = SelectedFreeProduct;
            RemoveTElementFromTList(SelectedProduct, ref freep, CurrentProducts, FreeProducts);
            SelectedFreeProduct = freep;
        }

        public void RemoveCategoryFromList()
        {
            var freeCategory = SelectedFreeCategory;
            RemoveTElementFromTList(SelectedCategory, ref freeCategory, CurrentCategories, FreeCategories);
            SelectedFreeCategory = freeCategory;
        }

        public void Save()
        {
            if (IsCategory)
            {
                EditCategoryViewModel.SaveCategory();
                EditCategoryViewModel = null;
                IsCategory = false;
            }
            else
            {
                EditProductViewModel.SaveProduct();
                EditProductViewModel = null;
            }
            IsFlipped = false;
        }

        public void Cancel()
        {
            if (IsCategory)
            {
                EditCategoryViewModel.Cancel();
                //IsCategory = false;
            }
            else
            {
                EditProductViewModel.Cancel();
            }
            //IsFlipped = false;
        }

        public void RemoveTElementFromTList<T>(T SelectedT, ref T SelectedFreeT,
            BindableCollection<T> CurrentTs, BindableCollection<T> FreeTs) where T : Ranked, new()
        {
            if (SelectedT == null)
            {
                return;
            }

            var index = CurrentTs.IndexOf(SelectedT);
            var rank = SelectedT.Rank;
            var freeT = SelectedT;
            SelectedT.Rank = null;


            if (SelectedT is Product selectedProduct)
            {
                selectedProduct.Category.ProductIds.Remove((long) selectedProduct.Id);
                _categoriesService.UpdateCategory(selectedProduct.Category);
                selectedProduct.Category = null;
                selectedProduct.CategorieId = null;
                _productsService.UpdateProduct(selectedProduct);
            }

            if (SelectedT is Category selectedCategory)
            {
                //TODO Fixing removal of categories from the current Categories list
                foreach (var product in selectedCategory.Products)
                {
                    product.Rank = null;
                    product.CategorieId = null;
                    product.Category = null;
                    _productsService.UpdateProduct(product);
                    if (!FreeProducts.Contains(product))
                    {
                        FreeProducts.Add(product);
                    }
                }

                // implementing  update many products in the backend
                CurrentProducts.Clear();
                SelectedCategory = null;

                selectedCategory.Products.Clear();
                selectedCategory.ProductIds.Clear();
                _categoriesService.UpdateCategory(selectedCategory);
            }

            CurrentTs[index] = new T {Rank = rank};
            FreeTs.Add(freeT);
        }


        public void RemoveCategory()
        {
            if (SelectedCategory == null)
            {
                return;
            }

            var index = CurrentCategories.IndexOf(SelectedCategory);
            var rank = SelectedProduct.Rank;
            var freeProd = SelectedProduct;
            SelectedProduct.Rank = null;
            SelectedProduct.Category = null;
            CurrentProducts[index] = new Product {Rank = rank};
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
        }


        public void OpenProdcutDetail()
        {
            ToastNotification.Notify("Open Form", 1);
            IsFlipped = true;
        }

        private void PutProductInCellOf(Product targetProduct, Product sourceProduct)
        {
            var index = CurrentProducts.IndexOf(targetProduct);

            sourceProduct.Rank = targetProduct.Rank;
            sourceProduct.Category = SelectedCategory;
            sourceProduct.CategorieId = SelectedCategory.Id;


            if (targetProduct.Category != null && targetProduct.Id != null)
            {
                targetProduct.Rank = null;
                targetProduct.Category = null;

                _productsService.UpdateProduct(targetProduct);
            }

            CurrentProducts[index] = sourceProduct;
            if (sourceProduct.Id == null)
            {
                long id = -1;
                _productsService.SaveProduct(sourceProduct, ref id);
                sourceProduct.Id = id;
            }
            else
            {
                _productsService.UpdateProduct(sourceProduct);
            }

            SelectedCategory.ProductIds.Add((long) sourceProduct.Id);
            SelectedCategory.Products.Add(sourceProduct);
            _categoriesService.UpdateCategory(SelectedCategory);
        }

        private void PutCategoryInCellOf(Category sourceCategory, Category destinationCategory)
        {
            var index = CurrentCategories.IndexOf(sourceCategory);

            destinationCategory.Rank = sourceCategory.Rank;
            if (sourceCategory.Name != null)
            {
                sourceCategory.Rank = null;
                _categoriesService.UpdateCategory(sourceCategory);
                FreeCategories.Add(sourceCategory);
            }

            if (destinationCategory.Id == null)
            {
                long id = -1;
                _categoriesService.SaveCategory(destinationCategory, ref id);
                destinationCategory.Id = id;
            }
            else
            {
                _categoriesService.UpdateCategory(destinationCategory);
            }

            CurrentCategories[index] = destinationCategory;
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

            var product = ClipboardProduct is Platter
                ? new Platter(ClipboardProduct as Platter)
                : new Product(ClipboardProduct);
            product.Category = SelectedCategory;
            product.Id = null;
            PutProductInCellOf(SelectedProduct, product);
        }

        public void MoveProductTo()
        {
            if (SelectedProduct?.Name == null || SelectedProduct.Category.Id == null)
            {
                ToastNotification.Notify("Selcect a product to move");
                return;
            }

            ProductToMove = SelectedProduct;
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

            var product = new Platter(SelectedFreeProduct as Platter);
        }

        public void CotegoryOfSelectFreeProductChanged(Category category)
        {
            ShowCategoryProducts(category);
        }

        public void DetailsProduct()
        {
            IsProductDetailsDrawerOpen = true;
        }

        public void DeleteCategory()
        {
            if (SelectedFreeCategory == null)
            {
                return;
            }

            if (SelectedFreeCategory.Id != null)
            {
                _categoriesService.DeleteCategory((long) SelectedFreeCategory.Id);
                AllCategories.Remove(SelectedFreeCategory);
                FreeCategories.Remove(SelectedFreeCategory);
                SelectedFreeCategory = null;
            }
        }

        public void DeleteProduct()
        {
            if (SelectedFreeProduct == null)
            {
                return;
            }

            if (SelectedFreeProduct.Id != null)
            {
                _productsService.DeleteProduct((long) SelectedFreeProduct.Id);
                AllProducts.Remove(SelectedFreeProduct);
                FreeProducts.Remove(SelectedFreeProduct);
                SelectedFreeProduct = null;
            }
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
            var key = "FreeProduct";
            MouseMoveEventHandler<Product>(sender, e, key);
        }

        public void ProductsList_MouseMove(object sender, MouseEventArgs e)
        {
            var key = "Product";
            MouseMoveEventHandler<Product>(sender, e, key);
        }

        public void FreeCategoriesList_MouseMove(object sender, MouseEventArgs e)
        {
            var key = "FreeCategory";
            MouseMoveEventHandler<Category>(sender, e, key);
        }

        public void CategoriesList_MouseMove(object sender, MouseEventArgs e)
        {
            var key = "Category";

            MouseMoveEventHandler<Category>(sender, e, key);
        }

        public static void MouseMoveEventHandler<T>(object sender, MouseEventArgs e, string key,
            string requiredPropertyName = "Name") where T : Ranked, new()
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ListBox listBox = sender as ListBox;
                ListBoxItem listBoxItem =
                    FindAncestor<ListBoxItem>((DependencyObject) e.OriginalSource);
                T t;
                if (listBoxItem != null)
                {
                    t = (T) listBox.ItemContainerGenerator.ItemFromContainer(listBoxItem);
                    if (t == null || t.GetType().GetProperty(requiredPropertyName) == null)
                    {
                        return;
                    }

                    DataObject dragData = new DataObject(key, t);
                    DragDrop.DoDragDrop(listBoxItem, dragData, DragDropEffects.Move);
                }
            }
        }

        public static void ListTouchDownEventHandler<T>(object sender, TouchEventArgs e, string key)
        {
            ListBox listBox = sender as ListBox;
            ListBoxItem listBoxItem = FindAncestor<ListBoxItem>((DependencyObject) e.OriginalSource);
            if (listBoxItem != null)
            {
                T t = (T) listBox.ItemContainerGenerator.ItemFromContainer(listBoxItem);
                if (t == null || t.GetType().GetProperty("Name") == null)
                {
                    return;
                }

                DataObject dragData = new DataObject(key, t);
                DragDrop.DoDragDrop(listBoxItem, dragData, DragDropEffects.Move);
            }
        }

        public void FreeProductsList_TouchDown(object sender, TouchEventArgs e)
        {
            var key = "FreeProduct";
            ListTouchDownEventHandler<Product>(sender, e, key);
        }

        public void ProductsList_TouchDown(object sender, TouchEventArgs e)
        {
            var key = "Product";
            ListTouchDownEventHandler<Product>(sender, e, key);
        }

        public void DragContrinueHandler(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action == DragAction.Continue && e.KeyStates != DragDropKeyStates.LeftMouseButton)
            {
            }
        }

        public static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T) current;
                }

                current = VisualTreeHelper.GetParent(current);
            } while (current != null);

            return null;
        }

        public void ProductsList_DragEnter(object sender, DragEventArgs e)
        {
            if ((!e.Data.GetDataPresent("FreeProduct")
                 && !e.Data.GetDataPresent("Product"))
                || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        public void ProductsList_Drop(object sender, DragEventArgs e)
        {
            if (SelectedCategory?.Id == null) return;
            if (e.Data.GetDataPresent("FreeProduct") ||
                e.Data.GetDataPresent("Product"))
            {
                ListBox listView = sender as ListBox;
                ListBoxItem listBoxItem =
                    FindAncestor<ListBoxItem>((DependencyObject) e.OriginalSource);

                if (listBoxItem == null)
                {
                    return;
                }

                // Find the data behind the ListViewItem
                Product targetProduct = (Product) listView.ItemContainerGenerator.ItemFromContainer(listBoxItem);

                Product receivedProduct;

                if (e.Data.GetDataPresent("FreeProduct"))
                {
                    receivedProduct = e.Data.GetData("FreeProduct") as Product;
                }
                else
                {
                    receivedProduct = e.Data.GetData("Product") as Product;
                }

                if (receivedProduct == null || receivedProduct.Name == null) return;

                if (receivedProduct.Rank == null)
                {
                    SelectedFreeProduct = receivedProduct;
                    SelectedProduct = targetProduct;
                    AttachProductToCategory();
                }
                else
                {
                    SelectedProduct = targetProduct;
                    var targetRank = targetProduct.Rank;
                    var receivedRank = receivedProduct.Rank;
                    targetProduct.Rank = receivedProduct.Rank;
                    receivedProduct.Rank = targetRank;
                    CurrentProducts[(int) targetRank - 1] = receivedProduct;
                    CurrentProducts[(int) receivedRank - 1] = targetProduct;
                    _productsService.UpdateProduct(receivedProduct);
                    _productsService.UpdateProduct(targetProduct);
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
                ListBoxItem listBoxItem = FindAncestor<ListBoxItem>((DependencyObject) e.OriginalSource);

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

        public void CategoriesList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("FreeCategory") ||
                e.Data.GetDataPresent("Category"))
            {
                ListBox listView = sender as ListBox;
                ListBoxItem listBoxItem =
                    FindAncestor<ListBoxItem>((DependencyObject) e.OriginalSource);

                if (listBoxItem == null)
                {
                    return;
                }

                // Find the data behind the ListViewItem
                Category category = (Category) listView.ItemContainerGenerator.ItemFromContainer(listBoxItem);

                Category categorySrc;

                if (e.Data.GetDataPresent("FreeCategory"))
                {
                    categorySrc = e.Data.GetData("FreeCategory") as Category;
                }
                else
                {
                    categorySrc = e.Data.GetData("Category") as Category;
                }

                if (categorySrc == null || categorySrc.Name == null) return;

                if (categorySrc.Rank == null)
                {
                    SelectedFreeCategory = categorySrc;
                    SelectedCategory = category;
                    AttachCategoryToList();
                }
                else
                {
                    CategoryToMove = categorySrc;
                    SelectedCategory = category;
                    var index = CurrentCategories.IndexOf(CategoryToMove);
                    var cat = new Category() {Rank = CategoryToMove.Rank};
                    if (SelectedCategory.Equals(CategoryToMove))
                    {
                        CategoryToMove = null;
                        return;
                    }

                    PutCategoryInCellOf(SelectedCategory, CategoryToMove);
                    CurrentCategories[index] = cat;
                    CategoryToMove = null;
                }
            }
        }

        private void AttachCategoryToList()
        {
            if (SelectedFreeCategory == null || SelectedCategory == null)
            {
                ToastNotification.Notify("Select Both free product and category product");
                return;
            }

            PutCategoryInCellOf(SelectedCategory, SelectedFreeCategory);
            FreeCategories.Remove(SelectedFreeCategory);
        }

        public void FreeCategoriesList_Drop(object sender, DragEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            if (e.Data.GetDataPresent("Category"))
            {
                Category contact = e.Data.GetData("Category") as Category;
                ListBox listView = sender as ListBox;
                ListBoxItem listBoxItem =
                    FindAncestor<ListBoxItem>((DependencyObject) e.OriginalSource);

                // Find the data behind the ListViewItem
                var categorySrc = e.Data.GetData("Category") as Category;
                if (categorySrc == null) return;
                if (string.IsNullOrEmpty(categorySrc.Name))
                    return;
                if (categorySrc.Rank == null)
                {
                    return;
                }

                SelectedCategory = categorySrc;
                RemoveCategoryFromList();
            }
        }

        public void EditProduct()
        {
            if (SelectedCategory.Id == null) return;
            this.EditProductViewModel = new EditProductViewModel(ref this._selectedProduct, this._productsService);
            EditProductViewModel.ErrorsChanged += EditProductViewModel_ErrorsChanged;
            IsCategory = false;
            NotifyOfPropertyChange((() => IsCategory));
            IsFlipped = true;
        }

        public void EditCategory()
        {
            this.EditCategoryViewModel = new EditCategoryViewModel(ref this._selectedCategory, this._categoriesService);
            EditCategoryViewModel.ErrorsChanged += EditCategoryViewModel_ErrorsChanged;
            IsCategory = true;
            IsFlipped = true;
        }

        public void SavedProduct()
        {
            EditProductViewModel = null;
            IsFlipped = false;
            IsCategory = false;
            if (SelectedProduct?.Id != null)
            {
                Save();
                Notify("Product Saved");
            }
        }

        //public void AdditiveChecked()
        //{
        //    ToastNotification.Notify("hi.cos");
        //}
    }

    public class Comparer<T> : IComparer<T> where T : Ranked
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