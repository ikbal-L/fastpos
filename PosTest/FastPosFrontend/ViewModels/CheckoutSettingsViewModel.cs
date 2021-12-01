using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using FastPosFrontend.Events;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using FastPosFrontend.ViewModels.Settings;
using MaterialDesignThemes.Wpf;
using ServiceInterface.ExtentionsMethod;
using ServiceInterface.Model;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{
    [NavigationItem("Checkout settings",target:typeof(CheckoutSettingsViewModel),groupName:"Settings",isQuickNavigationEnabled:true)]
    [PreAuthorize("Create_Product","Create_Category")]
    public class CheckoutSettingsViewModel : LazyScreen,ISettingsController
    {
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
        private CategoryDetailViewModel _categoryDetailViewModel;
        private bool _isCategory;
        private ProductDetailViewModel _productDetailViewModel;
        private Type _activeTab;
        private ProductLayoutViewModel _productLayoutViewModel;
        private bool _isDialogOpen;

        

        public CheckoutSettingsViewModel() : base()
        {
        }

        protected sealed override void Setup()
        {
            var categories = StateManager.GetAsync<Category>();
            var products = StateManager.GetAsync<Product>();

            _data = new NotifyAllTasksCompletion(categories,products);

            IsDialogOpen = false;
            ProductLayoutViewModel = new ProductLayoutViewModel();
            ProductLayoutViewModel.OnLayoutChanged(() =>
            {
                _productPageSize = ProductLayoutViewModel.Configuration.NumberOfProducts;
                var category = SelectedCategory;
                SelectedCategory = null;
                ShowCategoryProducts(category);
            });
            var pageSize = ProductLayoutViewModel.Configuration.NumberOfProducts;

            ProductPageSize = pageSize;
            CategoryPageSize = AppConfigurationManager.Configuration<GeneralSettings>().NumberOfCategories;

            CreateProductCommand = new DelegateCommandBase(CreateProduct);
            EditProductCommand = new DelegateCommandBase(EditProduct);
            MoveProductCommand = new DelegateCommandBase(MoveProductTo);
            CopyProductCommand = new DelegateCommandBase(CopyProduct);
            PasteProductCommand = new DelegateCommandBase(PasteProduct);
            RemoveProductCommand = new DelegateCommandBase(RemoveProductFromCategory);
            DeleteProductCommand = new DelegateCommandBase(DeleteProduct);

            CreateCategoryCommand = new DelegateCommandBase(CreateCategory);
            EditCategoryCommand = new DelegateCommandBase(EditCategory);
            MoveCategoryCommand = new DelegateCommandBase(MoveCategoryTo);
            CopyCategoryCommand = new DelegateCommandBase(CopyCategory);
            PasteCategoryCommand = new DelegateCommandBase(PasteCategory);
            RemoveCategoryCommand = new DelegateCommandBase(RemoveSelectedCategoryFromList);
            DeleteCategoryCommand = new DelegateCommandBase(DeleteCategory);

            ConfigureProductLayoutCommand = new DelegateCommandBase(ConfigureProductDisplayLayout);
            AddCategoryRowCommand = new DelegateCommandBase(AddCategoryRow);
        }

        public override void Initialize()
        {

       
            _allProducts = StateManager.Get<Product>().ToList();
  
           
            _allCategories = StateManager.Get<Category>().ToList();
            _allCategories.Where(c=>c.Rank!= null&& c.Products!= null).ToList().ForEach(c=>c.Products = c.Products.OrderBy(p => p.Rank).ToList());
           

            LoadCategoryPages();
            CurrentProducts = new BindableCollection<Product>();
            var freeProds = _allProducts.Where(p => p.Rank == null);
            var freeCategories = _allCategories.Where(c => c.Rank == null);

            FreeProducts = new BindableCollection<Product>(freeProds);
            FreeCategories = new BindableCollection<Category>(freeCategories);

            ToSaveUpdate = new BindableCollection<object>();
            ToSaveUpdate.CollectionChanged += ToSaveUpdateChanged;
            SelectedProduct = new Product();
         
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

        public CategoryDetailViewModel CategoryDetailViewModel
        {
            get => _categoryDetailViewModel;
            set => Set(ref _categoryDetailViewModel, value);
        }

        public ProductDetailViewModel ProductDetailViewModel
        {
            get => _productDetailViewModel;
            set => Set(ref _productDetailViewModel, value);
        }

        public bool IsSaveEnabled
        {
            get
            {
                if (IsCategory)
                {
                    if (CategoryDetailViewModel != null)
                    {
                        return !CategoryDetailViewModel.HasErrors;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (ProductDetailViewModel != null)
                    { return !ProductDetailViewModel.HasErrors;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }


        private List<Product> _allProducts;
        private List<Product> _activeProducts;
        private List<Category> _allCategories;
        private int _numberOfCategoryRows;
        private Category _clipboardCategory;

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

        public ProductLayoutViewModel ProductLayoutViewModel
        {
            get => _productLayoutViewModel;
            set => Set(ref _productLayoutViewModel, value);
        }

        public ProductLayout ProductLayout => ProductLayout.Instance;    

        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set => Set(ref _isDialogOpen, value);
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                Set(ref _selectedProduct, value);
                if (ProductDetailViewModel != null)
                {
                    ProductDetailViewModel.Source = SelectedProduct;
                    NotifyOfPropertyChange(() => ProductDetailViewModel);
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

        public Category ClipboardCategory
        {
            get => _clipboardCategory;
            set => Set(ref _clipboardCategory, value);
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
                if (CategoryDetailViewModel != null)
                {
                    CategoryDetailViewModel.Source = SelectedCategory;
                    NotifyOfPropertyChange(() => CategoryDetailViewModel);
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
            var categories = new List<Category>(_allCategories.Where(c => c.Rank != null));
            categories.Sort(comparer);
            CurrentCategories = new BindableCollection<Category>();
            var maxRank = categories.Max(c => c.Rank)??1;
            _numberOfCategoryRows = (maxRank / _categpryPageSize) + (maxRank % _categpryPageSize == 0 ? 0 : 1);
            
            var size = _numberOfCategoryRows * _categpryPageSize;

            RankedItemsCollectionHelper.LoadPagesFilled(source: categories, target: CurrentCategories, size: size);
        }

        public void ShowCategoryProducts(Category category)
        {

            if (CategoryToMove!= null)
            {
              
                if (category == CategoryToMove)
                {
                    CategoryToMove = null;
                    return;
                }

                var index = CurrentCategories.IndexOf(CategoryToMove);
                var cat = new Category() { Rank = CategoryToMove.Rank };
                if (category.Equals(CategoryToMove))
                {
                    CategoryToMove = null;
                    return;
                }

                //PutCategoryInCellOf(SelectedCategory, CategoryToMove);
                IList<Category> categories = CurrentCategories;
                try
                {
                    RankedItemsCollectionHelper.InsertTElementInPositionOf(ref _categoryToMove, ref category,
                        categories);
                }
                catch (Exception ex)
                {
#if DEBUG
                    throw;
#endif
                    NLog.LogManager.GetCurrentClassLogger().Error(ex);
                }

                try
                {
                    if (category.Id != null)
                    {
                        StateManager.Save(category);
                    }

                    StateManager.Save(CategoryToMove);
                    CurrentCategories = (BindableCollection<Category>)categories;
                }
                catch (AggregateException)
                {
                    ToastNotification.Notify("Problem connecting to server");
                }
                CategoryToMove = null;
                SelectedCategory = null;
                category = null;
                CurrentProducts?.Clear();
                return;
            }
            else if(SelectedFreeCategory!= null)
            {
                SelectedCategory = category;
                AttachCategoryToList();
            }
            SelectedProduct = null;
            if (category == SelectedCategory) return;
            SelectedCategory = category;
            if (category?.Id == null)
            {
                CurrentProducts?.Clear();
                return;
            }
            if (CurrentProducts?.Count> ProductPageSize)
            {
                foreach (var product in CurrentProducts.ToList())
                {
                    if (product.Rank> ProductPageSize)
                    {
                        CurrentProducts.Remove(product);
                    }
                }
                return;
            }

            CurrentProducts?.Clear();

            var filteredProducts = category.Products;

            var comparer = new Comparer<Product>();

            filteredProducts.Sort(comparer);
            RankedItemsCollectionHelper.LoadPagesFilled(source: filteredProducts, target: CurrentProducts,
                size: ProductPageSize, parameter: category);
        }

        public void SelectProduct(Product product)
        {
            
            if (ProductToMove != null)
            {
                SelectedFreeCategory = null;
                SelectedProduct = product;
                var index = CurrentProducts.IndexOf(ProductToMove);
                var prod = new Product { Rank = ProductToMove.Rank };
                if (SelectedProduct.Equals(ProductToMove))
                {
                    ProductToMove = null;
                    return;
                }


                var incomingProduct = ProductToMove;
                var targetProduct = SelectedProduct;
                IList<Product> products = CurrentProducts;
                RankedItemsCollectionHelper.InsertTElementInPositionOf(ref incomingProduct,ref targetProduct,  products);


                if (targetProduct?.Id != null)
                {
                    StateManager.Save(targetProduct);
                }

                StateManager.Save(incomingProduct);
                StateManager.Save(_currentCategory);
                //CurrentProducts[index] = prod;
                SelectedProduct = null;
                ProductToMove = null;
                return;
            }
            SelectedProduct = product;

            if (SelectedFreeProduct != null&& SelectedProduct?.Id == null)
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

        public void RemoveProductFromCategory(object obj)
        {
            if (SelectedProduct?.Id == null)
            {
                ToastNotification.Notify("Select a product First!");
                return;
            }


            var freep = SelectedFreeProduct;
            RemoveTElementFromTList(SelectedProduct, ref freep, CurrentProducts, FreeProducts);
            //SelectedFreeProduct = freep;
        }

        public void RemoveSelectedCategoryFromList(object obj)
        {
            bool result = true;
            if (SelectedCategory?.Id == null)
            {
                ToastNotification.Notify("Select a category First!");
                return;
            }

            if (SelectedCategory.Products != null && SelectedCategory.Products.Any())
            {
                 result = ModalDialogBox.YesNo("Do you want to remove this category and all of it associated products",
                    "Remove Category").Show();
                
            }

            if (!result) return;
            var freeCategory = SelectedFreeCategory;
            RemoveTElementFromTList(SelectedCategory, ref freeCategory, CurrentCategories, FreeCategories);


        }

        public void Save()
        {
            if (IsCategory)
            {
                CategoryDetailViewModel.SaveCategory();
                CategoryDetailViewModel = null;
                IsCategory = false;
                var category = SelectedCategory;
                SelectedCategory = null;
                ShowCategoryProducts(category);
            }
            else
            {
                ProductDetailViewModel.SaveProduct();
                //ProductDetailViewModel = null;
            }

            IsFlipped = false;
        }

        public void Cancel()
        {
            if (IsCategory)
            {
                CategoryDetailViewModel = null;
            }
            else
            {
                ProductDetailViewModel = null;
            }

            IsFlipped = false;
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
                RemoveProductFromList<T>(selectedProduct);
            }

            if (SelectedT is Category selectedCategory)
            {
                //TODO Fixing removal of categories from the current Categories list


                RemoveCategoryFromList(selectedCategory);
            }

            CurrentTs[index] = new T { Rank = rank };
            FreeTs.Add(freeT);
            //SelectedFreeT = freeT;
        }

        private void RemoveCategoryFromList(Category selectedCategory)
        {

            if (ManageCategoryProductsForDeletion(selectedCategory))
            {
                CurrentProducts?.Clear();
                SelectedCategory = null;

                if (StateManager.Save(selectedCategory))
                {
                    //ToastNotification.Notify("Category was Save successfully ", NotificationType.Success);
                }
            }

        }

        private void RemoveProductFromList<T>(Product selectedProduct) where T : Ranked, new()
        {
            try
            {

                selectedProduct.Category.ProductIds.Remove((long)selectedProduct.Id);
                StateManager.Save<Category>(selectedProduct.Category);

                selectedProduct.Category = null;
                selectedProduct.CategoryId = null;
                selectedProduct.Rank = null;
                StateManager.Save<Product>(selectedProduct);
            }
            catch (AggregateException)
            {
                ToastNotification.Notify("Problem connecting to server");
            }

        }

        private bool ManageCategoryProductsForDeletion(Category selectedCategory) 
        {
            //TODO SELECTED CATEGORY NULL
            if (selectedCategory.Products == null || !selectedCategory.Products.Any()) return true;
            selectedCategory.Products.ForEach(product =>
            {
                product.Rank = null;
                product.CategoryId = null;
                product.Category = null;

                if (!FreeProducts.Contains(product))
                {
                    FreeProducts.Add(product);
                }
            });

            if (!StateManager.Save(selectedCategory.Products)) return false;
            selectedCategory.Products= null;
            selectedCategory.ProductIds = null;
            return true;


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
            CurrentProducts[index] = new Product { Rank = rank };
            FreeProducts.Add(freeProd);
            SelectedFreeProduct = null;
        }


        public void AttachProductToCategory()
        {
            if (SelectedFreeProduct == null || SelectedProduct == null)
            {
                ToastNotification.Notify("Select Both free product and category product",NotificationType.Warning);
                return;
            }

            PutProductInCellOf(SelectedProduct, SelectedFreeProduct);
            //_activeProducts.Add(SelectedFreeProduct);
            FreeProducts.Remove(SelectedFreeProduct);
        }

        public void CopyProduct(object obj)
        {
            if (SelectedProduct?.Category == null || SelectedProduct.Id == null)
            {
                ToastNotification.Notify("Select a non empty product to copy",NotificationType.Warning);
                return;
            }

            ClipboardProduct = SelectedProduct;
        }

        public void CopyCategory(object obj)
        {
            if (SelectedCategory?.Id == null)
            {
                ToastNotification.Notify("Select a non empty category to copy", NotificationType.Warning);
                return;
            }

            ClipboardCategory = ClipboardCategory!= null ? null : SelectedCategory;
        }


        public void OpenProdcutDetail()
        {
            ToastNotification.Notify("Open Form", NotificationType.Information);
            IsFlipped = true;
        }

        private void PutProductInCellOf(Product targetProduct, Product sourceProduct)
        {
            var index = CurrentProducts.IndexOf(targetProduct);

            sourceProduct.Rank = targetProduct.Rank;
            sourceProduct.Category = SelectedCategory;
            sourceProduct.CategoryId = SelectedCategory.Id;

            if (targetProduct.Category != null && targetProduct.Id != null)
            {
                targetProduct.Rank = null;
                targetProduct.Category = null;

                StateManager.Save(targetProduct);
            }

            CurrentProducts[index] = sourceProduct;

            StateManager.Save(sourceProduct);

            SelectedCategory.ProductIds.Add((long)sourceProduct.Id);
            
            //var insertIndex = SelectedCategory.Products.FindIndex(p=> p.Rank-1 == sourceProduct.Rank);
            //SelectedCategory.Products.Insert(insertIndex,sourceProduct);
            SelectedCategory.Products.Add(sourceProduct);

            StateManager.Save(SelectedCategory);

        }

        //for test make it public 
        //TODO replace in test
        //DEPRECATED
        public void PutCategoryInCellOf(Category targetCategory, Category incomingCategory)
        {
            int indexOfTargetCategory = CurrentCategories.IndexOf(targetCategory);
            int indexOfIncomingCategory = CurrentCategories.IndexOf(incomingCategory);


            int targetCategoryRank = (int)targetCategory.Rank;
            targetCategory.Rank = incomingCategory.Rank;
            if (incomingCategory.Rank != null)
            {
                CurrentCategories[indexOfIncomingCategory] = targetCategory;
            }

            incomingCategory.Rank = targetCategoryRank;
            CurrentCategories[indexOfTargetCategory] = incomingCategory;

            StateManager.Save(targetCategory);

            StateManager.Save(incomingCategory);
        }

        public void PasteProduct(object obj)
        {
            if (ClipboardProduct == null)
            {
                ToastNotification.Notify("Copy a product before",NotificationType.Warning);
                return;
            }

            if (SelectedProduct == null)
            {
                ToastNotification.Notify("Select a zone to copy in",NotificationType.Warning);
                return;
            }

            if (SelectedProduct.Id!=null)
            {
                Notify("Select an empty cell to paste product in!");
                SelectedProduct = null;
                return;
            }

            if (ClipboardProduct.Equals(SelectedProduct))
            {
                return;
            }

            //var product = new Product(ClipboardProduct);
            var product = ClipboardProduct.Clone();
            product.Category = SelectedCategory;
            product.Id = null;
            product.Rank = null;
            
            var name = Regex.Replace(ClipboardProduct.Name, @"\([0-9]{1,2}\)", "");
            var count = CurrentProducts.Where(a => a.Name != null).Count(a => a.Name.Contains(name));
            product.Name = $"{name}({count})";
            var target = SelectedProduct;
            //PutProductInCellOf(SelectedProduct, product);

            int targetRank = target.Rank.Value;
            product.Rank = targetRank;
            CurrentProducts[targetRank - 1] = product;
            

            try
            {
                StateManager.Save(product);
                ClipboardProduct = null;
                //StateManager.Save(target);


            }
            catch (AggregateException)
            {
                ToastNotification.Notify("Problem connecting to server");
            }
        }

        public void PasteCategory(object obj)
        {
            if (ClipboardCategory == null)
            {
                ToastNotification.Notify("Copy a category before", NotificationType.Warning);
                return;
            }

            if (SelectedCategory == null)
            {
                ToastNotification.Notify("Select a zone to copy in", NotificationType.Warning);
                return;
            }

            if (SelectedCategory.Id != null)
            {
                Notify("Select an empty cell to paste category in!");
                SelectedProduct = null;
                return;
            }

            if (ClipboardCategory.Equals(SelectedCategory))
            {
                return;
            }

            //var product = new Product(ClipboardProduct);
            var category = ClipboardCategory.Clone();
            category.Id = null;
            category.Rank = null;
            category.Products = new List<Product>();
            category.ProductIds = new List<long>();

            var name = Regex.Replace(ClipboardCategory.Name, @"\([0-9]{1,2}\)", "");
            var count = CurrentCategories.Where(a => a.Name != null).Count(a => a.Name.Contains(name));
            category.Name = $"{name}({count})";
            var target = SelectedCategory;
            //PutProductInCellOf(SelectedProduct, product);

            int targetRank = target.Rank.Value;
            category.Rank = targetRank;
            CurrentCategories[targetRank - 1] = category;


            try
            {
                StateManager.Save(category);
                ClipboardCategory = null;
                //StateManager.Save(target);


            }
            catch (AggregateException)
            {
                ToastNotification.Notify("Problem connecting to server");
            }
        }

        public void MoveProductTo(object obj)
        {
            if (SelectedProduct?.Name == null || SelectedProduct.Category.Id == null)
            {
                ToastNotification.Notify("Selcect a product to move");
                return;
            }

            ProductToMove = SelectedProduct;
        }

        public void MoveCategoryTo(object obj)
        {
            if (SelectedCategory?.Name == null )
            {
                ToastNotification.Notify("Selcect a category to move");
                return;
            }

            CategoryToMove = CategoryToMove==null? SelectedCategory:null;
        }


        public void DeleteCategory(object obj)
        {
            if (SelectedCategory!=null && SelectedFreeCategory == null)
            {
                Notify("Category must be unassigned to be deleted");
            }

            if (SelectedFreeCategory?.Id != null)
            {

                try
                {


                    if (StateManager.Delete(SelectedFreeCategory))
                    {
                        
                    }
                    
                }
                catch (AggregateException)
                {
                    ToastNotification.Notify("Problem connecting to server");
                }
                
                _allCategories.Remove(SelectedFreeCategory);
                FreeCategories.Remove(SelectedFreeCategory);
                SelectedFreeCategory = null;
            }
        }

        public void DeleteProduct(object obj)
        {
            if (SelectedProduct != null && SelectedFreeProduct == null)
            {
                Notify("Product must be unassigned from category to be deleted");
            }

            if (SelectedFreeProduct?.Id != null)
            {
                StateManager.Delete(SelectedFreeProduct);


                _allProducts.Remove(SelectedFreeProduct);
                FreeProducts.Remove(SelectedFreeProduct);
                SelectedFreeProduct = null;
            }
        }

        public void Close()
        {
            //LoginViewModel loginvm = new LoginViewModel();
            //StateManager.Flush();
            //loginvm.Parent = this.Parent;
            //(this.Parent as Conductor<object>).ActivateItem(loginvm);
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

        public Type ActiveTab
        {
            get => _activeTab;
            set => Set(ref _activeTab, value);
        }

        public void SetActiveTab(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Product"))
            {
                ActiveTab = typeof(Product);
                return;
            }

            if (e.Data.GetDataPresent("Category"))
            {
                ActiveTab = typeof(Category);
            }
        }

        public void ExpandFreeItemsTabs(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("Product") && !e.Data.GetDataPresent("Category")) return;
            if (sender is Expander expander)
            {
                expander.IsExpanded = true;
            }


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

        public void CategoryList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count>0)
            {
                var category = e.AddedItems[0] as Category;
                ShowCategoryProducts(category);
            }
        }

        public void DragCategory(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Border border = (sender as Border);
                if (border?.DataContext is Category t)
                {
                    
                    if (t == null || t.GetType().GetProperty("Name").GetValue(t) == null)
                    {
                        return;
                    }

                    DataObject dragData = new DataObject("Category", t);
                    DragDrop.DoDragDrop(border, dragData, DragDropEffects.Move);
                }

                e.Handled = true;
            }
        }

        public static void MouseMoveEventHandler<T>(object sender, MouseEventArgs e, string key,
            string requiredPropertyName = "Id") where T : Ranked, new()
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ListBox listBox = sender as ListBox;
                ListBoxItem listBoxItem =
                    FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
                T t;
                if (listBoxItem != null)
                {
                    t = (T)listBox.ItemContainerGenerator.ItemFromContainer(listBoxItem);
                    if (t == null || t.GetType().GetProperty(requiredPropertyName).GetValue(t) == null)
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
            ListBoxItem listBoxItem = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
            if (listBoxItem != null)
            {
                T t = (T)listBox.ItemContainerGenerator.ItemFromContainer(listBoxItem);
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

        public void FreeCategoriesList_TouchDown(object sender, TouchEventArgs e)
        {
            var key = "FreeCategory";
            ListTouchDownEventHandler<Category>(sender, e, key);
        }

        public void ProductsList_TouchDown(object sender, TouchEventArgs e)
        {
            var key = "Product";
            ListTouchDownEventHandler<Product>(sender, e, key);
        }

        public void CategoriesList_TouchDown(object sender, TouchEventArgs e)
        {
            var key = "Category";
            ListTouchDownEventHandler<Category>(sender, e, key);
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
                    return (T)current;
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
                    FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                if (listBoxItem == null)
                {
                    return;
                }

                // Find the data behind the ListViewItem
                Product targetProduct = (Product)listView.ItemContainerGenerator.ItemFromContainer(listBoxItem);
           
                Product receivedProduct;

                if (e.Data.GetDataPresent("FreeProduct"))
                {
                    receivedProduct = e.Data.GetData("FreeProduct") as Product;
                }
                else
                {
                    receivedProduct = e.Data.GetData("Product") as Product;
                }

                if (receivedProduct?.Id == null) return;

                if (receivedProduct.Rank == null)
                {
                    SelectedFreeProduct = receivedProduct;
                    SelectedProduct = targetProduct;
                    AttachProductToCategory();
                }
                else
                {
                    //SelectedProduct = targetProduct;
                    if (targetProduct == receivedProduct) return;
                    
                    var targetRank = targetProduct.Rank;
                    var receivedRank = receivedProduct.Rank;
                    targetProduct.Rank = receivedProduct.Rank;
                    receivedProduct.Rank = targetRank;
                    CurrentProducts[(int)targetRank - 1] = receivedProduct;
                    CurrentProducts[(int)receivedRank - 1] = targetProduct;
                    
                    try
                    {
                        StateManager.Save(receivedProduct);
                        if (targetProduct.Id!=null)
                        {
                            StateManager.Save(targetProduct); 
                        }

                       
                    }
                    catch (AggregateException)
                    {
                        ToastNotification.Notify("Problem connecting to server");
                    }

                    SelectedProduct = null;
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
                ListBoxItem listBoxItem = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                var productSrc = e.Data.GetData("Product") as Product;
                if (productSrc == null) return;
                if (string.IsNullOrEmpty(productSrc.Name))
                    return;
                if (productSrc.Rank == null)
                {
                    return;
                }

                SelectedProduct = productSrc;
                RemoveProductFromCategory(null);
            }
        }

        public void CategoriesList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("FreeCategory") ||
                e.Data.GetDataPresent("Category"))
            {
                ListBox listView = sender as ListBox;
                ListBoxItem listBoxItem =
                    FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                if (listBoxItem == null)
                {
                    return;
                }

                // Find the data behind the ListViewItem
                Category targetCategory = (Category)listView.ItemContainerGenerator.ItemFromContainer(listBoxItem);

                Category incomingCategory;

                if (e.Data.GetDataPresent("FreeCategory"))
                {
                    incomingCategory = e.Data.GetData("FreeCategory") as Category;
                }
                else
                {
                    incomingCategory = e.Data.GetData("Category") as Category;
                }

                if (incomingCategory?.Id == null) return;

                if (incomingCategory.Rank == null)
                {
                    SelectedFreeCategory = incomingCategory;
                    SelectedCategory = targetCategory;
                    AttachCategoryToList();
                }
                else
                {
                    CategoryToMove = incomingCategory;
                    SelectedCategory = targetCategory;
                    if (SelectedCategory == CategoryToMove)
                    {
                        CategoryToMove = null;
                        return;
                    }
                   
                    var index = CurrentCategories.IndexOf(CategoryToMove);
                    var cat = new Category() { Rank = CategoryToMove.Rank };
                    if (SelectedCategory.Equals(CategoryToMove))
                    {
                        CategoryToMove = null;
                        return;
                    }

                    //PutCategoryInCellOf(SelectedCategory, CategoryToMove);
                    IList<Category> categories = CurrentCategories;
                    try
                    {
                        RankedItemsCollectionHelper.InsertTElementInPositionOf(ref incomingCategory, ref targetCategory,
                            categories);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        throw;
#endif
                        NLog.LogManager.GetCurrentClassLogger().Error(ex);
                    }

                    try
                    {
                        if (targetCategory.Id != null)
                        {
                            StateManager.Save(targetCategory);
                        }

                        StateManager.Save(incomingCategory);
                        CurrentCategories = (BindableCollection<Category>)categories;
                    }
                    catch (AggregateException)
                    {
                        ToastNotification.Notify("Problem connecting to server");
                    }
                    CategoryToMove = null;
                    
                    ShowCategoryProducts(null);
                    //CurrentCategories[index] = cat;
                    //CategoryToMove = null;
                }
            }
        }

        private void AttachCategoryToList()
        {
            if (SelectedFreeCategory == null || SelectedCategory == null)
            {
                ToastNotification.Notify("Select Both free product and category product",NotificationType.Warning);
                return;
            }

            if (SelectedCategory.Id != null)
            {
                ToastNotification.Notify("Must select an Empty Cell to drop Free Category", NotificationType.Warning);
                return;
            }

            //PutCategoryInCellOf(SelectedCategory, SelectedFreeCategory);
            Category incomingCategory = SelectedFreeCategory;
            Category targetCategory = SelectedCategory;
            IList<Category> categories = CurrentCategories;
            RankedItemsCollectionHelper.InsertTElementInPositionOf(ref incomingCategory, ref targetCategory,
                categories);

            try
            {

                if (targetCategory?.Id != null)
                {
                    StateManager.Save(targetCategory);
                }

                incomingCategory.ProductIds = new List<long>();
                incomingCategory.Products = new List<Product>();
                StateManager.Save(incomingCategory);
       
            }
            catch (AggregateException)
            {
                ToastNotification.Notify("Problem connecting to server");
            }
            CurrentCategories = (BindableCollection<Category>)categories;

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
                    FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

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
                //RemoveCategoryFromList(SelectedCategory);
                RemoveSelectedCategoryFromList(SelectedCategory);
            }
        }

        public void EditProduct(object obj)
        {
            if (SelectedProduct?.Id == null)
            {
                ToastNotification.Notify("There is no Product to edit, select a valid cell!");
                return;
            }


            if (SelectedCategory.Id == null || SelectedProduct == null) return;
            ProductDetailViewModel = new ProductDetailViewModel(ref _selectedProduct);
            ProductDetailViewModel.ErrorsChanged += EditProductViewModel_ErrorsChanged;
                var parent = Parent as MainViewModel;
            parent?.OpenDialog(ProductDetailViewModel)
                .OnClose(() =>
                {
                    if (ProductDetailViewModel != null)
                    {
                        ProductDetailViewModel.ErrorsChanged -= EditProductViewModel_ErrorsChanged;
                        ProductDetailViewModel = null; 
                    }
                });
            NotifyOfPropertyChange((() => IsCategory));
           
        }

        public void CreateProduct(object obj)
        {

            if (SelectedProduct== null|| SelectedProduct?.Id!= null)
            {
                ToastNotification.Notify("Select an empty cell to create a new Product");
                return;
            }

            if (SelectedCategory?.Id == null) return;
            
            ProductDetailViewModel = new ProductDetailViewModel(ref _selectedProduct);
            ProductDetailViewModel.ErrorsChanged += EditProductViewModel_ErrorsChanged;
            var parent = (Parent as MainViewModel);
            parent?.OpenDialog(ProductDetailViewModel)
                .OnClose(() =>
                {
                    if (ProductDetailViewModel!= null)
                    {
                        ProductDetailViewModel.ErrorsChanged -= EditProductViewModel_ErrorsChanged;
                        ProductDetailViewModel = null;
                    }
                });
        }

        public void EditCategory(object obj)
        {
            EditCategory();
        }

        public void EditCategory(bool callFromCreate = false)
        {
            if (!callFromCreate&& SelectedCategory?.Id == null)
            {
                ToastNotification.Notify("There is no category to edit, select a valid cell!");
                return;
            }
            IsCategory = true;
            CategoryDetailViewModel = new CategoryDetailViewModel(ref _selectedCategory,_allCategories);
            (Parent as MainViewModel)?.OpenDialog(CategoryDetailViewModel).OnClose(() =>
            {
                var category = SelectedCategory;
                SelectedCategory = null;
                ShowCategoryProducts(category);
            });
            //CategoryDetailViewModel.ErrorsChanged += EditCategoryViewModel_ErrorsChanged;


        }

        public void CreateCategory(object obj) 
        {
            CreateCategory();
        }


        public void CreateCategory()
        {
            if (SelectedCategory== null)
            {
                return;
            }

            if (SelectedCategory.Id != null)
            {
                ToastNotification.Notify("Select an empty cell to create a new Category");
                return;
            }
            EditCategory(true);
        }

        public void AddCategoryRow(object obj)
        {
            var baseRank = CategoryPageSize * _numberOfCategoryRows + 1;
            _numberOfCategoryRows++;
            var maxRank = CategoryPageSize * _numberOfCategoryRows;
            for (var i = baseRank; i <= maxRank; i++)
            {
                CurrentCategories.Add(new Category(){Rank = i});
            }
        }

        public void SavedProduct()
        {
            ProductDetailViewModel = null;
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

        public void ConfigureProductDisplayLayout(object obj)
        {
            
            (Parent as MainViewModel)?.OpenDialog(ProductLayoutViewModel);
        }

        public void ApplyLayoutConfig()
        {
            ProductLayoutViewModel.Apply();
        }

        private void Configuration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ProductLayoutConfiguration.NumberOfProducts))
            {
                if (ProductLayoutViewModel.Configuration.NumberOfProducts>0)
                {
                    _productPageSize = ProductLayoutViewModel.Configuration.NumberOfProducts; 
                    ShowCategoryProducts(SelectedCategory);
                }
            }
        }

        public event EventHandler<SettingsUpdatedEventArgs> SettingsUpdated;

        public void RaiseSettingsUpdated()
        {
            
            SettingsUpdated?.Invoke(this,new SettingsUpdatedEventArgs(_allProducts,_allCategories,ProductLayoutViewModel?.Configuration));
        }

        public override void BeforeNavigateAway()
        {
            RaiseSettingsUpdated();
        }

        public ICommand CreateProductCommand { get; set; }
        public ICommand EditProductCommand { get; set; }
        public ICommand MoveProductCommand { get; set; }
        public ICommand PasteProductCommand { get; set; }
        public ICommand CopyProductCommand { get; set; }
        public ICommand RemoveProductCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }

        public ICommand CreateCategoryCommand { get; set; }
        public ICommand EditCategoryCommand { get; set; }
        public ICommand MoveCategoryCommand { get; set; }
        public ICommand PasteCategoryCommand { get; set; }
        public ICommand CopyCategoryCommand { get; set; }
        public ICommand RemoveCategoryCommand { get; set; }
        public ICommand DeleteCategoryCommand { get; set; }

        public ICommand ConfigureProductLayoutCommand { get; set; }
        public ICommand AddCategoryRowCommand { get; set; }
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