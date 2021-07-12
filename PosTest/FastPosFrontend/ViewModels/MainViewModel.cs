using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FastPosFrontend.Events;
using FastPosFrontend.Helpers;
using ServiceInterface.Model;
using ServiceLib.helpers;
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


        public MainViewModel()
        {
            IsBackendServerOn = ConnectionHelper.PingHost();
            IsDbServerOn = ConnectionHelper.PingHost(portNumber: 3306);
            Associations.Setup();
            StateManager.Instance.HandleErrorsUsing(ResponseHandler.Handler);
        }

        public void SetSettingsListeners()
        {
            
        }

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
            this.ActivateItem(MainDialog);
            LoginViewModel toActivateViewModel = new LoginViewModel {Parent = this};
            
            NavigateToItem(new AppNavigationLookupItem("Login", target: typeof(LoginViewModel)));

            //splashScreen.Close();
        }

        public void Logout()
        {
            LoginViewModel toActivateViewModel = new LoginViewModel { Parent = this };
            if (NavigateToItem(new AppNavigationLookupItem("Login", target: typeof(LoginViewModel))))
            {
                StateManager.Flush();
                KeepAliveScreens.Clear();
                IsLoggedIn = false;
            }
            
            
        }
    }

    public interface IAppNavigationConductor
    {
    }
}