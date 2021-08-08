using System;
using System.Diagnostics;
using Caliburn.Micro;
using NLog;
using FastPosFrontend.ViewModels;
using System.Windows;
using FastPosFrontend.Helpers;
using FastPosFrontend.ViewModels.DeliveryAccounting;
using FastPosFrontend.ViewModels.Settings;
using FastPosFrontend.ViewModels.Settings.Customer;

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
            NavigationIndexer.ImplicitIndex()
                .Add<CheckoutViewModel>()
                .Add<DeliveryAccountingViewModel>()
                .Add<DailyExpenseReportsViewModel>()
                .Add(Constants.Navigation.GroupNames.Settings)
                .Add<CheckoutSettingsViewModel>()
                .Add<GeneralSettingsViewModel>()
                .Add<PrintSettingsViewModel>()
                .Add<UserSettingsViewModel>()
                .Add<RoleSettingsViewModel>()
                .Add<AdditivesSettingsViewModel>()
                .Add<DeliveryManSettingsViewModel>()
                .Add<WaiterSettingsViewModel>()
                .Add<CustomerSettingsViewModel>();

            //splashScreen.Show();
            //System.Threading.Thread.Sleep(7000);
            //splashScreen.Close();




            //_backendServerProcess = new Process()
           

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

            
            _backendServerProcess ??= new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "java",
                    Arguments = "-jar model-0.0.1-SNAPSHOT.jar",
                    ErrorDialog = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };

            if (!ConnectionHelper.PingHost())
            {
                _backendServerProcess.Start();

            }
            else
            {
                return true;
            }

            var stopWatch = Stopwatch.StartNew();
            stopWatch.Start();
            while (!ConnectionHelper.PingHost())
            {
                if (stopWatch.ElapsedMilliseconds >= 100000)
                {
                    stopWatch.Stop();
                    //backendServerProcess.Kill();
                    break;
                }
            }
            return stopWatch.IsRunning;
        }

    }
}