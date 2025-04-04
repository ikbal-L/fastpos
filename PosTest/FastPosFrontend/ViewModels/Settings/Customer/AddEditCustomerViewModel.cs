﻿using System;
using System.Linq;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using ServiceInterface.ExtentionsMethod;

using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels.Settings.Customer
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
        private ServiceInterface.Model.Customer _Customer;
        public ServiceInterface.Model.Customer Customer
        {
            get { return _Customer; }
            set {
                _Customer = value;
                NotifyOfPropertyChange(nameof(Customer));
            }
        }
        private BindableCollection<string> numbres;
        public BindableCollection<string> Numbers
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
        private CustomerSettingsViewModel Parent { get; set; }
        public AddEditCustomerViewModel(CustomerSettingsViewModel parent)
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
            Customer = new  ServiceInterface.Model.Customer();
            NewPhoneNumber = "";
            Numbers.Clear();
        }
        public void Save() {
           
            Customer.PhoneNumbers = Numbers;
            if (StateManager.Save(Customer))
            {
                int index=   Parent.Customers.IndexOf(Parent.Customers.FirstOrDefault(x => x.Id == Customer.Id));
                if (index==-1)
                {
                    Parent.Customers.Add(Customer);
                    Parent.NotifyCustomers();
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
        public void ChangeCustomer(ServiceInterface.Model.Customer selectedCustomer)
        { 
            Customer = selectedCustomer.Clone();
            Numbers = selectedCustomer.PhoneNumbers.Clone();
        }
        public void AddPhoneNumber() {
            if (!string.IsNullOrEmpty(NewPhoneNumber))
            {
                Numbers.Add(NewPhoneNumber);
                NewPhoneNumber = "";
            }
            NotifyOfPropertyChange(() => Customer.PhoneNumbers);
        }
        public void DeletePhoneNumber(string number)
        {
            Numbers?.Remove(number);
        }
    }
}
