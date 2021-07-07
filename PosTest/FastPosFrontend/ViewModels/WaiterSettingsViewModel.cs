﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using FastPosFrontend.Events;
using FastPosFrontend.Helpers;
using FastPosFrontend.ViewModels.Settings;
using FastPosFrontend.Views;
using FastPosFrontend.Views.Settings;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{

    [NavigationItemConfiguration("Waiter Settings", typeof(WaiterSettingsViewModel), groupName: "Settings")]
    public class WaiterSettingsViewModel: LazyScreen,ISettingsController
    {
        private ObservableCollection<ServiceInterface.Model.Waiter> _Waiters;

        public ObservableCollection<ServiceInterface.Model.Waiter> Waiters
        {
            get { return _Waiters; }
            set {
                _Waiters = value;
                NotifyOfPropertyChange((nameof(Waiters)));
            }
        }
        private ServiceInterface.Model.Waiter _SelectedWaiter;

        public ServiceInterface.Model.Waiter SelectedWaiter
        {
            get { return _SelectedWaiter; }
            set { _SelectedWaiter = value;
                NotifyOfPropertyChange(nameof(SelectedWaiter));
                NotifyOfPropertyChange(nameof(DeleteVisibility));
                NotifyOfPropertyChange(nameof(EditVisibility));
            }
        }
        private UserControl _DailogContent;

        public UserControl DailogContent
        {
            get { return _DailogContent; }
            set { _DailogContent = value;
                NotifyOfPropertyChange(nameof(DailogContent));
            }
        }
        private AddEditWaiterViewModel _AddEditWaiterViewModel;
        private AddEditWaiterView _addEditWaiterView;
        public WaiterSettingsViewModel() {

            Setup();
            OnReady();
           
        }
        public bool DeleteVisibility { get => SelectedWaiter != null; }
        public bool EditVisibility { get => SelectedWaiter != null; }
        public void AddWiaterAction() {
            _AddEditWaiterViewModel.NewWaiter();
            _AddEditWaiterViewModel.ChangeDailogSatate(true);
        }
        public void EditWiaterAction()
        {
            _AddEditWaiterViewModel.ChangeWaiter(SelectedWaiter);
            _AddEditWaiterViewModel.ChangeDailogSatate(true);
        }
        public void DeleteWiaterAction() {
            if (SelectedWaiter == null)
            {
                ToastNotification.Notify("Selected Waiter First" ,NotificationType.Warning);
            }


            DailogContent = new DialogView()
            {
                DataContext = new DialogViewModel("Are you sure to delete this Waiter?", "Check", "Ok", "Close", "No",
                (e) =>
                {
                    if (StateManager.Delete<ServiceInterface.Model.Waiter>(SelectedWaiter))
                    {

                        Waiters.Remove(SelectedWaiter);
                        SelectedWaiter = null;
                        DailogContent = _addEditWaiterView;
                        ToastNotification.Notify("Delete  Success", NotificationType.Success);
                    }
                    else
                    {
                        ToastNotification.Notify("Delete  Fail");
                    }
                },null,()=> {
                    DailogContent = _addEditWaiterView;
                })
            };
            
        }

        protected override void Setup()
        {
            _data = new NotifyAllTasksCompletion(StateManager.GetAsync<Waiter>());
        }

        public override void Initialize()
        {
            Waiters = new ObservableCollection<ServiceInterface.Model.Waiter>(StateManager.Get<ServiceInterface.Model.Waiter>());
            _AddEditWaiterViewModel = new AddEditWaiterViewModel(this);
            _addEditWaiterView = new AddEditWaiterView() { DataContext = _AddEditWaiterViewModel };
            DailogContent = _addEditWaiterView;
        }

        public event EventHandler<SettingsUpdatedEventArgs> SettingsUpdated;

        public void RaiseSettingsUpdated()
        {
            SettingsUpdated?.Invoke(this,new SettingsUpdatedEventArgs(Waiters));
        }

        public override void BeforeNavigateAway()
        {
            RaiseSettingsUpdated();
        }
    }
}
