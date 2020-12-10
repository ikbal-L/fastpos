using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using PosTest.Helpers;
using ServiceInterface.Interface;

namespace PosTest.ViewModels
{
    public class CustomerViewModel : Screen

    {
        private readonly ICustomerService _customerService;
        private ICollectionView _CustomerView;
        private string _FilterString;
        private Customer _selectedCustomer;
        public CheckoutViewModel ParentChechoutVM { get; set; }
      
        public CustomerViewModel(CheckoutViewModel checkoutViewModel,ICustomerService customerService)
        {
            _customerService = customerService;
            ParentChechoutVM = checkoutViewModel;

            _CustomerView = CollectionViewSource.GetDefaultView(ParentChechoutVM.Customers);
            _CustomerView.Filter = CustomerFilter;
            SelectCustomerCommand = new DelegateCommandBase(SelectCustomer,CanSelectCustomer);

        }

        
        public ICollectionView Customers
        {
            get => _CustomerView;
        }


        public ICommand SelectCustomerCommand { get; set; }
        public Customer SelectedCustomer
        {
            get
            {
                return _selectedCustomer;
            }
            set 
            {
                if (ParentChechoutVM.CurrentOrder == null && value!=null)
                {
                    ParentChechoutVM.NewOrder();
                }

                Set(ref _selectedCustomer, value);
                _selectedCustomer = value;

                if (ParentChechoutVM.CurrentOrder!=null)
                {
                    ParentChechoutVM.CurrentOrder.Customer = value; 
                }
                
                ParentChechoutVM.IsTopDrawerOpen = false;
            }
        }
        
        public string FilterString
        {
            get => _FilterString;
            set {
                Set(ref _FilterString, value);
                _CustomerView.Refresh(); 
            }
        }

        public bool CanSelectCustomer(object customer)
        {
            return true;
        }
        public void SelectCustomer(object customer)
        {
            if(customer ==null) return;
            var c = (customer as Customer);
            if (c==SelectedCustomer)
            {
                SelectedCustomer = null;
            }
            else
            {
                SelectedCustomer = c;
            }
        }


        private bool CustomerFilter(object item)
        {
            if (string.IsNullOrEmpty(FilterString)) 
                return true;
            try
            {
                Customer customer = item as Customer;
                return customer.Name.ToLower().Contains(FilterString.ToLower()) || customer.Mobile.Contains(FilterString);
            }
            catch
            {
                return true;
            }
            
        }

        public void CreateAndSave()
        {
            string name= Regex.Replace(FilterString, @"\d", "");
            string mobile = "+213"+Regex.Replace(FilterString, @"\D", "").Remove(0,1);
            Customer customer = new Customer { Name = name, Mobile = mobile };
            _customerService.SaveCustomer(customer, out long id, out IEnumerable<string> errors);
            customer.Id = id;
            ParentChechoutVM.Customers.Add(customer);
            
            _CustomerView.Refresh();

            if (ParentChechoutVM.CurrentOrder == null)
            {
                ParentChechoutVM.NewOrder();
            }

            ParentChechoutVM.CurrentOrder.Customer =customer;
            SelectedCustomer = customer;
        }

        public void CreateAndEdit()
        {
            string mobile = Regex.Replace(FilterString, @"\d", "");
            string name = Regex.Replace(FilterString, @"\D", "");

            Customer customer = new Customer { Name = name, Mobile = mobile };
        }
    }
}
