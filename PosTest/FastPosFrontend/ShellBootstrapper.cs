using Caliburn.Micro;
using NLog;
using FastPosFrontend.ViewModels;
using System.Windows;

namespace FastPosFrontend
{
    public class ShellBootstrapper : BootstrapperBase
    {
        public ShellBootstrapper()
        {

            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "/logs/fastpos.txt" };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            NLog.LogManager.Configuration = config;

            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //var splashScreen = new SplashScreenView();
            //splashScreen.Show();
            //System.Threading.Thread.Sleep(7000);
            //splashScreen.Close();

            DisplayRootViewFor<MainViewModel>();
        }

       
    }
}
