using Caliburn.Micro;
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
