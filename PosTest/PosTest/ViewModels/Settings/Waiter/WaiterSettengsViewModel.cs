using PosTest.Helpers;
using PosTest.ViewModels.SubViewModel;
using PosTest.Views.Settings;
using PosTest.Views.SubViews;
using ServiceInterface.Model;
using ServiceLib.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PosTest.ViewModels.Settings
{
    public class WaiterSettengsViewModel: SettingsItemBase
    {
        private ObservableCollection<Waiter> _Waiters;

        public ObservableCollection<Waiter> Waiters
        {
            get { return _Waiters; }
            set {
                _Waiters = value;
                NotifyOfPropertyChange((nameof(Waiters)));
            }
        }
        private Waiter _SelectedWaiter;

        public Waiter SelectedWaiter
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
        public WaiterSettengsViewModel() {

            this.Title = "Waiter";
            this.Content = new WaiterSettingsView() { DataContext = this };
            Waiters = new ObservableCollection<Waiter>(StateManager.Get<Waiter>());
            _AddEditWaiterViewModel = new AddEditWaiterViewModel(this);
            _addEditWaiterView = new AddEditWaiterView() { DataContext = _AddEditWaiterViewModel };
            DailogContent = _addEditWaiterView;
        }
        public bool DeleteVisibility { get => SelectedWaiter != null; }
        public bool EditVisibility { get => SelectedWaiter != null; }
        public void AddWiaterAction() {
            _AddEditWaiterViewModel.ChangeWaiter(new Waiter());
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
                    if (StateManager.Delete<Waiter>(SelectedWaiter))
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
    }
}
