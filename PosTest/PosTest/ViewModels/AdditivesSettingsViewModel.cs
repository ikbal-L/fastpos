using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PosTest.Helpers;
using PosTest.ViewModels.SubViewModel;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using ServiceLib.Service;

namespace PosTest.ViewModels
{
    public class AdditivesSettingsViewModel : Screen
    {
        private BindableCollection<Additive> _additives;
        private Additive _selectedAdditive;
        private Additive _copySelectedAdditive;
        private IAdditiveService _additiveService;
        private List<Additive> _allAdditives;
        private int _additivePageSize;
        private bool _isEditing;
        private Additive _clipBoardAdditive;
        private Additive _additiveToMove;
        private bool _isDialogOpen;
        private INotifyPropertyChanged _dialogViewModel;
        private WarningViewModel _warningViewModel;

        public AdditivesSettingsViewModel(/*IAdditiveService additiveService,*/ int additivePageSize)
        {
            IsDialogOpen = false;
            //_additiveService = additiveService;
            _additivePageSize = additivePageSize;
            
             //_allAdditives = additiveService.GetAllAdditives(ref additiveStatusCode).ToList();

            //var (additiveStatusCode, _allAdditives) = GenericRest.GetAll<Additive>(UrlConfig.AdditiveUrl.GetAllAdditives);
            _allAdditives = StateManager.Get<Additive>().ToList();
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
            set {
                CopySelectedAdditive = value?.Clone();
                Set(ref _selectedAdditive, value); }
        }

        public Additive CopySelectedAdditive
        {
            get => _copySelectedAdditive;
            set { Set(ref _copySelectedAdditive, value); }
        }

        public Additive ClipBoardAdditive
        {
            get => _clipBoardAdditive;
            set => Set(ref _clipBoardAdditive, value);
        }

        public Additive AdditiveToMove
        {
            get => _additiveToMove;
            set => Set(ref _additiveToMove, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => Set(ref _isEditing, value);
        }

        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set => Set(ref _isDialogOpen, value);
        }

        public INotifyPropertyChanged DialogViewModel
        {
            get => _dialogViewModel;
            set
            {
                _dialogViewModel = value;
                NotifyOfPropertyChange();
            }
        }

        public WarningViewModel WarningViewModel
        {
            get => _warningViewModel;
            set
            {
                _warningViewModel = value;
                NotifyOfPropertyChange();
            }
        }

        private void PopulateAdditivesPage()
        {
            var comparer = new Comparer<Additive>();
            var additives = new List<Additive>(_allAdditives.Where(a => a.Rank != null));
            additives.Sort(comparer);
            Additives = new BindableCollection<Additive>();
            var maxRank = 30;
            if (additives.Count>0)
            {
                maxRank =  (int) additives.Max(a => a.Rank);
            }
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

                if (receivedAdditive?.Description == null) return;

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
                SelectedAdditive = targetAdditive;
            }
        }

        private void PutAdditiveInCellOf(Additive targetAdditive, Additive incomingAdditive)
        {
            var targetRank = targetAdditive.Rank;
            var receivedRank = incomingAdditive.Rank;
            var indexOfTargetAdditive = Additives.IndexOf(targetAdditive);
            var indexOfIncomingAdditive = Additives.IndexOf(incomingAdditive);
            targetAdditive.Rank = receivedRank;
            incomingAdditive.Rank = targetRank;
            
            Additives[indexOfTargetAdditive] = incomingAdditive;
            Additives[indexOfIncomingAdditive] = targetAdditive;
            
            if (incomingAdditive.Id != null)
            {
                StateManager.Save(incomingAdditive);

            }
            if (targetAdditive.Id != null)
            {
                StateManager.Save(targetAdditive);
            }
        }

        public void SaveAdditive()
        {
            IsEditing = false;
            this.SelectedAdditive.Description = CopySelectedAdditive.Description;
            this.SelectedAdditive.Background = CopySelectedAdditive.Background;
            if (StateManager.Save(SelectedAdditive)) ToastNotification.Notify("Additive saved Successfully",NotificationType.Success);
        }
        public void Cancel() {
            SelectedAdditive = null;
            IsEditing = false;

        }

        public void CopyAdditive()
        {
            if (SelectedAdditive?.Id == null)
            {
                ToastNotification.Notify("Select a Valid Additive to copy", NotificationType.Warning);
                return;
            }

            ClipBoardAdditive = SelectedAdditive;
        }

        public void PasteAdditive()
        {
            if (ClipBoardAdditive == null)
            {
                ToastNotification.Notify("Copy an additive first",NotificationType.Warning);
                return;
            }

            if (SelectedAdditive?.Rank == null)
            {
                ToastNotification.Notify("Select a zone to copy in first",NotificationType.Warning);
                return;
            }

            if (ClipBoardAdditive.Equals(SelectedAdditive))
            {
                ToastNotification.Notify("You chose the same additive",NotificationType.Warning);
                return;
            }

            var rank = (int) SelectedAdditive.Rank;
            var additive = new Additive(ClipBoardAdditive) {Rank = rank, Id = null};
            StateManager.Save(additive);

            Additives[rank - 1] = additive;
        }

        public void MoveAdditive()
        {
            if (AdditiveToMove == null)
            {
                AdditiveToMove = SelectedAdditive;
                return;
            }

            if (AdditiveToMove == SelectedAdditive)
            {
                ToastNotification.Notify("You selected the same additive",NotificationType.Warning);
                AdditiveToMove = null;
                SelectedAdditive = null;
                return;
            }

            
        }

        public void DeleteAdditive()
        {
            if (SelectedAdditive == null)
            {
                return;
            }

            WarningViewModel = new WarningViewModel("Are you sure to delete this Order?", "Check", "Ok", "Close", "No",
                (o) => DeleteAdditiveAction(o), this, () => IsDialogOpen = false);
            DialogViewModel = WarningViewModel;
            IsDialogOpen = true;


        }

        public void DeleteAdditiveAction(object param)
        {

            if (SelectedAdditive?.Id == null)
            {
                ToastNotification.Notify("Select an Additive to delete first ", NotificationType.Warning);
                return;
            }

            var selectedAdditiveId = (long)SelectedAdditive.Id;

            var additiveToDelete = SelectedAdditive;


            //additiveToDelete.Rank = null;


            if (StateManager.Delete(additiveToDelete))
            {
                //Additives.Remove(SelectedAdditive);
                var index = Additives.IndexOf(additiveToDelete);
                Additives[index] = new Additive() { Rank = additiveToDelete.Rank };
                additiveToDelete = null;
                SelectedAdditive = null;
                ToastNotification.Notify("Additive was deleted successfully", NotificationType.Success);

            }
            else
            {
                ToastNotification.Notify("Something happened", NotificationType.Error);
            }
            IsDialogOpen = false;
        }
  
        public void BackToLogin()
        {
            LoginViewModel loginvm = new LoginViewModel();
            loginvm.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(loginvm);
        }

        public void SelectAdditive(Additive additive)
        {
            if (AdditiveToMove!= null)
            {
                PutAdditiveInCellOf(SelectedAdditive, AdditiveToMove);
                AdditiveToMove = null;
                return;
            }

            if (AdditiveToMove == SelectedAdditive)
            {
                ToastNotification.Notify("You selected the same additive", NotificationType.Warning);
                AdditiveToMove = null;
                SelectedAdditive = null;
            }
        }
    }
}