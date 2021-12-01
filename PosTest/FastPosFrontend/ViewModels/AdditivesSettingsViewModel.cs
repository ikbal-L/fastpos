using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using FastPosFrontend.ViewModels.SubViewModel;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{
    [NavigationItem("Additive settings", typeof(AdditivesSettingsViewModel),"", parentNavigationItem:typeof(CheckoutSettingsViewModel),isQuickNavigationEnabled:true)]
    [PreAuthorize("Create_Additive")]
    public class AdditivesSettingsViewModel : LazyScreen
    {
        private BindableCollection<Additive> _additives;
        private Additive _selectedAdditive;
        private Additive _copySelectedAdditive;
        private List<Additive> _allAdditives;
        private int _additivePageSize = 30;
        private bool _isEditing;
        private Additive _clipBoardAdditive;
        private Additive _additiveToMove;
        private bool _isDialogOpen;
        private INotifyPropertyChanged _dialogViewModel;
        private WarningViewModel _warningViewModel;
       

        public AdditivesSettingsViewModel() : base()
        {
           IsDialogOpen = false;
            _isEditing = false;
        }

        //public AdditivesSettingsViewModel(int additivePageSize):base()
        //{
        //    IsDialogOpen = false;
          
        //    _additivePageSize = additivePageSize;

        //    _isEditing = false;
     
        //}

        public BindableCollection<Additive> Additives
        {
            get => _additives;
            set { Set(ref _additives, value); }
        }

        public Additive SelectedAdditive
        {
            get => _selectedAdditive;
            set
            {
                //CopySelectedAdditive = value?.Clone();
                Set(ref _selectedAdditive, value);
            }
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
            set
            {
                Set(ref _isEditing, value); 
                NotifyOfPropertyChange(nameof(IsEditing));
            }
        }

        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set => Set(ref _isDialogOpen, value);
        }

        public AdditiveDetailViewModel AdditiveDetailViewMode { get; set; }

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
            if (additives.Count > 0)
            {
                maxRank = (int) additives.Max(a => a.Rank);
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

        public void CreateAdditive(object obj)
        {
            if (obj is Additive additive)
            {
                if (SelectedAdditive == null)
                {
                    SelectedAdditive = additive;
                }

                var mVm = Parent as MainViewModel;

                AdditiveDetailViewMode = new AdditiveDetailViewModel(SelectedAdditive, this);
                mVm?.OpenDialog(AdditiveDetailViewMode).OnClose(() =>
                {
                    AdditiveDetailViewMode = null;
                }); 
            }
        }

        

        public void Close()
        {
            (Parent as MainViewModel)?.CloseDialog(AdditiveDetailViewMode);
        }

        public void EditAdditive(object obj)
        {
            if (SelectedAdditive?.Id == null)
            {
                ToastNotification.Notify("Select a non empty cell to edit", NotificationType.Warning);
                return;
            }

            CopySelectedAdditive = SelectedAdditive.Clone();
            //IsEditing = true;
            var mVm = (Parent as MainViewModel);
            AdditiveDetailViewMode = new AdditiveDetailViewModel(SelectedAdditive,this);
            mVm?.OpenDialog(AdditiveDetailViewMode).OnClose(() =>
            {
                
                AdditiveDetailViewMode = null;
            });
        }

        public void CopyAdditive(object obj)
        {
            if (SelectedAdditive?.Id == null)
            {
                ToastNotification.Notify("Select a Valid Additive to copy", NotificationType.Warning);
                return;
            }

            ClipBoardAdditive = SelectedAdditive;
        }

        public void PasteAdditive(object obj)
        {
            if (ClipBoardAdditive == null)
            {
                ToastNotification.Notify("Copy an additive first", NotificationType.Warning);
                return;
            }

            if (SelectedAdditive?.Rank == null)
            {
                ToastNotification.Notify("Select a zone to copy in first", NotificationType.Warning);
                return;
            }

            if (ClipBoardAdditive.Equals(SelectedAdditive))
            {
                ToastNotification.Notify("You chose the same additive", NotificationType.Warning);
                return;
            }

            var rank = (int) SelectedAdditive.Rank;
            var additive = new Additive(ClipBoardAdditive) {Rank = rank, Id = null};
            var description = Regex.Replace(additive.Description, @"\([0-9]{1,2}\)", "");
            var count = Additives.Where(a => a.Description != null).Count(a => a.Description.Contains(description));

            additive.Description = $"{description}({count})";
            if (StateManager.Save(additive))
            {
                Additives[rank - 1] = additive;
            }

            ClipBoardAdditive = null;
        }

        public void MoveAdditive(object obj)
        {
            if (AdditiveToMove == null)
            {
                AdditiveToMove = SelectedAdditive;
                return;
            }

            if (AdditiveToMove == SelectedAdditive)
            {
                ToastNotification.Notify("You selected the same additive", NotificationType.Warning);
                AdditiveToMove = null;
                SelectedAdditive = null;
                return;
            }
        }
        //TODO must add delete set null to additive entity
        public void DeleteAdditive(object obj)
        {
            if (SelectedAdditive == null)
            {
                return;
            }
            var mVm = (Parent as MainViewModel);
            mVm?.OpenDialog(
                DefaultDialog
                    .New("Are you sure to delete this Additive?")
                    .Title("Delete additive")
                    .Ok(o =>
                    {
                        DeleteAdditiveAction();
                        mVm.CloseDialog();
                    })
                    .Cancel(o =>
                    {
                        mVm.CloseDialog();
                    } ));
        }

        public void DeleteAdditiveAction()
        {
            if (SelectedAdditive?.Id == null)
            {
                ToastNotification.Notify("Select an Additive to delete first ", NotificationType.Warning);
                return;
            }

            var selectedAdditiveId = (long) SelectedAdditive.Id;

            var additiveToDelete = SelectedAdditive;


            //additiveToDelete.Rank = null;


            if (StateManager.Delete(additiveToDelete))
            {
                //Additives.Remove(SelectedAdditive);
                var index = Additives.IndexOf(additiveToDelete);
                Additives[index] = new Additive() {Rank = additiveToDelete.Rank};
                additiveToDelete = null;
                SelectedAdditive = null;
                
            }
            else
            {
                ToastNotification.Notify("Something happened", NotificationType.Error);
            }

            IsDialogOpen = false;
        }


        public void SelectAdditive(Additive additive)
        {
            if (AdditiveToMove != null)
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
                return;
            }

            SelectedAdditive = additive;
        }

        protected override void Setup()
        {
            var additivesTask= StateManager.GetAsync<Additive>();
            _data = new NotifyAllTasksCompletion(additivesTask);

            CreateAdditiveCommand = new DelegateCommandBase(CreateAdditive);
            EditAdditiveCommand = new DelegateCommandBase(EditAdditive);
            MoveAdditiveCommand = new DelegateCommandBase(MoveAdditive);
            CopyAdditiveCommand = new DelegateCommandBase(CopyAdditive);
            PasteAdditiveCommand = new DelegateCommandBase(PasteAdditive);
            DeleteAdditiveCommand = new DelegateCommandBase(DeleteAdditive);
        }

        public override void Initialize()
        {

            _allAdditives = StateManager.Get<Additive>().ToList();
            PopulateAdditivesPage();
        }

        public ICommand CreateAdditiveCommand { get; set; }
        public ICommand EditAdditiveCommand { get; set; }
        public ICommand CopyAdditiveCommand { get; set; }
        public ICommand PasteAdditiveCommand { get; set; }
        public ICommand MoveAdditiveCommand { get; set; }
        public ICommand DeleteAdditiveCommand { get; set; }
    }
}