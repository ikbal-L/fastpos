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
        public IProductService productService;

        public ICollectionView Products { get; set; }

        private Product _currentProduct;
        private int _pageSize;
        private IProductService _productsService;

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
            //Products = new BindableCollection<Product>();
            //CurrentProduct = new Product();
        }

        public ProductsViewModel(int pageSize, IProductService productsService)
        {
            _pageSize = pageSize;
            _productsService = productsService;
            Products = CollectionViewSource.GetDefaultView(_productsService.GetAllProducts());
        }

        public void DeleteCommand()
        {
            if (_currentProduct == null)
                return;
            _productsService.DeleteProduct(_currentProduct.Id);
        }     
            public void EditProduct()
        {
            //var newDialogViewModel = new NewDialogViewModel();
            //bool? result = this.windowManager.ShowDialog(newDialogViewModel);
        }

    }
}
