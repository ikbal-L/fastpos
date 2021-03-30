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
using ServiceLib.Service.StateManager;

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
        private BindableCollection<String> numbres;
        public BindableCollection<String> Numbers
        {
            get { return numbres; }
            set { numbres = value;

                NotifyOfPropertyChange(nameof(Numbers));
            }
        }
        private string _newPhoneNumber;
        public string NewPhoneNumber
        {
            get { return _newPhoneNumber; }
            set { _newPhoneNumber = value;
                NotifyOfPropertyChange(nameof(NewPhoneNumber));
            }
        }
        private CustomerSettengsViewModel Parent { get; set; }
        public AddEditCustomerViewModel(CustomerSettengsViewModel parent)
        {
            Parent = parent;
            Numbers = new BindableCollection<string>();
        }
        public void ChangeDailogSatate(bool value)
        {
            IsOpenDailog = value;
        }
        public void NewCustome()
        {
            Customer = new  Customer();
            NewPhoneNumber = "";
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
                    Parent.NotifyCustomers();
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
        public void ChangeCustomer(Customer selectedCustomer)
        { 
            Customer = selectedCustomer.Clone();
            Numbers = selectedCustomer.PhoneNumbers.Clone();
        }
        public void AddPhoneNumber() {
            if (!string.IsNullOrEmpty(NewPhoneNumber))
            {
                this.Numbers.Add(NewPhoneNumber);
                NewPhoneNumber = "";
            }
            NotifyOfPropertyChange(() => this.Customer.PhoneNumbers);
        }
        public void DeletePhoneNumber(string number)
        {
            this.Customer.PhoneNumbers.Remove(number);
        }
    }
}
