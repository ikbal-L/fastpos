using Caliburn.Micro;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.ViewModels
{
    class PaginationTestViewModel : Screen
    {
        private IProductService productService;
        private ICategoryService categorieService;
        private IOrderService orderService;

        public PaginationTestViewModel(IProductService productService, ICategoryService categorieService, IOrderService orderService)
        {
            this.productService = productService;
            this.categorieService = categorieService;
            this.orderService = orderService;
            int getProductsStatusCode = 0;
            var products = productService.GetAllProducts(ref getProductsStatusCode);
            Products = new BindableCollection<Product>(products);
        }

        public BindableCollection<Product> Products { get; set; }

        public void Close()
        {
            LoginViewModel loginvm = new LoginViewModel();
            loginvm.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(loginvm);
        }
    }
}
