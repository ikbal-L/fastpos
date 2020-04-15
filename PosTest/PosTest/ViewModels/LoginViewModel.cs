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
using System.ComponentModel;
using System.Windows.Data;

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

        protected override void OnActivate()
        {
            base.OnActivate();
            this.Compose();
            
        }
        public bool CanLogin(string username, string password)
        {
            //logger.Info("User: " + username + "  Pass: " + password);
            return true;// !String.IsNullOrEmpty(username)/* && !String.IsNullOrEmpty(password)*/;
        }

        public async void Login(string username, string password)
        {
            
            CheckoutViewModel checkoutViewModel = new CheckoutViewModel();
            checkoutViewModel.Parent = this.Parent;

            checkoutViewModel.AllRequestedProducts = productService.GetAllProducts();
            checkoutViewModel.FilteredProducts = CollectionViewSource.GetDefaultView(checkoutViewModel.AllRequestedProducts);
            checkoutViewModel.MaxProductPageSize = 30;
            checkoutViewModel.ProductsVisibility = Visibility.Visible;
            checkoutViewModel.AdditivesVisibility = Visibility.Collapsed;
            //checkoutViewModel.ProductsPage = checkoutViewModel.FilteredProducts;
            checkoutViewModel.PaginateProducts(NextOrPrevious.First);
            /*toActivateViewModel.currentOrderitem =
                new BindableCollection<OrdreItem>(
                    new List<OrdreItem>{
                        new OrdreItem{ Id=0, ProductId= 1, OrderId=0, Quantity=1, UnitPrice=1, Total=1, product= 
                        new Product{Id=1, Name="test", Price=1, Unit="ddd", },} });*/
            
            checkoutViewModel.Categories = new BindableCollection<Category>(categorieService.GetAllCategory());
            checkoutViewModel.InitCategoryColors();
            (this.Parent as Conductor<object>).ActivateItem(checkoutViewModel);
        }

        //This method load th DLL file containing the implemetation of IProductService 
        // and satisfay the import in this class
        private void Compose()
        {
            //AssemblyCatalog catalog1 = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
            AssemblyCatalog catalog2 = new AssemblyCatalog("ServiceLib.dll");
            CompositionContainer container = new CompositionContainer(catalog2);
            container.SatisfyImportsOnce(this);
        }
    }
}
