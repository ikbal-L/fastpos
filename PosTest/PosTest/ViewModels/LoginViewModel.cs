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

        //public String Username { get; set; }
        //public String Password { get; set; }

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

            CheckoutViewModel checkoutViewModel = 
                new CheckoutViewModel(ActionConfig.NumberOfProductsPerPage, 
                productService, 
                categorieService, orderService, waiterService, delivereyService);

            checkoutViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(checkoutViewModel);
        }
        
        public void Settings()
        {
            authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id = 1 });
            SettingsViewModel settingsViewModel = new SettingsViewModel(30, productService, categorieService, additiveService);
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
