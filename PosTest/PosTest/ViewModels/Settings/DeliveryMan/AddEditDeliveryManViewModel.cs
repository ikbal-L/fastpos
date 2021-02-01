using Caliburn.Micro;
using PosTest.Extensions;
using PosTest.Helpers;
using ServiceInterface.Model;
using ServiceLib.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.ViewModels.Settings
{
   public class AddEditDeliveryManViewModel : PropertyChangedBase
    {
        private bool _IsOpenDailog ;

        public bool IsOpenDailog
        {
            get { return _IsOpenDailog; }
            set { _IsOpenDailog = value;
                NotifyOfPropertyChange(nameof(IsOpenDailog));
            }
        }
        private Deliveryman _deliveryman;

        public Deliveryman Deliveryman
        {
            get { return _deliveryman; }
            set {
                _deliveryman = value;
                NotifyOfPropertyChange(nameof(Deliveryman));
            }
        }
        private ObservableCollection<String> numbres;

        public ObservableCollection<String> Numbers
        {
            get { return numbres; }
            set { numbres = value;

                NotifyOfPropertyChange(nameof(Numbers));
            }
        }

        private string _NewPhoneNumner;

        public string NewPhoneNumner
        {
            get { return _NewPhoneNumner; }
            set { _NewPhoneNumner = value;
                NotifyOfPropertyChange(nameof(NewPhoneNumner));
            }
        }

        private DeliveryManSettengsViewModel Parent { get; set; }
        public AddEditDeliveryManViewModel(DeliveryManSettengsViewModel parent)
        {
            Parent = parent;
            Numbers = new ObservableCollection<string>();
        }
        public void ChangeDailogSatate(bool value)
        {
            IsOpenDailog = value;
        }
   
    
        public void Save() {

            Deliveryman.PhoneNumbers = Numbers;
            if (StateManager.Save<Deliveryman>(Deliveryman))
            {
                int index=   Parent.Deliverymans.IndexOf(Parent.Deliverymans.FirstOrDefault(x => x.Id == Deliveryman.Id));
                
                Numbers = Deliveryman.PhoneNumbers!=null?new ObservableCollection<string>(Deliveryman.PhoneNumbers) :new ObservableCollection<string>();
                if (index==-1)
                {
                    Parent.Deliverymans.Add(Deliveryman);
                }
                else
                {
                    Parent.Deliverymans.RemoveAt(index);
                    Parent.Deliverymans.Insert(index, Deliveryman);
                }
                IsOpenDailog = false;
                ToastNotification.Notify("Save  Success", NotificationType.Success);
            }
            else
            {
                ToastNotification.Notify("Save  Fail");

            }
        }

        internal void ChangeDeliveryMan(Deliveryman selectedDeliveryMan)
        {
            Deliveryman = selectedDeliveryMan.Clone();
            Numbers = Deliveryman.PhoneNumbers.Clone();
        }

        internal void NewDeliveryMan()
        {
            Deliveryman = new Deliveryman();
            NewPhoneNumner = "";
            Numbers.Clear();
        }

        public void Close()
        {
            IsOpenDailog = false;
        }

 
        public void AddPhoneNumber() {
            if (!string.IsNullOrEmpty(NewPhoneNumner))
            {
                this.Numbers.Add(NewPhoneNumner);
                NewPhoneNumner = "";
            }
            NotifyOfPropertyChange(() => this.Deliveryman.PhoneNumbers);
        }
        public void DeletePhoneNumber(string number)
        {
            this.Numbers.Remove(number);
           // this.Deliveryman.PhoneNumbers.Remove(number);
        }
    }
}
