using Caliburn.Micro;
using PosTest.Extensions;
using PosTest.Helpers;
using ServiceInterface.Model;
using ServiceLib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceLib.Service.StateManager;

namespace PosTest.ViewModels.Settings
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
        private Waiter _Waiter;

        public Waiter Waiter
        {
            get { return _Waiter; }
            set { _Waiter = value;
                NotifyOfPropertyChange(nameof(Waiter));
            }
        }
        private WaiterSettengsViewModel Parent { get; set; }
        public AddEditWaiterViewModel(WaiterSettengsViewModel parent)
        {
            Parent = parent;
        }
        public void ChangeDailogSatate(bool value)
        {
            IsOpenDailog = value;
        }
        public void ChangeWaiter(Waiter waiter) {
            Waiter = waiter.Clone();
        }
        public void NewWaiter()
        {
            Waiter =new  Waiter();
        }
        public void Save() {
            if (StateManager.Save<Waiter>(Waiter))
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
