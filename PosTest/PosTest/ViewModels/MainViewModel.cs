using Caliburn.Micro;
using PosTest.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        private const string WindowTitleDefault = "FAST POS";

        private string _windowTitle = WindowTitleDefault;
        private string _buttonStr = "Login";




        public String ButtonStr { get=> _buttonStr; set { _buttonStr = value; NotifyOfPropertyChange(() => ButtonStr); } }

        /*public IScreen ActiveItem { get; private set; }

        public void ActivateItem(object item)
        {
            ActiveItem = item as IScreen;

            var child = ActiveItem as IChild;
            if (child != null)
                child.Parent = this;

            if (ActiveItem != null)
                ActiveItem.Activate();

            NotifyOfPropertyChange(() => ActiveItem);
            ActivationProcessed(this, new ActivationProcessedEventArgs { Item = ActiveItem, Success = true });
        }*/

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
            LoginViewModel toActivateViewModel = new LoginViewModel();
            toActivateViewModel.Parent = this;
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
