using System;
using System.Threading.Tasks;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{
    public class MainViewModel : AppNavigationConductor<object>
    {
        private const string WindowTitleDefault = "FAST POS";

        private string _windowTitle = WindowTitleDefault;
        private string _buttonStr = "Login";
        private bool _isLoggedIn;
        private bool _isDbServerOn;
        private bool _isBackendServerOn;
        private MainDialog _mainDialog;
        private bool _isAttemptingToStartBackendServer;
        private AppDrawerConductor _drawer = AppDrawerConductor.Instance;


        public MainViewModel()
        {

            OpenNavigationDrawerCommand = new DelegateCommandBase((o)=> {
                AppDrawerConductor.Instance.OpenNavigationDrawer();
            });
            AppDrawerConductor.Instance.InitNavigationDrawer(this, "AppNavigationDrawer", this);
            
            IsBackendServerOn = ConnectionHelper.PingHost();
            IsDbServerOn = ConnectionHelper.PingHost(portNumber: 3306);
            Associations.Setup();
            StateManager.Instance.HandleErrorsUsing(ResponseHandler.Handler);
        }

        public void SetSettingsListeners()
        {
            
        }
        public DelegateCommandBase OpenNavigationDrawerCommand { get; set; }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                Set(ref _isLoggedIn, value);
                if (value)
                {
                    LoadNavigationItems();
                    OnLogin();
                }
            }
        }

        public bool IsDbServerOn
        {
            get => _isDbServerOn;
            set => Set(ref _isDbServerOn, value);
        }

        public bool IsBackendServerOn
        {
            get => _isBackendServerOn;
            set => Set(ref _isBackendServerOn, value);
        }

        public bool IsAttemptingToStartBackendServer
        {
            get => _isAttemptingToStartBackendServer;
            set => Set(ref _isAttemptingToStartBackendServer, value);
        }


        public String ButtonStr
        {
            get => _buttonStr;
            set
            {
                _buttonStr = value;
                NotifyOfPropertyChange(() => ButtonStr);
            }
        }


        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                _windowTitle = value;
                NotifyOfPropertyChange(() => WindowTitle);
            }
        }

        public MainDialog MainDialog
        {
            get => _mainDialog;
            set => Set(ref _mainDialog, value);
        }

        public AppDrawerConductor Drawer
        {
            get => _drawer;
            set => Set(ref _drawer, value);
        }

        public DialogClosedHandler OpenDialog<T>() where T : DialogContent, new()
        {
            var dialog = new T();
            return OpenDialog(dialog);
        }

        public DialogClosedHandler OpenDialog<T>(T dialog) where T : DialogContent
        {
            return MainDialog.Open(dialog);
        }

        public bool CloseDialog<T>(T dialog) where T : DialogContent
        {
            return MainDialog.Close(dialog);
        }

        public bool CloseDialog()
        {
            return MainDialog.Close();
        }

        protected override void OnActivate()
        {
            //var splashScreen = new SplashScreenView();
            //splashScreen.Show();
            //System.Threading.Thread.Sleep(7000);
            //Task.Factory.StartNew(() =>
            //{
            //    
            //    this.Dispatcher.Invoke(() =>
            //    {
            //        DisplayRootViewFor<MainViewModel>();
            //        splashScreen.Close();
            //    });
            //});
            MainDialog = new MainDialog();
            ActivateItem(MainDialog);
            LoginViewModel toActivateViewModel = new LoginViewModel {Parent = this};
            
            NavigateToItem(new NavigationLookupItem("Login", target: typeof(LoginViewModel)));

            //splashScreen.Close();
        }

        public void Logout()
        {
            LoginViewModel toActivateViewModel = new LoginViewModel { Parent = this };
            if (NavigateToItem(new NavigationLookupItem("Login", target: typeof(LoginViewModel))))
            {
                StateManager.Flush();
                KeepAliveScreens.Clear();
                IsLoggedIn = false;
            }

            StartBackendServer(true);


        }

        public async Task StartBackendServer(bool updateInfoRequested = false)
        {

            if (!IsAttemptingToStartBackendServer)
            {
                if (IsBackendServerOn && !updateInfoRequested)
                {
                    ModalDialogBox.Ok("The backend server is already running!", "Info - Backend server Status").Show();
                    return;
                }
                IsAttemptingToStartBackendServer = true;
                var result = await Task.Run(ShellBootstrapper.StartBackendServer);
                IsBackendServerOn = result;
                IsAttemptingToStartBackendServer = false;

            }
        }
    }

    public interface IAppNavigationConductor
    {
    }
}