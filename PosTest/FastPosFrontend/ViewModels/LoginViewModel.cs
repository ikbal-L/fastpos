using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using FastPosFrontend.Events;
using FastPosFrontend.Helpers;
using FastPosFrontend.SL.Controls;
using FastPosFrontend.ViewModels.DeliveryAccounting;
using FastPosFrontend.ViewModels.SubViewModel;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.helpers;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{
    [NavigationItemConfiguration("Login",target:typeof(LoginViewModel),NavigationItemLoadingStrategy.OnStartup)]
    public class LoginViewModel : AppScreen
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

        [Import(typeof(IUserRepository))]
        private IUserRepository _userRepository;

        [Import(typeof(IRoleRepository))]
        private IRoleRepository _roleRepository;
        [Import(typeof(IPermissionRepository))]
        private IPermissionRepository _permissionRepository;


        public LoginViewModel()
        {
            //SetupEmbeddedStatusBar();
        }

        private void SetupEmbeddedStatusBar()
        {
            this.EmbeddedContentBar = new EmbeddedContentBarViewModel(this)
            {
                EmbeddedStatusBarTemplate = Application.Current.FindResource("UserLoginBarDataTemplate") as DataTemplate
            };
        }

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

        public string Username
        {
            get
            {
                if (SelectedUser == null)
                {
                    return _userName;
                }

                return SelectedUser.Username;
            }
            set => Set(ref _userName, value);
        }

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public string Pincode
        {
            get => _pincode;
            set => Set(ref _pincode, value);
        }


        private bool _IsDialogOpen;

        public bool IsDialogOpen
        {
            get => _IsDialogOpen;
            set => Set(ref _IsDialogOpen, value);
        }

       
        private string _userName;
        private string _password;
        private SettingsManager<LoginHistory> _loginHistoryManager;
        private LoginHistory _loginHistory;
        private string _pincode;

        public void SetPasswordAndLogin( object sender)
        {
            Password = (sender as PasswordBox)?.Password;
            if (string.IsNullOrEmpty(Username))
            {
                ToastNotification.Notify("Enter a Username First", NotificationType.Error);
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                ToastNotification.Notify("Enter a Password First");
                return;
            }
            Login();
        }


        public void SetPinCodeAndLogin(object sender)
        {
            

            Password = (sender as PasswordBox)?.Password;
            Pincode = (sender as PasswordBox)?.Password;
            if (string.IsNullOrEmpty(Username))
            {
                ToastNotification.Notify("Enter a Username First");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                ToastNotification.Notify("Enter a Pincode First");
                return;
            }
            Login();
        }




        protected override void OnActivate()
        {
            (this.Parent as MainViewModel).IsLoggedIn = false;
            base.OnActivate();
            this.Compose();
           

            List<string> auths = new List<string>();
            auths.Add("Read_Product");
            auths.Add("Can_login");
            var principal = new GenericPrincipal(new GenericIdentity("UserTest", ""), auths.ToArray());
            Thread.CurrentPrincipal = principal;
           LoadLoginHistory();


        }
        public bool CanLogin()
        {
            //logger.Info("User: " + username + "  Pass: " + password);
            return true;// !String.IsNullOrEmpty(username)/* && !String.IsNullOrEmpty(password)*/;
        }
        
        public void Login()
        {
            HttpResponseMessage resp;
            try
            {
                // resp = authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id = 1 });
                 resp = _authService.Authenticate(Username, Password, new Terminal { Id = 1 }, new Annex { Id = 1 });
               
                // resp = authService.Authenticate(new User(){Username = "admin",Password = "admin",Agent = Agent.Desktop});
                if ((int)resp.StatusCode == 401)
                {
                    ToastNotification.Notify("Wrong username or password");
                    return;
                }
                
               

            }
            catch (AggregateException)
            {
                //ToastNotification.Notify("Check your server connection");
                ToastNotification.Notify(NotificationHelper.CHECK_SERVER_CONNECTION);
                return;
            }

            IsDialogOpen = true;
            var associationManager = AssociationManager.Instance;
            

            StateManager.Instance
                .Manage(_productRepository,fetch:false,withAssociatedTypes:true)
                .Manage(_categoryRepository,false,withAssociatedTypes:true)
                .Manage(_additiveRepository,false)
                .Manage(_orderRepository, fetch: false,withAssociatedTypes:true)
                .Manage(_tableRepository,false)
                .Manage(_customerRepository)
                .Manage(_waiterRepository,false)
                .Manage(_deliverymanRepository,false)
                .Manage(_paymentRepository)
                .Manage(_userRepository,withAssociatedTypes:true)
                .Manage(_roleRepository,withAssociatedTypes:true)
                .Manage(_permissionRepository);

            var user = Users.FirstOrDefault(u => u.Username.Equals(Username));
            var userBackground = resp.Headers.GetValues("user-meta-background").First();

            if (user == null)
            {
                user = new User() { Username = Username };
                Users.Add(user);
            }
            user.BackgroundString = userBackground;

            _loginHistory = new LoginHistory() {Users = Users.Select(u=> new User(){Id = u.Id,Username = u.Username,BackgroundString = u.BackgroundString}).ToList()};
            _loginHistory.LastLoggedUserByUsername = user.Username;
            
            if (user?.Id!= null)
            {
                _loginHistory.LastLoggedUserId = (long)user.Id;
            }
            var sm = new SettingsManager<LoginHistory>("login.history.json");
            AppConfigurationManager.Save(_loginHistory);

            (this.Parent as MainViewModel).IsLoggedIn = true;
                //(this.Parent as MainViewModel).IsLoggedIn = true;


            //Checkout();
            //CheckoutSettings();
        




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
           

            CheckoutViewModel checkoutViewModel =
                new CheckoutViewModel(){Parent = this.Parent};
            checkoutViewModel.ActivateLoadingScreen();
            checkoutViewModel.ViewModelInitialized+=ViewModelInitialized;

        }

        public void CheckoutSettings()
        {
            IsDialogOpen = false;


            var checkoutSettingsViewModel =
                new CheckoutSettingsViewModel() { Parent = this.Parent };
            checkoutSettingsViewModel.ActivateLoadingScreen();
            checkoutSettingsViewModel.ViewModelInitialized += ViewModelInitialized;

        }

        private void ViewModelInitialized(object sender, ViewModelInitializedEventArgs e)
        {
            var checkoutViewModel = sender as CheckoutViewModel;
            if (e.IsInitialized)
            {
                (this.Parent as Conductor<object>).ActivateItem(checkoutViewModel);
            }
        }

        public void Settings()
        {
            _authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id = 1 });
            Settings1ViewModel settingsViewModel = new Settings1ViewModel(30/*, _productService, _categorieService, _additiveService*/);
            settingsViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(settingsViewModel);
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

        
        public void CustomersSettings()
        {
            CustomerViewModel customerViewModel = new CustomerViewModel(null/*,_customerService*/);
            customerViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(customerViewModel);
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

        private void LoadLoginHistory()
        {
            var history = AppConfigurationManager.Configuration<LoginHistory>();
            Users = history?.Users == null ? 
                new ObservableCollection<User>() : 
                new ObservableCollection<User>(history.Users);
            //if (history?.LastLoggedUserId!= null)
            //{
            //    SelectedUser = Users?.FirstOrDefault(u => u.Id == history.LastLoggedUserId);
            //}
            if (history?.LastLoggedUserByUsername != null)
            {
                SelectedUser = Users?.FirstOrDefault(u => u.Username == history.LastLoggedUserByUsername);
            }
        }

        public void ForgetUser(User user)
        {
            if (Users!= null && Users.Any())
            {
                Users.Remove(user);
            }
            AppConfigurationManager.Save(_loginHistory);
        }
    }

    class LoginHistory
    {
        [DataMember]
        public IList<User> Users { get; set; }

        [DataMember]
        public long LastLoggedUserId { get; set; }
        public string LastLoggedUserByUsername { get; set; }
    }
}
