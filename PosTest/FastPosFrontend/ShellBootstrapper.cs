using System;
using System.Diagnostics;
using Caliburn.Micro;
using NLog;
using FastPosFrontend.ViewModels;
using System.Windows;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using FastPosFrontend.ViewModels.DeliveryAccounting;
using FastPosFrontend.ViewModels.Settings;
using FastPosFrontend.ViewModels.Settings.Customer;
using System.ServiceProcess;
using FastPosFrontend.Configurations;
using ServiceLib.Service;

namespace FastPosFrontend
{
    public class ShellBootstrapper : BootstrapperBase
    {

        private static Process _backendServerProcess;
        public ShellBootstrapper()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") {FileName = "/logs/fastpos.txt"};
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            NLog.LogManager.Configuration = config;

            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            // setting up the order of views in navigation menu 
            NavigationIndexer.ImplicitIndex()
                .Add<CheckoutViewModel>()
                .Add<DeliveryCheckoutViewModel>()    
                .Add<CreditCheckoutViewModel>()
                .Add<DailyExpenseReportsViewModel>()
                .Add<OrderRefundViewModel>()
                .Add<CheckoutSettingsViewModel>()
                .Add<AdditivesSettingsViewModel>()
                .Add<GeneralSettingsViewModel>()
                .Add<PrintSettingsViewModel>()
                .Add<UserSettingsViewModel>()
                .Add<RoleSettingsViewModel>()
                .Add<DeliveryManSettingsViewModel>()
                .Add<WaiterSettingsViewModel>()
                .Add<CustomerSettingsViewModel>();
            // loading app config from file 
            ConfigurationManager.Init<PosConfig>(PosConfig.FILE_NAME);
            
            var baseUrl = ConfigurationManager.Get<PosConfig>().Url;
            var lm = new LicenseManager(baseUrl);
            lm.Check();

            DisplayRootViewFor<MainViewModel>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            //EXCEPTION

            try
            {
                _backendServerProcess?.Kill();
            }
            catch (Exception)
            {

            }

            base.OnExit(sender, e);
        }

        public static bool StartBackendServer()
        {
            ServiceController service = new ServiceController("FrutaPOS Server");
            if (service.Status == ServiceControllerStatus.Stopped|| service.Status == ServiceControllerStatus.StopPending)
            {
                service.Start();
            }

            return service.Status == ServiceControllerStatus.Running;
        }

        private static void _backendServerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            
        }

        private static void _backendServerProcess_Exited(object sender, EventArgs e)
        {
            
        }
    }
}