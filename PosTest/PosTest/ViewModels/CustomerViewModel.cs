using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using PosTest.Helpers;
using PosTest.ViewModels.SubViewModel;
using ServiceInterface.Interface;
using ServiceLib.Service;

namespace PosTest.ViewModels
{
    public class CustomerViewModel : Screen

    {
        private ICollectionView _CustomerView;
        private string _FilterString ="";
        private Customer _selectedCustomer;
        private bool _isEditing;
        private CustomerDetailViewModel _customerDetailVm;
        public CheckoutViewModel ParentChechoutVM { get; set; }
      
        public CustomerViewModel(CheckoutViewModel checkoutViewModel/*,ICustomerService customerService*/)
        {
            //_customerService = customerService;
            ParentChechoutVM = checkoutViewModel;

            //_CustomerView = CollectionViewSource.GetDefaultView(ParentChechoutVM.Customers);
            CustomerCollectionViewSource = new CollectionViewSource {Source = ParentChechoutVM.Customers};
            CustomerCollectionViewSource.Filter += CustomerCollectionViewSource_Filter;
            
            //_CustomerView.Filter = CustomerFilter;
            SelectCustomerCommand = new DelegateCommandBase(SelectCustomer,CanSelectCustomer);
            IsEditing = false;

        }

        private void CustomerCollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            var customer = (Customer) e.Item;
            string name = Regex.Replace(FilterString, @"\d", "").ToLowerInvariant();
            string mobile = Regex.Replace(FilterString, @"\D", "");

            
            if (customer.Name.ToLowerInvariant().Contains(name))
            {
                if (customer.Mobile!= null && !customer.Mobile.Contains(mobile))
                {
                    e.Accepted = false;
                }

                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        public ICollectionView Customers
        {
            get => _CustomerView;
        }

        public CollectionViewSource CustomerCollectionViewSource { get; set; }

        public ICommand SelectCustomerCommand { get; set; }
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
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
                //_CustomerView.Refresh(); 
                CustomerCollectionViewSource.View.Refresh();
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
            if (string.IsNullOrEmpty(FilterString)|| string.IsNullOrWhiteSpace(FilterString)) 
                return true;
            try
            {
                Customer customer = item as Customer;
                return customer.Name.ToLower().Contains(FilterString.ToLower());//|| customer.Mobile.Contains(FilterString);
            }
            catch
            {
                return true;
            }
            
        }

        public void CreateAndSave()
        {
            string name= Regex.Replace(FilterString, @"\d", "");
            string mobile = Regex.Replace(FilterString, @"\D", "");
           
            
            Customer customer = new Customer { Name = name, Mobile = mobile };
            var errors =EntityValidationHelper.Validate(customer);
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ToastNotification.Notify(error,NotificationType.Error);
                }
                return;
            }

            customer.Mobile = "+213" + customer.Mobile;
            StateManager.Save(customer);
            //customer.Id = id;
           
            ParentChechoutVM.Customers.Add(customer);
            
            //_CustomerView.Refresh();
            CustomerCollectionViewSource.View.Refresh();

            if (ParentChechoutVM.CurrentOrder == null)
            {
                ParentChechoutVM.NewOrder();
            }

            ParentChechoutVM.CurrentOrder.Customer =customer;
            SelectedCustomer = customer;

            FilterString = "";
        }

        

        public void CreateAndEdit()
        {
            string mobile = Regex.Replace(FilterString, @"\D", "");
            string name = Regex.Replace(FilterString, @"\d", "");

            Customer customer = new Customer { Name = name, Mobile = mobile };
            
            CustomerDetailVm = new CustomerDetailViewModel(customer);
            CustomerDetailVm.CommandExecuted += CustomerDetailViewModel_CommandExecuted;
            IsEditing = true;
            FilterString = string.Empty;
            
        }

        private void CustomerDetailViewModel_CommandExecuted(object sender, CommandExecutedEventArgs e)
        {
            if (e.CommandName.ToLowerInvariant() == "save"|| e.CommandName.ToLowerInvariant() == "cancel")
            {
                ParentChechoutVM.Customers.Add(CustomerDetailVm.Customer);
                CustomerCollectionViewSource.View.Refresh();
                IsEditing = false;
            }
        }

        public CustomerDetailViewModel CustomerDetailVm
        {
            get => _customerDetailVm;
            set => Set(ref _customerDetailVm, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => Set(ref _isEditing, value);
        }
    }
}
