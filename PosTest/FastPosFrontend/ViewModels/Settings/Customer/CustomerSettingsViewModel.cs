using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using FastPosFrontend.Events;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using FastPosFrontend.Views.Settings;
using FastPosFrontend.Views.Settings.Customer;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels.Settings.Customer
{
    [NavigationItem("Customer Settings", target: typeof(CustomerSettingsViewModel), "", groupName: "Settings", isQuickNavigationEnabled: true)]
    [PreAuthorize("Create_customer|Read_Customer|Update_Customer|Delete_Customer")]
    public class CustomerSettingsViewModel : AppScreen, ISettingsController
    {
        public ObservableCollection<ServiceInterface.Model.Customer> Customers { get; set; }

        private UserControl _DailogContent;

        public UserControl DailogContent
        {
            get { return _DailogContent; }
            set
            {
                _DailogContent = value;
                NotifyOfPropertyChange(nameof(DailogContent));
            }
        }

        public ServiceInterface.Model.Customer SelectedCustomer 
        { 
            get => _selectedCustomer; 
            set => Set(ref _selectedCustomer , value); 
        }

        private AddEditCustomerViewModel _AddEditCustomerViewModel;
        private AddEditCustomerView _AddEditCustomerView;
        private string _searchString = "";
        private ServiceInterface.Model.Customer _selectedCustomer;

        public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                NotifyOfPropertyChange(nameof(SearchString));
                NotifyOfPropertyChange(nameof(FilteredCustomers));
            }
        }

        public ObservableCollection<ServiceInterface.Model.Customer> FilteredCustomers { get => new ObservableCollection<ServiceInterface.Model.Customer>(Customers.Where(x => (x.Name?.ToLower().Contains(SearchString?.ToLower()) ?? false) || (x.PhoneNumbers?.Any(n => n.ToLower().Contains(SearchString?.ToLower())) ?? false) || (x.Debit.ToString()?.ToLower().Contains(SearchString?.ToLower()) ?? false))); }

        public CustomerSettingsViewModel()
        {

            Customers = new ObservableCollection<ServiceInterface.Model.Customer>(StateManager.GetAll<ServiceInterface.Model.Customer>());
            _AddEditCustomerViewModel = new AddEditCustomerViewModel(this);
            _AddEditCustomerView = new AddEditCustomerView() { DataContext = _AddEditCustomerViewModel };
            DailogContent = _AddEditCustomerView;
        }

        public void AddCustomerAction()
        {
            _AddEditCustomerViewModel.NewCustome();
            _AddEditCustomerViewModel.ChangeDailogSatate(true);
        }
        public void EditCustomerAction()
        {
            if (SelectedCustomer ==null)
            {
                return;
            }
            _AddEditCustomerViewModel.ChangeCustomer(SelectedCustomer);
            _AddEditCustomerViewModel.ChangeDailogSatate(true);
        }
        public void DeleteCustomerAction()
        {
            if (SelectedCustomer == null)
            {
                ToastNotification.Notify("Select Customer First", NotificationType.Warning);
                return;
            }

            var main = this.Parent as MainViewModel;
            main?.OpenDialog(
                DefaultDialog
                    .New("Are you sure you want perform this action?")
                    .Title("Delete Customer")
                    .Ok(o =>
                    {
                        DeleteCustomer();
                        main.CloseDialog();
                    })
                    .Cancel(o =>
                    {
                        main.CloseDialog();
                    }));


            //var response = ModalDialogBox.YesNo("Are you sure you want to Delete this Customer ?", "Delete Customer").Show();
            //if (response)
            //{
            //    DeleteCustomer();
            //}

           

        }

        public void DeleteCustomer()
        {
            if (StateManager.Delete<ServiceInterface.Model.Customer>(SelectedCustomer))
            {

                Customers.Remove(SelectedCustomer);
                NotifyOfPropertyChange(nameof(FilteredCustomers));
                DailogContent = _AddEditCustomerView;
                ToastNotification.Notify("Delete  Success", NotificationType.Success);
            }
            else
            {
                ToastNotification.Notify("Delete  Fail");
            }
        }
        public void NotifyCustomers()
        {
            NotifyOfPropertyChange(nameof(FilteredCustomers));

        }

        public event EventHandler<SettingsUpdatedEventArgs> SettingsUpdated;

        public void RaiseSettingsUpdated()
        {
            SettingsUpdated?.Invoke(this, new SettingsUpdatedEventArgs(Customers));
        }

        public override void BeforeNavigateAway()
        {
            RaiseSettingsUpdated();
        }
    }
}
