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
   public class AddEditCustomerViewModel : PropertyChangedBase
    {
        private bool _IsOpenDailog ;

        public bool IsOpenDailog
        {
            get { return _IsOpenDailog; }
            set { _IsOpenDailog = value;
                NotifyOfPropertyChange(nameof(IsOpenDailog));
            }
        }
        private Customer _Customer;

        public Customer Customer
        {
            get { return _Customer; }
            set {
                _Customer = value;
                NotifyOfPropertyChange(nameof(Customer));
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

        private CustomerSettengsViewModel Parent { get; set; }
        public AddEditCustomerViewModel(CustomerSettengsViewModel parent)
        {
            Parent = parent;
            Numbers = new ObservableCollection<string>();
        }
        public void ChangeDailogSatate(bool value)
        {
            IsOpenDailog = value;
        }
   
        public void NewCustome()
        {
            Customer = new  Customer();
            NewPhoneNumner = "";
            Numbers.Clear();
        }
        public void Save() {
           
            Customer.PhoneNumbers = Numbers;
            if (StateManager.Save<Customer>(Customer))
            {
                int index=   Parent.Customers.IndexOf(Parent.Customers.FirstOrDefault(x => x.Id == Customer.Id));
                if (index==-1)
                {
                    Parent.Customers.Add(Customer);
                }
                else
                {
                    Parent.Customers.RemoveAt(index);
                    Parent.Customers.Insert(index, Customer);
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
        }

        internal void ChangeCustomer(Customer selectedCustomer)
        { 
            Customer = selectedCustomer.Clone();
            Numbers.Clear();
            NewPhoneNumner = "";
            if (Customer?.PhoneNumbers!=null)
            foreach (var item in Customer?.PhoneNumbers)
            {
                Numbers.Add(item);
            }
        }
        public void AddPhoneNumber() {
            if(!string.IsNullOrEmpty(NewPhoneNumner))
                this.Numbers.Add(NewPhoneNumner);
            NotifyOfPropertyChange(() => this.Customer.PhoneNumbers);
        }
        public void DeletePhoneNumber(string number)
        {
            this.Customer.PhoneNumbers.Remove(number);
        }
    }
}
