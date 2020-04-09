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

        [Import(typeof(ICategorieService))]
        private ICategorieService categorieService = null;


        public String Username { get; set; }
        public String Password { get; set; }

        public bool CanLogin(string username, string password)
        {
            //logger.Info("User: " + username + "  Pass: " + password);
            return true;// !String.IsNullOrEmpty(username)/* && !String.IsNullOrEmpty(password)*/;
        }

        public async void Login(string username, string password)
        {
            this.Compose();
            CheckoutViewModel toActivateViewModel = new CheckoutViewModel();
            toActivateViewModel.Parent = this.Parent;

            toActivateViewModel.Products1 = new BindableCollection<Product>(productService.GetAllProducts());
            /*toActivateViewModel.currentOrderitem =
                new BindableCollection<OrdreItem>(
                    new List<OrdreItem>{
                        new OrdreItem{ Id=0, ProductId= 1, OrderId=0, Quantity=1, UnitPrice=1, Total=1, product= 
                        new Product{Id=1, Name="test", Price=1, Unit="ddd", },} });*/
            
            toActivateViewModel.Category = new BindableCollection<Categorie>(categorieService.GetAllCategory());
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
