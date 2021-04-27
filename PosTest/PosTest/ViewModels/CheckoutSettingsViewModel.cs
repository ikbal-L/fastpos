using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using NLog;
using PosTest.Extensions;
using PosTest.Helpers;
using PosTest.ViewModels.Settings;
using PosTest.ViewModels.SubViewModel;
using PosTest.Views;
using ServiceInterface.ExtentionsMethod;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;

namespace PosTest.ViewModels
{
    public class CheckoutSettingsViewModel : SettingsItemBase
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
        private EditCategoryViewModel _editCategoryViewModel;
        private bool _isCategory;
        private EditProductViewModel _editProductViewModel;
        private Type _activeTab;
        private ProductLayoutViewModel _productLayout;
        private bool _isDialogOpen;

        

        public CheckoutSettingsViewModel()
        {
            IsDialogOpen = false;
            ProductLayout = new ProductLayoutViewModel();
            var Manager = new SettingsManager<ProductLayoutConfiguration>("product.layout.config");
            var setting = Manager.LoadSettings();
            var pageSize = setting.NumberOfProducts;
            
            this.Title = "Checkout";
            this.Content = new CheckoutSettingsView() {DataContext= this};
            ProductPageSize = pageSize;
            CategoryPageSize = 6;


            
            StateManager.Fetch();
            var products = StateManager.Get<Product>();
            var categories = StateManager.Get<Category>();

            StateManager.Associate<Additive, Product>();
            StateManager.Associate<Product, Category>();

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
            SelectedCategory = CurrentCategories.FirstOrDefault();
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
                    if (EditCategoryViewModel != null)
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
                    { return !EditProductViewModel.HasErrors;
                    }
                    else
                    {
                        return true;
                    }
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

        public ProductLayoutViewModel ProductLayout
        {
            get => _productLayout;
            set => Set(ref _productLayout, value);
        }

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
            var maxRank = (int)categories.Max(c => c.Rank);
            int nbpage = (maxRank / _categpryPageSize) + (maxRank % _categpryPageSize == 0 ? 0 : 1);
            nbpage = nbpage == 0 ? 1 : nbpage;
            var size = nbpage * _categpryPageSize;

            RankedItemsCollectionHelper.LoadPagesFilled(source: categories, target: CurrentCategories, size: size);
        }

