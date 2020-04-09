using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServiceInterface.Model;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel;

namespace PosTest.ViewModels
{
    public class CheckoutViewModel : Screen
    {

        public ICollectionView Products { get; set; }

        public BindableCollection<Product> products { get; set; }

        public BindableCollection<Product> Products1
        {
            get { return products; }
            set
            {
                products = value;
                Products = CollectionViewSource.GetDefaultView(value);
                NotifyOfPropertyChange(() => Products);
                //NotifyOfPropertyChange(() => CanSayHello);
            }
        }

        public BindableCollection<Categorie> Category { get; set; }

        public BindableCollection<Order> Orders { get; set; }



        //BindableCollection<OrdreItem> currentOrderitem;
        public BindableCollection<OrdreItem> CurrentOrderitem
        {
            get;
            set;
            
        }

        private Order currentOrder;

        public Order CurrentOrder
        {
            get { return currentOrder; }
            set
            {
                currentOrder = value;

                NotifyOfPropertyChange(() => currentOrder);
                //NotifyOfPropertyChange(() => currentOrderitem);
                //NotifyOfPropertyChange(() => CanSayHello);
            }
        }

        public CheckoutViewModel()
        {
            //currentOrderitem = new BindableCollection<OrdreItem>();
            Orders = new BindableCollection<Order>();
            CurrentOrderitem = new BindableCollection<OrdreItem>();
            Orders.Add(new Order { Id = 1, BuyerId = "1" });
            CurrentOrder = Orders[0];
        }

        Categorie currantCategorie;

        public Categorie CurrantCategorie
        {
            get { return currantCategorie; }
            set
            {
                currantCategorie = value;
                NotifyOfPropertyChange(() => currantCategorie);
                //NotifyOfPropertyChange(() => CanSayHello);
            }
        }

        public void Close()
        {
            LoginViewModel toActivateViewModel = new LoginViewModel();
            toActivateViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(toActivateViewModel);
        }

        public void CategorieFiltering(object sender)
        {
            //Console.WriteLine(sender);
            Categorie _categorie = sender as Categorie;
            if (_categorie != null)
            {
                CurrantCategorie = _categorie;
                Products.Filter = CatFilter;
                //Console.WriteLine(Products.Where(p => p.CategorieId == CurrantCategorie.Id).Count());
                //Console.WriteLine(Products.Count());
                //Products =  Products.Where(p => p.CategorieId == CurrantCategorie.Id).ToList();

                //Products = Products;
            }
            Button _home = sender as Button;
            if (_home != null && _home.Name == "Home")
                Products.Filter =  null;
        }

        public void ProductFiltering(object sender)
        {
            Console.WriteLine(sender);
        }

        public void AddOrderItem(object sender)
        {
            Product selectedproduct = (Product)sender;
            Console.WriteLine();

            CurrentOrderitem.Add(
                new OrdreItem
                {
                    Id = 1,
                    OrderId = CurrentOrder.Id,
                    ProductId = selectedproduct.Id,
                    product = selectedproduct,
                    Quantity = 1,
                    UnitPrice = selectedproduct.Price,
                    Total = 1 * selectedproduct.Price,
                });
        
            //Console.WriteLine(CurrentOrder.Items.Count);
            //currentOrderitem = new BindableCollection<OrdreItem>(CurrentOrder.Items);
            //Console.WriteLine(selectedproduct+"  |  "+ currentOrderitem);


        }

        private bool CatFilter(object item)
        {
            Product p = item as Product;
            return p.CategorieId.Equals(currantCategorie.Id);
        }

    }
}
