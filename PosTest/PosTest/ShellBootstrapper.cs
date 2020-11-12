using Caliburn.Micro;
using NLog;
using PosTest.ViewModels;
using PosTest.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PosTest
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
