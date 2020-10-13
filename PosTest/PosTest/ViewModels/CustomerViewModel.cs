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

namespace PosTest.ViewModels
{
    public class CustomerViewModel : Screen

    {
        private ICollectionView _CustomerView;
        private string _FilterString;
        private Customer _selectedCustomer;
        public CheckoutViewModel ParentChechoutVM { get; set; }
      
        public CustomerViewModel(CheckoutViewModel checkoutViewModel)
        {
            ParentChechoutVM = checkoutViewModel;
            _CustomerView = CollectionViewSource.GetDefaultView(ParentChechoutVM.Customers.ToList());
            _CustomerView.Filter = CustomerFilter;

        }


        public ICollectionView Customers
        {
            get => _CustomerView;
        } 
        
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
            string mobile = Regex.Replace(FilterString, @"\D", "");
            Customer customer = new Customer { Name = name, Mobile = mobile };
            ParentChechoutVM.Customers.Add(customer);
            
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
