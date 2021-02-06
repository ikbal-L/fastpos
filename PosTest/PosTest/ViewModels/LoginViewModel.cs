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
using PosTest.ViewModels.Settings;
using PosTest.ViewModels.DeliveryAccounting;
using System.Threading;
using System.Security.Principal;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace PosTest.ViewModels
{
    public class LoginViewModel : Screen
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [Import(typeof(IProductRepository))]
        private IProductRepository _productRepository= null;

        [Import(typeof(ICategoryRepository))]
        private ICategoryRepository _categoryRepository =  null;
                
        [Import(typeof(IAdditiveRepository))]
        private IAdditiveRepository _additiveRepository = null;

        [Import(typeof(IAuthentification))]
        private IAuthentification _authService = null;

        [Import(typeof(IOrderRepository))]
        private IOrderRepository _orderRepository;

        [Import(typeof(ITableRepository))]
        private ITableRepository _tableRepository;

        [Import(typeof(IDeliverymanRepository))]
        private IDeliverymanRepository _deliverymanRepository;

        [Import(typeof(IWaiterRepository))]
        private IWaiterRepository _waiterRepository;

        [Import(typeof(ICustomerRepository))]
        private ICustomerRepository _customerRepository;
        [Import(typeof(IPaymentRepository))]
        private IPaymentRepository _paymentRepository;

        private ObservableCollection<User> _Users;

        public ObservableCollection<User> Users
        {
            get { return _Users; }
            set { _Users = value;
                NotifyOfPropertyChange(nameof(Users));
            }
        }
        private User _selectedUser;

        public User SelectedUser
        {
            get { return _selectedUser; }
            set { _selectedUser = value;
                NotifyOfPropertyChange(nameof(SelectedUser));
            }
        }

        //public String Username { get; set; }
        //public String Password { get; set; }

        private bool _IsDialogOpen;

        public bool IsDialogOpen
        {
            get => _IsDialogOpen;
            set => Set(ref _IsDialogOpen, value);
        }

        private string _Password;

        public string Password
        {
            get { return _Password; }
            set { _Password = value;
                NotifyOfPropertyChange(nameof(Password));
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            this.Compose();
            StateManager.Instance
                .Manage(_productRepository)
                .Manage(_categoryRepository)
                .Manage(_additiveRepository)
                .Manage(_orderRepository, fetch: false)
                .Manage(_tableRepository)
                .Manage(_customerRepository)
                .Manage(_waiterRepository)
                .Manage(_deliverymanRepository)
                .Manage(_paymentRepository);
            
            List<string> auths = new List<string>();
            auths.Add("Read_Product");
            auths.Add("Can_login");
            var principal = new GenericPrincipal(new GenericIdentity("UserTest", ""), auths.ToArray());
            Thread.CurrentPrincipal = principal;
            Users = new ObservableCollection<User>();
            Users.Add(new User() { Username = "admin", Password = "admin" });
            Users.Add(new User() { Username = "Elahbib ", Password = "admin", BackgroundColor = Color.FromRgb(66, 114, 192) });
            Users.Add(new User() { Username = "Othman ", Password = "admin", BackgroundColor = Color.FromRgb(23, 43, 77) });
            Users.Add(new User() { Username = "Djaber ", Password = "admin", BackgroundColor = Color.FromRgb(112, 192, 232) });


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
                // resp = authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id = 1 });
                resp = _authService.Authenticate("admin", "admin", new Annex { Id = 1 }, new Terminal { Id = 1 });
                // resp = authService.Authenticate(new User(){Username = "admin",Password = "admin",Agent = Agent.Desktop});
                int statusCode = 0;
                
               

            }
            catch (AggregateException)
            {
                //ToastNotification.Notify("Check your server connection");
                ToastNotification.Notify(NotificationHelper.CHECK_SERVER_CONNECTION);
                return;
            }

            IsDialogOpen = true;

          
            Checkout();
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
        public void NumericKeyboard(PasswordBox passwordBox,string key)
        {
            if(!key.Equals("-"))
                passwordBox.Password += key;
            else
                passwordBox.Password = passwordBox.Password.Substring(0, passwordBox.Password.Length==0?passwordBox.Password.Length: passwordBox.Password.Length - 1);
        }

        public void Checkout()
        {
            IsDialogOpen = false;
            var  Manager = new SettingsManager<ProductLayoutConfiguration>("product.layout.config");
            var setting = Manager.LoadSettings();
            if (setting == null)
            {
             setting = new ProductLayoutConfiguration(){Rows = 5,Columns = 6};  
             Manager.SaveSettings(setting);
            }
            var pageSize = setting.NumberOfProducts;
            CheckoutViewModel checkoutViewModel =
                new CheckoutViewModel(pageSize
                //,_productService,
                //_categorieService,
                //_orderRepository,
                //_waiterService,
                //_deliverymanRepository,
                //_customerService
                );
            //resp = authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id = 1 });
           
            checkoutViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(checkoutViewModel);
        }

        public void Settings()
        {
            _authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id = 1 });
            Settings1ViewModel settingsViewModel = new Settings1ViewModel(30/*, _productService, _categorieService, _additiveService*/);
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
                resp = _authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id = 1 });
            }
            catch (AggregateException)
            {
                ToastNotification.Notify("Check your server connection");
                return;
            }

            PaginationTestViewModel paginationtesttViewModel =
                new PaginationTestViewModel(
                        //_productService,
                        //_categorieService, 
                        //_orderRepository
                        );

            paginationtesttViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(paginationtesttViewModel);
        }

        public void LoginWithPin()
        {
            IsDialogOpen = false;
            PinLoginViewModel pinLoginViewModel =
                new PinLoginViewModel(
                    //_productService,
                    //_categorieService,
                    //_orderRepository,
                    //_waiterService,
                    //_deliverymanRepository,
                    //_customerService
                );
            
            pinLoginViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(pinLoginViewModel);
        }
        public void CheckoutSettings()
        {
        /*    IsDialogOpen = false;
            int resp;
            try
            {
                //resp = authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id = 1 });
                resp = _authService.Authenticate("admin", "admin", new Annex { Id = 1 }, new Terminal { Id = 1 });
            }
            catch (AggregateException)
            {
                ToastNotification.Notify("Check your server connection");
                return;
            }

            CheckoutSettingsViewModel checkoutSettingsViewModel =
                    new CheckoutSettingsViewModel(30, 8
                //        , _productService,
                //_categorieService
                      );
            checkoutSettingsViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(checkoutSettingsViewModel);*/
        }  
        
        public void CustomersSettings()
        {
            CustomerViewModel customerViewModel = new CustomerViewModel(null/*,_customerService*/);
            customerViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(customerViewModel);
        }  
        
        public void AdditivesSettings()
        {
            // authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id = 1 });
         //   _authService.Authenticate("admin", "admin", new Annex { Id = 1 }, new Terminal { Id = 1 });
            //_additiveService = new AdditiveService();
         /*   AdditivesSettingsViewModel additivesSettingsViewModel = new AdditivesSettingsViewModel(/*_additiveService,*///30);
            //additivesSettingsViewModel.Parent = this.Parent;
        //    (this.Parent as Conductor<object>).ActivateItem(additivesSettingsViewModel);
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

        public void   Settingsbtn() {
            int resp;
            try
            {
                //resp = authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id = 1 });
                resp = _authService.Authenticate("admin", "admin", new Annex { Id = 1 }, new Terminal { Id = 1 });
            }
            catch (AggregateException)
            {
                ToastNotification.Notify("Check your server connection");
                return;
            }
            //     SettingsViewModel settingsViewModel = new SettingsViewModel();
            DeliveryAccountingViewModel viewModel = new DeliveryAccountingViewModel();
             viewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(viewModel);
        }
    }
}
