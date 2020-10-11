using Caliburn.Micro;
using ServiceInterface.Model;
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
            foreach (var additive in Additives)
            {
             Console.WriteLine(additive.Description);   
            }
        }

        public void AdditivesList_MouseMove(object sender, MouseEventArgs e)
        {
            var key = "Additive";

            CheckoutSettingsViewModel.MouseMoveEventHandler<Additive>(sender, e, key,nameof(Additive.Description));
        }

        public void AdditivesList_TouchDown(object sender, TouchEventArgs e)
        {
            var key = "Additive";

            CheckoutSettingsViewModel.ListTouchDownEventHandler<Additive>(sender, e, key);
        }



        public void AdditivesList_Drop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent("Additive"))
            {

                ListBox listView = sender as ListBox;
                ListBoxItem target =
                   CheckoutSettingsViewModel.FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                if (target == null)
                {
                    return;
                }
                // Find the data behind the ListViewItem
                Additive targetAdditive = (Additive)listView.ItemContainerGenerator.
                    ItemFromContainer(target);

                Additive receivedAdditive = e.Data.GetData("Additive") as Additive;

                    if (receivedAdditive == null || receivedAdditive.Description == null) return;

                Console.WriteLine(targetAdditive.GetHashCode());
                Console.WriteLine(receivedAdditive.GetHashCode());
                
                SelectedAdditive = targetAdditive;
                //TODO Fix locating the index of empty additive
                var indexOfReceived = Additives.IndexOf(receivedAdditive);
                var indexOfTarget = Additives.IndexOf(targetAdditive);
                if (SelectedAdditive.Equals(receivedAdditive))
                {
                    return;
                }

                var targetRank = targetAdditive.Rank;
                var receivedRank = receivedAdditive.Rank;
                targetAdditive.Rank = receivedAdditive.Rank;
                receivedAdditive.Rank = targetRank;
                Additives[(int)targetRank-1] = receivedAdditive;
                Additives[(int)receivedRank-1] = targetAdditive;
                _additiveService.UpdateAdditive(receivedAdditive);
                _additiveService.UpdateAdditive(targetAdditive);

            }
        }

        
    }
}
