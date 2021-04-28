using System.Linq;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using ServiceInterface.ExtentionsMethod;
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
        private string _phoneNumber;

        public ServiceInterface.Model.Waiter Waiter
        {
            get { return _Waiter; }
            set { _Waiter = value;
                NotifyOfPropertyChange(nameof(Waiter));
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => Set(ref _phoneNumber, value);
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
            PhoneNumber = Waiter.PhoneNumbers?.FirstOrDefault();
        }
        public void NewWaiter()
        {
            Waiter =new  ServiceInterface.Model.Waiter();
        }
        public void Save() {

            Waiter.PhoneNumbers ??= new BindableCollection<string>();
            Waiter.PhoneNumbers.Clear();
            Waiter.PhoneNumbers.Add(PhoneNumber);
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
