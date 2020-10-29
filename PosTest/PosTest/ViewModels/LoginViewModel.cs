using Caliburn.Micro;
using ServiceInterface.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ServiceInterface.Model;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel;
using System.Windows.Data;
using System.Net.Http;
using ServiceInterface.StaticValues;
using PosTest.ViewModels.SubViewModel;
using ServiceLib.Service;
using PosTest.Helpers;

namespace PosTest.ViewModels
{
    public class LoginViewModel : Screen
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [Import(typeof(IProductService))]
        private IProductService productService = null;

        [Import(typeof(ICategoryService))]
        private ICategoryService categorieService = null;
                
        [Import(typeof(IAdditiveService))]
        private IAdditiveService additiveService = null;

        [Import(typeof(IAuthentification))]
        private IAuthentification authService = null;

        [Import(typeof(IOrderService))]
        private IOrderService orderService;

        [Import(typeof(IDelivereyService))]
        private IDelivereyService delivereyService;

        [Import(typeof(IWaiterService))]
        private IWaiterService waiterService;

        [Import(typeof(ICustomerService))]
        private ICustomerService customerService;

        //public String Username { get; set; }
        //public String Password { get; set; }

        private bool _IsDialogOpen;

        public bool IsDialogOpen
        {
            get => _IsDialogOpen;
            set => Set(ref _IsDialogOpen, value);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            this.Compose();
        }
        public bool CanLogin()
        {
            //logger.Info("User: " + username + "  Pass: " + password);
            return true;// !String.IsNullOrEmpty(username)/* && !String.IsNullOrEmpty(password)*/;
        }

        public void Login()
        {
            int resp;
            try
            {
                resp = authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id = 1 });
            }
            catch (AggregateException)
            {
                ToastNotification.Notify("Check your server connection");
                return;
            }

            IsDialogOpen = true;

            //CheckoutViewModel checkoutViewModel =
            //    new CheckoutViewModel(ActionConfig.NumberOfProductsPerPage,
            //    productService,
            //    categorieService,
            //    orderService,
            //    waiterService,
            //    delivereyService,
            //    customerService
            //    );

            //checkoutViewModel.Parent = this.Parent;
            //(this.Parent as Conductor<object>).ActivateItem(checkoutViewModel);
        }
        
        public void Checkout()
        {
            IsDialogOpen = false;
            CheckoutViewModel checkoutViewModel =
                new CheckoutViewModel(ActionConfig.NumberOfProductsPerPage,
                productService,
                categorieService,
                orderService,
                waiterService,
                delivereyService,
                customerService
                );

            checkoutViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(checkoutViewModel);
        }

        public void Settings()
        {
            authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id = 1 });
            Settings1ViewModel settingsViewModel = new Settings1ViewModel(30, productService, categorieService, additiveService);
            settingsViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(settingsViewModel);
        }

        public void close()
        {
            Application.Current.MainWindow.Close();
        }

        public void PaginationTest()
        {
            int resp;
            try
            {
                resp = authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id = 1 });
            }
            catch (AggregateException)
            {
                ToastNotification.Notify("Check your server connection");
                return;
            }

            PaginationTestViewModel paginationtesttViewModel =
                new PaginationTestViewModel(
                        productService,
                        categorieService, 
                        orderService);

            paginationtesttViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(paginationtesttViewModel);
        }

        public void LoginWithPin()
        {
            IsDialogOpen = false;
            PinLoginViewModel pinLoginViewModel =
                new PinLoginViewModel(
                    productService,
                    categorieService,
                    orderService,
                    waiterService,
                    delivereyService,
                    customerService
                );
            
            pinLoginViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(pinLoginViewModel);
        }
        public void CheckoutSettings()
        {
            IsDialogOpen = false;
            int resp;
            try
            {
                resp = authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id = 1 });
            }
            catch (AggregateException)
            {
                ToastNotification.Notify("Check your server connection");
                return;
            }

            CheckoutSettingsViewModel checkoutSettingsViewModel =
                    new CheckoutSettingsViewModel(30, 8, productService,
                categorieService);
            checkoutSettingsViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(checkoutSettingsViewModel);
        }  
        
        public void CustomersSettings()
        {
            CustomerViewModel customerViewModel = new CustomerViewModel(null);
            customerViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(customerViewModel);
        }  
        
        public void AdditivesSettings()
        {
            authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id = 1 });
            additiveService = new AdditiveService();
            AdditivesSettingsViewModel additivesSettingsViewModel = new AdditivesSettingsViewModel(additiveService,30);
            additivesSettingsViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(additivesSettingsViewModel);
        }
        public void AdditivesOfProduct()
        {
            AdditivesOfProductViewModel AdditivesOfProduct = new AdditivesOfProductViewModel ();
            AdditivesOfProduct.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(AdditivesOfProduct);
        }

        //This method load th DLL file containing the implemetation of IProductService 
        // and satisfay the import in this class
        private void Compose()
        {
            //AssemblyCatalog catalog1 = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
            AssemblyCatalog catalog2 = new AssemblyCatalog("ServiceLib.dll");
            CompositionContainer container = new CompositionContainer(catalog2);
            container.SatisfyImportsOnce(this);
        }
    }
}
