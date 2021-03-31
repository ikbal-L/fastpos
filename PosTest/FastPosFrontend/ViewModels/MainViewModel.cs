using System;
using System.Net;
using System.Net.NetworkInformation;
using Caliburn.Micro;

namespace FastPosFrontend.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        private const string WindowTitleDefault = "FAST POS";

        private string _windowTitle = WindowTitleDefault;
        private string _buttonStr = "Login";
        private bool _isLoggedIn = false;
        private bool _isDbServerOn = false;
        private bool _isBackendServerOn = false;

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => Set(ref _isLoggedIn, value);
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


        public String ButtonStr { get=> _buttonStr; set { _buttonStr = value; NotifyOfPropertyChange(() => ButtonStr); } }


        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                _windowTitle = value;
                NotifyOfPropertyChange(() => WindowTitle);
            }
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
            LoginViewModel toActivateViewModel = new LoginViewModel {Parent = this};
            //UserSettingsViewModel userSettingsViewModel = new UserSettingsViewModel() { Parent = this };
            //ActivateItem(userSettingsViewModel);
            ActivateItem(toActivateViewModel);

            //splashScreen.Close();
        }
        public void showLogin()
        {
            ButtonStr = "Logged in";
            ActivateItem(new LoginViewModel());
        }
    }
}
