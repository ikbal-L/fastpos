using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ServiceInterface.Interface;
using ServiceInterface.Model;

namespace PosTest.ViewModels
{
    public class AdditivesSettingsViewModel: Screen
    {
        private BindableCollection<Additive> _additives;
        private Additive _selectedAdditive;
        private IAdditiveService _additiveService;
        private List<Additive>_allAdditives;
        private int _additivePageSize; 

        public AdditivesSettingsViewModel(IAdditiveService additiveService,int additivePageSize)
        {
            _additiveService = additiveService;
            _additivePageSize = additivePageSize;
            int additiveStatusCode =0;
            _allAdditives = additiveService.GetAllAdditives(ref additiveStatusCode).ToList();
            PopulateAdditivesPage();
            
        }

        public BindableCollection<Additive> Additives
        {
            get => _additives;
            set { Set(ref _additives, value); }
        }

        public Additive SelectedAdditive
        {
            get => _selectedAdditive;
            set { Set(ref _selectedAdditive, value); }
        }

        private void PopulateAdditivesPage()
        {
            var comparer = new Comparer<Additive>();
            var additives = new List<Additive>(_allAdditives.Where(a => a.Rank != null));
            additives.Sort(comparer);
            Additives = new BindableCollection<Additive>();
            var maxRank = (int)additives.Max(a => a.Rank);
            int numberOfPages = (maxRank / _additivePageSize) + (maxRank % _additivePageSize == 0 ? 0 : 1);
            numberOfPages = numberOfPages == 0 ? 1 : numberOfPages;
            var size = numberOfPages * _additivePageSize;
            
            int indexOfRanked = 0;
            for (int i = 1; i <= size; i++)
            {
                Additive additive = null;
                if (indexOfRanked < additives.Count)
                {
                    additive = additives[indexOfRanked];
                }
               
                if (additive != null && additive.Rank == i)
                {
                    Additives.Add(additive);
                    indexOfRanked++;
                }
                else
                {
                    additive = new Additive() { Rank = i };
                    
                    Additives.Add(additive);

                }

            }
        }

        public void AdditivesList_MouseMove(object sender, MouseEventArgs e)
        {
            var key = "Additive";

            CheckoutSettingsViewModel.MouseMoveEventHandler<Additive>(sender, e, key);
        }

        public void AdditivesList_TouchDown(object sender, TouchEventArgs e)
        {
            var key = "Additive";

            CheckoutSettingsViewModel.ListTouchDownEventHandler<Additive>(sender, e, key);
        }



        public void ProductsList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Addative"))
            {

                ListBox listView = sender as ListBox;
                ListBoxItem listBoxItem =
                   CheckoutSettingsViewModel.FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                if (listBoxItem == null)
                {
                    return;
                }
                // Find the data behind the ListViewItem
                Additive additive = (Additive)listView.ItemContainerGenerator.
                    ItemFromContainer(listBoxItem);

                Additive addativeSrc;

                
                    addativeSrc = e.Data.GetData("Addative") as Additive;

                    if (addativeSrc == null || addativeSrc.Description == null) return;


                    //ProductToMove = addativeSrc;
                    //SelectedAdditive = additive;
                    //var index = Additives.IndexOf(ProductToMove);
                    //var prod = new Product { Rank = ProductToMove.Rank };
                    //if (SelectedProduct.Equals(ProductToMove))
                    //{
                    //    ProductToMove = null;
                    //    return;
                    //}
                    //PutProductInCellOf(SelectedProduct, ProductToMove);
                    //CurrentProducts[index] = prod;

                    //ProductToMove = null;
                
            }
        }
    }
}
