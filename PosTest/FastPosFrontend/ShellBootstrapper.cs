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
                .Add<DeliveryCheckoutViewModel>()    
                .Add<CreditCheckoutViewModel>()
                .Add<DailyExpenseReportsViewModel>()
                .Add<OrderRefundViewModel>()
                //.Add(Constants.Navigation.GroupNames.Settings)
                .Add<CheckoutSettingsViewModel>()
                .Add<AdditivesSettingsViewModel>()
                .Add<GeneralSettingsViewModel>()
                .Add<PrintSettingsViewModel>()
                .Add<UserSettingsViewModel>()
                .Add<RoleSettingsViewModel>()
                .Add<DeliveryManSettingsViewModel>()
                .Add<WaiterSettingsViewModel>()
                .Add<CustomerSettingsViewModel>();



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
            var jre = @".\jre\model-win-x";
            var jrex86 = @$"{jre}86\bin\";
            var jrex64 = @$"{jre}64\bin\";

            _backendServerProcess ??= new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "java",

                    Arguments = "-jar model-0.0.1-SNAPSHOT.jar",
                    ErrorDialog = true,
                    CreateNoWindow = true,
                    UseShellExecute = true
                },

            };

            //if (Environment.Is64BitOperatingSystem)
            //{
            //    _backendServerProcess.StartInfo.WorkingDirectory = jrex64;
            //}
            //else
            //{
            //    _backendServerProcess.StartInfo.WorkingDirectory = jrex86;

            //}


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

        private static void _backendServerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            
        }

        private static void _backendServerProcess_Exited(object sender, EventArgs e)
        {
            
        }
    }
}