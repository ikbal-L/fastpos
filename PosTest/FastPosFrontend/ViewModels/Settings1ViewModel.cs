using Caliburn.Micro;
using ServiceInterface.Interface;

namespace FastPosFrontend.ViewModels
{
    public class Settings1ViewModel : Screen
    {
        private int _pageSize;
        private IProductService _productService;
        private ICategoryService _categorieService;
        private IAdditiveService _additiveService;
        private Screen _currentView;


        //public Screen CurrentView { get; set; }

        public Screen CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                NotifyOfPropertyChange(() => CurrentView);
            }
        }


        private bool _isLeftDrawerOpen;
        public bool IsLeftDrawerOpen
        {
            get => _isLeftDrawerOpen;
            set => Set(ref _isLeftDrawerOpen, value);
        }




        public Settings1ViewModel()
        {
            CurrentView = new ProductsViewModel();
        }

        public void CategoryTabCommand()
        {
            CurrentView = new CategoryViewModel(_pageSize, _productService, _categorieService);
            IsLeftDrawerOpen = false;

        }

        public void ProductsTabCommand()
        {
            CurrentView = new ProductsViewModel(_pageSize, _productService, _categorieService, _additiveService);
            IsLeftDrawerOpen = false;
        }

        public Settings1ViewModel(int pageSize/*, 
            IProductService productService, 
            ICategoryService categorieService,
            IAdditiveService additiveService*/
            )
        {
            _pageSize = pageSize;
            //this._productService = productService;
            //_categorieService = categorieService;
            //_additiveService = additiveService;

            ProductsTabCommand();
        }

        public void CloseCommand()
        {
            LoginViewModel loginvm = new LoginViewModel();
            loginvm.Parent = Parent;
            (Parent as Conductor<object>).ActivateItem(loginvm);
        }

    }
}
