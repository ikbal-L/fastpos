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
using PosTest.Helpers;
using ServiceInterface.Interface;
using ServiceInterface.Model;

namespace PosTest.ViewModels
{
    public class AdditivesSettingsViewModel : Screen
    {
        private BindableCollection<Additive> _additives;
        private Additive _selectedAdditive;
        private IAdditiveService _additiveService;
        private List<Additive> _allAdditives;
        private int _additivePageSize;
        private bool _isEditing;
        private Additive _clipBoardAdditive;

        public AdditivesSettingsViewModel(IAdditiveService additiveService, int additivePageSize)
        {
            _additiveService = additiveService;
            _additivePageSize = additivePageSize;
            int additiveStatusCode = 0;
            _allAdditives = additiveService.GetAllAdditives(ref additiveStatusCode).ToList();
            _isEditing = false;
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

        public Additive ClipBoardAdditive
        {
            get => _clipBoardAdditive;
            set => Set(ref _clipBoardAdditive, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => Set(ref _isEditing, value);
        }

        private void PopulateAdditivesPage()
        {
            var comparer = new Comparer<Additive>();
            var additives = new List<Additive>(_allAdditives.Where(a => a.Rank != null));
            additives.Sort(comparer);
            Additives = new BindableCollection<Additive>();
            var maxRank = (int) additives.Max(a => a.Rank);
            int numberOfPages = (maxRank / _additivePageSize) + (maxRank % _additivePageSize == 0 ? 0 : 1);
            numberOfPages = numberOfPages == 0 ? 1 : numberOfPages;
            var size = numberOfPages * _additivePageSize;

            RankedItemsCollectionHelper.LoadPagesFilled(source: additives, target: Additives, size: size);
        }


        public void AdditivesList_MouseMove(object sender, MouseEventArgs e)
        {
            var key = "Additive";

            CheckoutSettingsViewModel.MouseMoveEventHandler<Additive>(sender, e, key, nameof(Additive.Description));
        }

        public void AdditivesList_TouchDown(object sender, TouchEventArgs e)
        {
            var key = "Additive";

            CheckoutSettingsViewModel.ListTouchDownEventHandler<Additive>(sender, e, key);
        }


        public void AdditivesList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Additive"))
            {
                ListBox listView = sender as ListBox;
                ListBoxItem target =
                    CheckoutSettingsViewModel.FindAncestor<ListBoxItem>((DependencyObject) e.OriginalSource);

                if (target == null)
                {
                    return;
                }

                // Find the data behind the ListViewItem
                Additive targetAdditive = (Additive) listView.ItemContainerGenerator.ItemFromContainer(target);

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

                PutAdditiveInCellOf(targetAdditive, receivedAdditive);
            }
        }

        private void PutAdditiveInCellOf(Additive targetAdditive, Additive receivedAdditive)
        {
            
        }

        public void SaveAdditive()
        {
            IsEditing = false;
            if (SelectedAdditive.Id==null)
            {
                long id;
                _additiveService.SaveAdditive(SelectedAdditive,out id);
                SelectedAdditive.Id = id;
            }
            else
            {
                _additiveService.UpdateAdditive(SelectedAdditive);
            }
        }

        public void CopyAdditive()
        {
            if (SelectedAdditive==null|| SelectedAdditive.Id==null)
            {
                ToastNotification.Notify("Select a Valid Additive to copy",1);
                return;
            }

            ClipBoardAdditive = SelectedAdditive;
        }

        public void PasteAdditive()
        {
            if (ClipBoardAdditive == null)
            {
                ToastNotification.Notify("Copy an additive first");
                return;
            }

            if (SelectedAdditive == null|| SelectedAdditive.Id==null)
            {
                ToastNotification.Notify("Select a zone to copy in first");
                return;
            }

            if (ClipBoardAdditive.Equals(SelectedAdditive))
            {
                ToastNotification.Notify("You chose the same additive");
                return;
            }

            int rank = (int)SelectedAdditive.Rank;
            var additive = new Additive(ClipBoardAdditive);
            additive.Rank = rank;
            additive.Id = null;
            if (_additiveService.SaveAdditive(additive, out var id)==200)
            {
                
                additive.Id = id; 
            }

            Additives[rank - 1] = additive;



        }

        public object clone(object obj)
        {
            return obj;
        }
    }
}