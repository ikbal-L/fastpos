using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using FastPosFrontend.Helpers;
using FastPosFrontend.Views.Settings;
using FastPosFrontend.Views.Settings.Customer;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels.Settings.Customer
{
    [NavigationItemConfiguration("Customer Settings",type:typeof(CustomerSettingsViewModel), groupName: "Settings")]
    public class CustomerSettingsViewModel : AppScreen
    {
        private ObservableCollection<ServiceInterface.Model.Customer> _Customers;

        public ObservableCollection<ServiceInterface.Model.Customer> Customers
        {
            get { return _Customers; }
            set {
                _Customers = value;
                NotifyOfPropertyChange((nameof(Customers)));
            }
        }
   
        private UserControl _DailogContent;

        public UserControl DailogContent
        {
            get { return _DailogContent; }
            set { _DailogContent = value;
                NotifyOfPropertyChange(nameof(DailogContent));
            }
        }
        private AddEditCustomerViewModel _AddEditCustomerViewModel;
        private AddEditCustomerView _AddEditCustomerView;
        private string _searchString="";

        public string SearchString
        {
            get { return _searchString; }
            set { _searchString = value;
                NotifyOfPropertyChange(nameof(SearchString));
                NotifyOfPropertyChange(nameof(FilteredCustomers));
            }
        }

        public ObservableCollection<ServiceInterface.Model.Customer> FilteredCustomers { get => new ObservableCollection<ServiceInterface.Model.Customer>(Customers.Where(x => (x.Name?.ToLower().Contains(SearchString?.ToLower())??false) || (x.PhoneNumbers?.Any(n => n.ToLower().Contains(SearchString?.ToLower()))??false) || (x.Debit.ToString()?.ToLower().Contains(SearchString?.ToLower()) ??false))); }

        public CustomerSettingsViewModel() {

            //this.Title = "Customers";
            //this.Content = new CustomerSettingsView() { DataContext = this };
            Customers = new ObservableCollection<ServiceInterface.Model.Customer>(StateManager.Get<ServiceInterface.Model.Customer>());
            _AddEditCustomerViewModel = new AddEditCustomerViewModel(this);
            _AddEditCustomerView = new AddEditCustomerView() { DataContext = _AddEditCustomerViewModel };
            DailogContent = _AddEditCustomerView;
        }

        public void AddCustomerAction() {
            _AddEditCustomerViewModel.NewCustome();
            _AddEditCustomerViewModel.ChangeDailogSatate(true);
        }
        public void EditCustomerAction(ServiceInterface.Model.Customer _SelectedCustomer)
        {
            _AddEditCustomerViewModel.ChangeCustomer(_SelectedCustomer);
            _AddEditCustomerViewModel.ChangeDailogSatate(true);
        }
        public void DeleteCustomerAction(ServiceInterface.Model.Customer _SelectedCustomer) {
            if (_SelectedCustomer == null)
            {
                ToastNotification.Notify("Selected Customer First", NotificationType.Warning);
            }


            DailogContent = new DialogView()
            {
                DataContext = new DialogViewModel("Are you sure to delete this Customer ?", "Check", "Ok", "Close", "No",
                (e) =>
                {
                    if (StateManager.Delete<ServiceInterface.Model.Customer>(_SelectedCustomer))
                    {

                        Customers.Remove(_SelectedCustomer);
                        NotifyOfPropertyChange(nameof(FilteredCustomers));
                        DailogContent = _AddEditCustomerView;
                        ToastNotification.Notify("Delete  Success", NotificationType.Success);
                    }
                    else
                    {
                        ToastNotification.Notify("Delete  Fail");
                    }
                },null,()=> {
                    DailogContent = _AddEditCustomerView;
                })
            };
            
        }
      public void NotifyCustomers() {
            NotifyOfPropertyChange(nameof(FilteredCustomers));

        }
    }
}
