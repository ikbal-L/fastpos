using Caliburn.Micro;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;

namespace PosTest.ViewModels
{
    class PaginationTestViewModel : Screen
    {
        private IProductService productService;
        private ICategoryService categorieService;
        private IOrderService orderService;
        private int _productPageSize;

        public PaginationTestViewModel(/*IProductService productService, ICategoryService categorieService, IOrderService orderService*/)
        {
            //this.productService = productService;
            //this.categorieService = categorieService;
            //this.orderService = orderService;
            int getProductsStatusCode = 0;
            //var products = productService.GetAllProducts(ref getProductsStatusCode);
            var products = StateManager.Get<Product,long>();
            Products = new BindableCollection<Product>(products);
            ProductPageSize = 6;
        }

        public BindableCollection<Product> Products { get; set; }
        public int ProductPageSize
        {
            get => _productPageSize;
            set
            {
                Set(ref _productPageSize, value);
            }
        }
        public void Close()
        {
            LoginViewModel loginvm = new LoginViewModel();
            loginvm.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(loginvm);
        }
    }
}
