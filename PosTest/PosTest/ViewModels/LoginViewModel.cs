using Caliburn.Micro;
using ServiceInterface.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ServiceInterface.Model;
using System.ComponentModel.Composition.Hosting;

namespace PosTest.ViewModels
{
    public class LoginViewModel : Screen
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [Import(typeof(IProductService))]
        private IProductService productService = null;

        public String Username { get; set; }
        public String Password { get; set; }

        public bool CanLogin(string username, string password)
        {
            logger.Info("User: " + username + "  Pass: " + password);
            return !String.IsNullOrEmpty(username)/* && !String.IsNullOrEmpty(password)*/;
        }

        public void Login(string username, string password)
        {
            this.Compose();
            CheckoutViewModel toActivateViewModel = new CheckoutViewModel();
            toActivateViewModel.Parent = this.Parent;
            toActivateViewModel.Products = new BindableCollection<Product>(productService.Products);
            (this.Parent as Conductor<object>).ActivateItem(toActivateViewModel);
        }

        //This method load th DLL file containing the implemetation of IProductService 
        // and satisfay the import in this class
        private void Compose()
        {
            AssemblyCatalog catalog1 = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
            AssemblyCatalog catalog2 = new AssemblyCatalog("ServiceLib.dll");
            CompositionContainer container = new CompositionContainer(catalog2);
            container.SatisfyImportsOnce(this);
        }
    }
}
