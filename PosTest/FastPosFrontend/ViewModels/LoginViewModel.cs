using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using FastPosFrontend.Configurations;
using FastPosFrontend.Events;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using FastPosFrontend.ViewModels.DeliveryAccounting;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{
    [NavigationItem("Login",target:typeof(LoginViewModel),"",NavigationItemLoadingStrategy.OnStartup)]
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

        [Import(typeof(IDailyEarningsReportRepository))]
        private IDailyEarningsReportRepository _dailyExpenseReportRepository;
        [Import(typeof(ICashRegisterExpenseRepository))]
        private ICashRegisterExpenseRepository _cashRegisterExpenseRepository;
        [Import(typeof(IExpenseDescriptionRepository))]
        private IExpenseDescriptionRepository _expenseDescriptionRepository;


        public LoginViewModel()
        {
            //SetupEmbeddedStatusBar();
        }

        private void SetupEmbeddedStatusBar()
        {
            EmbeddedRightCommandBar = new EmbeddedCommandBarViewModel(this, "UserLoginBarDataTemplate");
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
            (Parent as MainViewModel).IsLoggedIn = false;
            base.OnActivate();
            Compose();

           LoadLoginHistory();
        }
       
        
        public void Login()
        {
            var baseUrl = ConfigurationManager.Get<PosConfig>()?.Url;

            HttpResponseMessage resp;
            try
            {

                 resp = _authService.Authenticate(Username, Password, new Terminal { Id = 1 }, new Annex { Id = 1 });
                if ((int)resp.StatusCode == 401)
                {
                    ToastNotification.Notify("Wrong username or password");
                    return;
                }

                if (resp.StatusCode!= HttpStatusCode.OK)
                {
                    ToastNotification.Notify(NotificationHelper.CHECK_SERVER_CONNECTION);

                }

            }
            catch (AggregateException)
            {
                ToastNotification.Notify(NotificationHelper.CHECK_SERVER_CONNECTION);
                return;
            }

            IsDialogOpen = true;

            StateManager
                .Instance
                .BaseUrl(baseUrl)
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
                .Manage(_permissionRepository)
                .Manage(_dailyExpenseReportRepository)
                .Manage(_cashRegisterExpenseRepository)
                .Manage(_expenseDescriptionRepository);
                

            var user = Users.FirstOrDefault(u => u.Username.Equals(Username));
            var hasUserBackground = resp.Headers.TryGetValues("user-meta-background",out var userBackground);

            if (user == null)
            {
                user = new User() { Username = Username };
                Users.Add(user);
                _loginHistory.Users.Add(user);
            }
            if (hasUserBackground)
            {
                user.BackgroundString = userBackground.First(); 
            }


            _loginHistory.LastLoggedUserByUsername = user.Username;
            
            if (user?.Id!= null)
            {
                _loginHistory.LastLoggedUserId = (long)user.Id;
            }

            _loginHistory.RequestSave();

            (Parent as MainViewModel).IsLoggedIn = true;

        }
        public void NumericKeyboard(PasswordBox passwordBox,string key)
        {
            if(!key.Equals("-"))
                passwordBox.Password += key;
            else
                passwordBox.Password = passwordBox.Password.Substring(0, passwordBox.Password.Length==0?passwordBox.Password.Length: passwordBox.Password.Length - 1);
        }

        

        private void ViewModelInitialized(object sender, ViewModelInitializedEventArgs e)
        {
            var checkoutViewModel = sender as CheckoutViewModel;
            if (e.IsInitialized)
            {
                (Parent as Conductor<object>).ActivateItem(checkoutViewModel);
            }
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

     

        private void LoadLoginHistory()
        {
            _loginHistory = ConfigurationManager.Get<PosConfig>().LoginHistory;
          
            Users = new ObservableCollection<User>(_loginHistory.Users);
            if (_loginHistory?.LastLoggedUserByUsername != null)
            {
                SelectedUser = Users?.FirstOrDefault(u => u.Username == _loginHistory.LastLoggedUserByUsername);
            }
        }

        public void ForgetUser(User user)
        {
            if (Users!= null && Users.Any())
            {
                Users.Remove(user);
            }
            _loginHistory.RequestSave();
        }
    }
}
