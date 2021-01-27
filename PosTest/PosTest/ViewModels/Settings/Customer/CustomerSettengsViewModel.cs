using PosTest.Helpers;
using PosTest.ViewModels.SubViewModel;
using PosTest.Views.Settings;
using PosTest.Views.SubViews;
using ServiceInterface.Model;
using ServiceLib.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PosTest.ViewModels.Settings
{
    public class CustomerSettengsViewModel : SettingsItemBase
    {
        private ObservableCollection<Customer> _Customers;

        public ObservableCollection<Customer> Customers
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

        public ObservableCollection<Customer> FilteredCustomers { get => new ObservableCollection<Customer>(Customers.Where(x => (x.Name?.ToLower().Contains(SearchString?.ToLower())??false) || (x.PhoneNumbers?.Any(n => n.ToLower().Contains(SearchString?.ToLower()))??false) || (x.Debit.ToString()?.ToLower().Contains(SearchString?.ToLower()) ??false))); }

        public CustomerSettengsViewModel() {

            this.Title = "Customers";
            this.Content = new CustomerSettingsView() { DataContext = this };
            Customers = new ObservableCollection<Customer>(StateManager.Get<Customer>());
            _AddEditCustomerViewModel = new AddEditCustomerViewModel(this);
            _AddEditCustomerView = new AddEditCustomerView() { DataContext = _AddEditCustomerViewModel };
            DailogContent = _AddEditCustomerView;
        }

        public void AddCustomerAction() {
            _AddEditCustomerViewModel.NewCustome();
            _AddEditCustomerViewModel.ChangeDailogSatate(true);
        }
        public void EditCustomerAction(Customer _SelectedCustomer)
        {
            _AddEditCustomerViewModel.ChangeCustomer(_SelectedCustomer);
            _AddEditCustomerViewModel.ChangeDailogSatate(true);
        }
        public void DeleteCustomerAction(Customer _SelectedCustomer) {
            if (_SelectedCustomer == null)
            {
                ToastNotification.Notify("Selected Customer First", NotificationType.Warning);
            }


            DailogContent = new DialogView()
            {
                DataContext = new DialogViewModel("Are you sure to delete this Customer ?", "Check", "Ok", "Close", "No",
                (e) =>
                {
                    if (StateManager.Delete<Customer>(_SelectedCustomer))
                    {

                        Customers.Remove(_SelectedCustomer);
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
        
    }
}
