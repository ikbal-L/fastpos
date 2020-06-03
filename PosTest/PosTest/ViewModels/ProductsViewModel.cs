using Caliburn.Micro;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PosTest.ViewModels
{
    public class ProductsViewModel : Screen
    {
        public bool ListViewSwitcher { get; set; }

        public int pageSize;
        

        public BindableCollection<Product> Products { get; set; }
        public BindableCollection<Category> Categories { get; set; }
        public BindableCollection<Additive> Additives { get; set; }

        public BindableCollection<Product> ProductsCanCompositIngredients { get; set; }

        private Product _currentProduct;
        private int _pageSize;
        private IProductService _productsService;
        private ICategoryService _categorieService;
        private IAdditiveService _additiveService;

        private bool _IsDialogOpen;
        public bool IsDialogOpen
        {
            get => _IsDialogOpen;
            set => Set(ref _IsDialogOpen, value);
        }
        public Product CurrentProduct
        {
            get { return _currentProduct; }
            set
            {
                _currentProduct = value;
                NotifyOfPropertyChange(() => CurrentProduct);

            }
        }

        public ProductsViewModel()
        {
        }

        public ProductsViewModel(int pageSize, 
            IProductService productsService, 
            ICategoryService categorieService,
            IAdditiveService additiveService
            
            )
        {
            _pageSize = pageSize;
            _productsService = productsService;
            _categorieService = categorieService;
            _additiveService = additiveService;

            //Products = BindableCollection.GetDefaultView(_productsService.GetAllProducts());
            Products = new BindableCollection<Product>( _productsService.GetAllProducts());
            ProductsCanCompositIngredients = new BindableCollection<Product>(Products.Where(p=>p.IsPlatter==false));
            Categories = new BindableCollection<Category>(_categorieService.GetAllCategories());

            //new BindableCollection<Category>(_categorieService.GetAllCategories());
            Additives = new BindableCollection<Additive>();
            Additives.Add(_additiveService.GetAdditive(1));

            CurrentProduct = Products.Cast<Product>().FirstOrDefault();

            
        }

        public long GetNewProductId()
        {
            long _nextId = 1;
            if (Products.Count != 0)
                _nextId = Products.Max(p => p.Id) + 1;
            return _nextId;
        }

        public void DeleteCommand()
        {
            if (_currentProduct == null)
                return;
            _productsService.DeleteProduct(_currentProduct.Id);
        }     
        public void NewCommand()
        {
            CurrentProduct = new Product();
            Products.Add(CurrentProduct);
            IsDialogOpen = true;
        }

        public void SaveCommand()
        {
            if (CurrentProduct.Id <= 0)
            {
                var _nextId = Products.Count == 0 ? 0 : Products.Max(f => f.Id);
                CurrentProduct.Id = _nextId + 1 ;
                // Insert Product
                _productsService.SaveProduct(CurrentProduct);
            }
            else
            {
                _productsService.UpdateProduct(CurrentProduct);
            }
            IsDialogOpen = false;
        }

        public void CancelCommand()
        {
            if (CurrentProduct != null)
            {
                //verify thqt CurrentProduct is the nez one
                if (CurrentProduct.Id == 0)
                    Products.Remove(CurrentProduct);
            }
            IsDialogOpen = false;
        }
    }
}
