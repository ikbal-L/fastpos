using System;
using System.Linq;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using ServiceInterface.ExtentionsMethod;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
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
        private BindableCollection<String> numbres;

        public BindableCollection<String> Numbers
        {
            get => numbres;
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

        private DeliveryManSettingsViewModel Parent { get; set; }
        public AddEditDeliveryManViewModel(DeliveryManSettingsViewModel parent)
        {
            Parent = parent;
            Numbers = new BindableCollection<string>();
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
                
                Numbers = Deliveryman.PhoneNumbers!=null? Deliveryman.PhoneNumbers : new BindableCollection<string>();
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
            Numbers = Deliveryman.PhoneNumbers.Clone()??new BindableCollection<string>();

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
                Numbers.Add(NewPhoneNumner);
                NewPhoneNumner = "";
            }
            NotifyOfPropertyChange(() => Deliveryman.PhoneNumbers);
        }
        public void DeletePhoneNumber(string number)
        {
            Numbers.Remove(number);
           // this.Deliveryman.PhoneNumbers.Remove(number);
        }
    }
}
