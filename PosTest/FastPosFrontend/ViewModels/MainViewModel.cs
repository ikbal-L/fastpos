﻿using System;
using System.Collections.Generic;
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
        private bool _isLoggedIn ;
        private bool _isDbServerOn;
        private bool _isBackendServerOn;
        

        public MainViewModel()
        {
            IsBackendServerOn =ConnectionHelper.PingHost();
            IsDbServerOn = ConnectionHelper.PingHost(portNumber:3306);
            Associations.Setup();
            StateManager.Instance.HandleErrorsUsing(ResponseHandler.Handler);
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
            NavigateToItem(new AppNavigationLookupItem("Login",target:typeof(LoginViewModel)));

            //splashScreen.Close();
        }

        public void Logout()
        {
            StateManager.Flush();
            KeepAliveScreens.Clear();
            ActiveScreen = new LoginViewModel {Parent = this};
            IsLoggedIn = false;
            ActivateItem(ActiveScreen);
        }
    }

    public interface IAppNavigationConductor
    {
    }
}