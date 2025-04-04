﻿using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using FastPosFrontend.Configurations;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{
    public class MainViewModel : Conductor<object>,IConductor<object> /*AppNavigationConductor<object>*/
    {
        private const string WindowTitleDefault = "FAST POS";

        private string _windowTitle = WindowTitleDefault;
        private string _buttonStr = "Login";
        private bool _isLoggedIn;
        private bool _isDbServerOn;
        private bool _isBackendServerOn;
        private MainDialog _mainDialog;
        private bool _isAttemptingToStartBackendServer;
        private DrawerManager _drawer = DrawerManager.Instance;
        public NavigationManager<object> Navigator{ get;  set ; }

        public MainViewModel()
        {
           
            // initializing navigation manager 
            Navigator = NavigationManager<object>.Init(this);


            OpenNavigationDrawerCommand = DelegateCommandBase.Of(DrawerManager.Instance.OpenNavigationDrawer);
            DrawerManager.Instance.InitNavigationDrawer(this, "AppNavigationDrawer", this);
            // check if the backend sever is up and running 
            IsBackendServerOn = ConnectionHelper.PingHost();
            // setting up mapping of different entities 
            Associations.Setup();
            // setting up response handler for state manager 
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
                    Navigator?.LoadNavigationItems();
                    Navigator?.OnLogin();
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



      
        public MainDialog MainDialog
        {
            get => _mainDialog;
            set => Set(ref _mainDialog, value);
        }

        public DrawerManager Drawer
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

            MainDialog = new MainDialog();
            ActivateItem(MainDialog);
            LoginViewModel toActivateViewModel = new LoginViewModel {Parent = this};

            Navigator?.NavigateToItem(new NavigationLookupItem("Login", target: typeof(LoginViewModel)));


        }

        public void Logout()
        {
            DrawerManager.Instance?.CloseAll();

            var result = Navigator?.NavigateToItem(new NavigationLookupItem("Login", target: typeof(LoginViewModel)));
            if (result.HasValue && result.Value)
            {
                StateManager.Flush();
                Navigator?.KeepAliveScreens.Clear();
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
                if (ConnectionHelper.PingHost())
                {
                    IsBackendServerOn = true;
                }
                else
                {
                    IsBackendServerOn = await Task.Run(ShellBootstrapper.StartBackendServer);
                }

                IsAttemptingToStartBackendServer = false;

            }
        }
    }

    public interface IAppNavigationConductor
    {
    }
}