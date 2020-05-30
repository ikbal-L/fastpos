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
        private ICategoryService _categorieService;


        public Screen CurrentView { get; set; }
        public SettingsViewModel()
        {
            CurrentView = new ProductsViewModel();
        }

        public SettingsViewModel(int pageSize, IProductService productService, ICategoryService categorieService)
        {
            this._pageSize = pageSize;
            this._productService = productService;
            _categorieService = categorieService;
            CurrentView = new ProductsViewModel(_pageSize, _productService, _categorieService);
        }
    }
}
