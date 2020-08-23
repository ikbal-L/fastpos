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
        private readonly ICollectionView _CustomerView;

        public CustomerViewModel()
        {
            _Customers = new BindableCollection<Customer>();

            _Customers.Add(new Customer { Id = 1, Mobile = "0560202020", Name = "C.ustomer1" });
            _Customers.Add(new Customer { Id = 2, Mobile = "0560202020", Name = "Customer2" });
            _Customers.Add(new Customer { Id = 3, Mobile = "0560202020", Name = "Customer3" });
            _Customers.Add(new Customer { Id = 4, Mobile = "0560202020", Name = "Customer4" });
            _Customers.Add(new Customer { Id = 5, Mobile = "0560202020", Name = "Customer5" });
            _Customers.Add(new Customer { Id = 6, Mobile = "0560202020", Name = "Customer6" });
            _Customers.Add(new Customer { Id = 7, Mobile = "0560202020", Name = "Customer7" });
            _Customers.Add(new Customer { Id = 8, Mobile = "0560202020", Name = "Customer8" });
            _Customers.Add(new Customer { Id = 9, Mobile = "0560202020", Name = "Customer9" });
            _Customers.Add(new Customer { Id = 10, Mobile = "022020202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "072980202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "072980202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "072980202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "072980202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "072980202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "072980202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "072980202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "062980202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "062980202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "062980202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "062980202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "062920202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "062020202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "062020202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "062020202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "062020202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "062020202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "062020202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "062020202", Name = "Customer10" });
            _Customers.Add(new Customer { Id = 10, Mobile = "062020202", Name = "Customer10" });
            
            _CustomerView = CollectionViewSource.GetDefaultView(_Customers.ToList());
            _CustomerView.Filter = CustomerFilter;

            

        }

        private IList<Customer> _Customers;
        private Customer _SelectedCustomer;
        private string _FilterString;

        public ICollectionView Customers
        {
            get => _CustomerView;
        } 
        
        public Customer SelectedCustomer
        {
            get => _SelectedCustomer;
            set => Set(ref _SelectedCustomer, value);
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
            string mobile = Regex.Replace(FilterString, @"\d", "");
            string name = Regex.Replace(FilterString, @"\D", "");

            Customer customer = new Customer { Name = name, Mobile = mobile };
            
        }

        public void CreateAndEdit()
        {
            string mobile = Regex.Replace(FilterString, @"\d", "");
            string name = Regex.Replace(FilterString, @"\D", "");

            Customer customer = new Customer { Name = name, Mobile = mobile };
        }
    }
}
