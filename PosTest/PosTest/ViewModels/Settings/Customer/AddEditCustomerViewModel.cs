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
        private CustomerSettengsViewModel Parent { get; set; }
        public AddEditCustomerViewModel(CustomerSettengsViewModel parent)
        {
            Parent = parent;
        }
        public void ChangeDailogSatate(bool value)
        {
            IsOpenDailog = value;
        }
   
        public void NewCustome()
        {
            _Customer = new  Customer();
        }
        public void Save() {
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
            Parent.SelectedCustomer = null;
        }

        internal void ChangeCustomer(Customer selectedCustomer)
        {
            Customer = selectedCustomer.Clone();
        }
    }
}
