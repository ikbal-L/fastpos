using System.Linq;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using PosTest.Extensions;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{
   public class AddEditWaiterViewModel: PropertyChangedBase
    {
        private bool _IsOpenDailog ;

        public bool IsOpenDailog
        {
            get { return _IsOpenDailog; }
            set { _IsOpenDailog = value;
                NotifyOfPropertyChange(nameof(IsOpenDailog));
            }
        }
        private ServiceInterface.Model.Waiter _Waiter;

        public ServiceInterface.Model.Waiter Waiter
        {
            get { return _Waiter; }
            set { _Waiter = value;
                NotifyOfPropertyChange(nameof(Waiter));
            }
        }
        private WaiterSettingsViewModel Parent { get; set; }
        public AddEditWaiterViewModel(WaiterSettingsViewModel parent)
        {
            Parent = parent;
        }
        public void ChangeDailogSatate(bool value)
        {
            IsOpenDailog = value;
        }
        public void ChangeWaiter(ServiceInterface.Model.Waiter waiter) {
            Waiter = waiter.Clone();
        }
        public void NewWaiter()
        {
            Waiter =new  ServiceInterface.Model.Waiter();
        }
        public void Save() {
            if (StateManager.Save<ServiceInterface.Model.Waiter>(Waiter))
            {
                int index=   Parent.Waiters.IndexOf(Parent.Waiters.FirstOrDefault(x => x.Id == Waiter.Id));
                if (index==-1)
                {
                    Parent.Waiters.Add(Waiter);
                }
                else
                {
                    Parent.Waiters.RemoveAt(index);
                    Parent.Waiters.Insert(index, Waiter);
                }
                IsOpenDailog = false;
                ToastNotification.Notify("Save  Success", NotificationType.Success);
            }
            else
            {
                ToastNotification.Notify("Save  Fail");

            }
        }
        public void Close()
        {
            IsOpenDailog = false;
            Parent.SelectedWaiter = null;
        }
    }
}
