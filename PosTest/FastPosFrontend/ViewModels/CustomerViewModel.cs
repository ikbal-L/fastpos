using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Input;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using FastPosFrontend.Views;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{
    public class CustomerViewModel : Screen

    {
        private ICollectionView _CustomerView;
        private string _FilterString ="";
        private Customer _selectedCustomer;
        private bool _isEditing;
        private CustomerDetailViewModel _customerDetailVm;
        private CustomerDetailView _detailView;
        public CheckoutViewModel ParentChechoutVM { get; set; }
      
        public CustomerViewModel(CheckoutViewModel checkoutViewModel)
        {

            ParentChechoutVM = checkoutViewModel;


            CustomerCollectionViewSource = new CollectionViewSource {Source = ParentChechoutVM.Customers};
            CustomerCollectionViewSource.Filter += CustomerCollectionViewSource_Filter;

            SelectCustomerCommand = new DelegateCommandBase(SelectCustomer);
            IsEditing = false;

        }

        private void CustomerCollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            var customer = (Customer) e.Item;
            var (name, mobile) = GetNameAndMobile();


            if (customer.Name.ToLowerInvariant().Contains(name))
            {
                if (customer.Mobile!= null && !customer.PhoneNumbers.Any(m=>m.Contains(mobile)))
                {
                    e.Accepted = false;
                }
                else
                {
                    e.Accepted = true;
                }

               
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

                if (ParentChechoutVM.CurrentOrder!=null)
                {
                    ParentChechoutVM.CurrentOrder.Customer = value; 
                }
                
                //ParentChechoutVM.IsTopDrawerOpen = false;
                NotifyOfPropertyChange(nameof(SelectedCustomer));
            }
        }
        
        public string FilterString
        {
            get => _FilterString;
            set {
                Set(ref _FilterString, value);

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
            string name= Regex.Replace(FilterString, @"\s[0-9]+", "");
            string mobile = Regex.Replace(FilterString, @"\w+\s", "");
           
            
            Customer customer = new Customer { Name = name, Mobile = mobile, PhoneNumbers = new BindableCollection<string>() { mobile } };
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
            if (StateManager.Save(customer))
            {
                ParentChechoutVM.Customers.Add(customer);

                //_CustomerView.Refresh();
                CustomerCollectionViewSource.View.Refresh();

            };
            //customer.Id = id;
           
            

            if (ParentChechoutVM.CurrentOrder == null)
            {
                ParentChechoutVM.NewOrder();
            }

            ParentChechoutVM.CurrentOrder.Customer =customer;
            SelectedCustomer = customer;

            FilterString = "";
        }

        
        public (string name ,string mobile) GetNameAndMobile()
        {
            string name = Regex.Match(FilterString, @"([^\d\W][1-9]*\s*)+").Value.Trim();
            string mobile = Regex.Match(FilterString, @"\s?[0-9]+").Value.Trim();
            return (name, mobile);
        }
        public void CreateAndEdit()
        {

            var (name, mobile) = GetNameAndMobile();
            Customer customer = new Customer { Name = name };
            if (!string.IsNullOrWhiteSpace(mobile))
            {
                //customer?.PhoneNumbers?.Add(mobile);
                customer.Mobile = mobile;
            }
            CustomerDetailVm = new CustomerDetailViewModel(customer);
            CustomerDetailVm.CommandExecuted += CustomerDetailViewModel_CommandExecuted;
            IsEditing = true;

            FilterString = string.Empty;
            
        }

        private void CustomerDetailViewModel_CommandExecuted(object sender, CommandExecutedEventArgs e)
        {
            if (e.CommandName.ToLowerInvariant() == "save"|| e.CommandName.ToLowerInvariant() == "cancel")
            {
                
                if (e.CommandName.ToLowerInvariant() == "save")
                {
                    var customer = CustomerDetailVm.Customer;
                    ParentChechoutVM.Customers.Add(customer);
                    CustomerCollectionViewSource.View.Refresh();
                    SelectedCustomer = customer;
                }
                IsEditing = false;
                CustomerDetailVm = null;


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