        public void ShowCategoryProducts(Category category)
        {
            //if ( category == SelectedCategory) return;

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
                SelectedFreeCategory = null;
                var index = CurrentProducts.IndexOf(ProductToMove);
                var prod = new Product { Rank = ProductToMove.Rank };
                if (SelectedProduct.Equals(ProductToMove))
                {
                    ProductToMove = null;
                    return;
                }

                //PutProductInCellOf(SelectedProduct, ProductToMove);
                var incomingProduct = ProductToMove;
                var targetProduct = SelectedProduct;
                IList<Product> products = CurrentProducts;
                RankedItemsCollectionHelper.InsertTElementInPositionOf(ref incomingProduct,ref targetProduct,  products);


                if (targetProduct?.Id != null)
                {
                    StateManager.Save(targetProduct);
                }

                StateManager.Save(incomingProduct);
                //CurrentProducts[index] = prod;
                SelectedProduct = null;
                ProductToMove = null;
                return;
            }

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
                EditCategoryViewModel = null;
            }
            else
            {
                EditProductViewModel = null;
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


                RemoveCategoryFromList<T>(selectedCategory);
            }

            CurrentTs[index] = new T { Rank = rank };
            FreeTs.Add(freeT);
            SelectedFreeT = freeT;
        }

        private void RemoveCategoryFromList<T>(Category selectedCategory) where T : Ranked, new()
        {
            ManageCategoryProductsForDeletion<T>(selectedCategory);


            CurrentProducts.Clear();
            SelectedCategory = null;


            if (StateManager.Save(selectedCategory))
            {
                ToastNotification.Notify("Category was Save successfully ",NotificationType.Success);
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
                StateManager.Save<Product>(selectedProduct);
            }
            catch (AggregateException)
            {
                ToastNotification.Notify("Problem connecting to server");
            }

        }

        private void ManageCategoryProductsForDeletion<T>(Category selectedCategory) where T : Ranked, new()
        {
            if (selectedCategory.Products != null || selectedCategory.Products?.Count > 0)
            {

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
                
                if (StateManager.Save<Product>(selectedCategory.Products))
                {
                    selectedCategory.Products= null;
                    selectedCategory.ProductIds = null;
                }

            }

            
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
            FreeProducts.Remove(SelectedFreeProduct);
        }

        public void CopyProduct()
        {
            if (SelectedProduct.Category == null)
            {
                ToastNotification.Notify("We can't copy empty product",NotificationType.Warning);
            }

            ClipboardProduct = SelectedProduct;
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

                StateManager.Save<Product>(targetProduct);
            }

            CurrentProducts[index] = sourceProduct;

            StateManager.Save<Product>(sourceProduct);

            SelectedCategory.ProductIds.Add((long)sourceProduct.Id);
            SelectedCategory.Products.Add(sourceProduct);
            StateManager.Save<Category>(SelectedCategory);

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

        public void PasteProduct()
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
            FreeProducts.Add(SelectedFreeProduct = new Product());
        }

        public void CopyFreeProduct()
        {
            if (SelectedFreeProduct == null)
            {
                return;
            }

            //   var product = new Product(SelectedFreeProduct);
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
                        ToastNotification.Notify($"{SelectedFreeCategory.Name} {SelectedFreeCategory.GetType().Name}",NotificationType.Success);
                    }
                    
                }
                catch (AggregateException)
                {
                    ToastNotification.Notify("Problem connecting to server");
                }
                
                AllCategories.Remove(SelectedFreeCategory);
                FreeCategories.Remove(SelectedFreeCategory);
                SelectedFreeCategory = null;
            }
        }

        public void DeleteProduct()
        {
            if (SelectedProduct != null && SelectedFreeProduct == null)
            {
                Notify("Product must be unassigned from category to be deleted");
            }

            if (SelectedFreeProduct?.Id != null)
            {
                StateManager.Delete(SelectedFreeProduct);


                AllProducts.Remove(SelectedFreeProduct);
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
                    FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
                T t;
                if (listBoxItem != null)
                {
                    t = (T)listBox.ItemContainerGenerator.ItemFromContainer(listBoxItem);
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
                    SelectedProduct = targetProduct;
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
                    SelectedCategory = incomingCategory;
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
                RemoveCategoryFromList();
            }
        }

        public void EditProduct()
        {
            /*  if (SelectedProduct.Id == null)
            {
                ToastNotification.Notify("There is no Product to edit, select a valid cell!");
                return;
            }*/

            IsCategory = false;
            if (SelectedCategory.Id == null || SelectedProduct == null) return;
            this.EditProductViewModel = new EditProductViewModel(ref this._selectedProduct);
            EditProductViewModel.ErrorsChanged += EditProductViewModel_ErrorsChanged;
            
            NotifyOfPropertyChange((() => IsCategory));
            IsFlipped = true;
        }

        public void CreateProduct()
        {
            if (SelectedProduct == null)
            {
                return;
            }

            if (SelectedProduct.Id!= null)
            {
                ToastNotification.Notify("Select an empty cell to create a new Product");
                return;
            }
            EditProduct();
        }

        public void EditCategory(bool callFromCreate = false)
        {
            if (!callFromCreate&& SelectedCategory.Id == null)
            {
                ToastNotification.Notify("There is no category to edit, select a valid cell!");
                return;
            }
            IsCategory = true;
            this.EditCategoryViewModel = new EditCategoryViewModel(ref this._selectedCategory);
            EditCategoryViewModel.ErrorsChanged += EditCategoryViewModel_ErrorsChanged;
            
            IsFlipped = true;
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

        public void ConfigureProductDisplayLayout()
        {
            IsDialogOpen = true;
            ProductLayout.Configuration.PropertyChanged += Configuration_PropertyChanged;
        }

        public void ApplyLayoutConfig()
        {
            ProductLayout.Apply();
        }

        private void Configuration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ProductLayoutConfiguration.NumberOfProducts))
            {
                if (ProductLayout.Configuration.NumberOfProducts>0)
                {
                    _productPageSize = ProductLayout.Configuration.NumberOfProducts; 
                    ShowCategoryProducts(SelectedCategory);
                }
            }
        }
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