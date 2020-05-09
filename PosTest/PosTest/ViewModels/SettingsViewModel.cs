using Caliburn.Micro;
using ServiceInterface.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.ViewModels
{
    public class SettingsViewModel : Screen
    {
        private int _pageSize;
        private IProductService _productService;
        

        public Screen CurrentView { get; set; }
        public SettingsViewModel()
        {
            CurrentView = new ProductsViewModel();
        }

        public SettingsViewModel(int pageSize, IProductService productService)
        {
            this._pageSize = pageSize;
            this._productService = productService;
            CurrentView = new ProductsViewModel(_pageSize, _productService);
        }
    }
}
