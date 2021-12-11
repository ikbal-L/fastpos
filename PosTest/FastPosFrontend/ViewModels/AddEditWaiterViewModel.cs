using System.Linq;
using Caliburn.Micro;
using Caliburn.Micro;
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

        public string NewPhoneNumber
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
            Numbers = waiter?.PhoneNumbers ?? new BindableCollection<string>();

        }
        public void NewWaiter()
        {
            Waiter =new  ServiceInterface.Model.Waiter();
            Numbers = new BindableCollection<string>();
        }
        public void Save() {

            Waiter.PhoneNumbers = Numbers;
            if (StateManager.Save(Waiter))
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
                
            }
            else
            {
                

            }
        }
        public void Close()
        {
            IsOpenDailog = false;
            Parent.SelectedWaiter = null;
        }

        private BindableCollection<string> _numbers;

        public BindableCollection<string>  Numbers
        {
            get { return _numbers; }
            set {Set(ref _numbers, value); }
        }

        public void AddPhoneNumber()
        {
            if (!string.IsNullOrEmpty(NewPhoneNumber))
            {
                Numbers?.Add(NewPhoneNumber);
                NewPhoneNumber = "";
            }
            NotifyOfPropertyChange(() => Waiter.PhoneNumbers);
        }
        public void DeletePhoneNumber(string number)
        {
            Numbers?.Remove(number);
        }
    }
}
