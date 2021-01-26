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
        private Customer _SelectedCustomer;

        public Customer SelectedCustomer
        {
            get { return _SelectedCustomer; }
            set {
                _SelectedCustomer = value;
                NotifyOfPropertyChange(nameof(SelectedCustomer));
                NotifyOfPropertyChange(nameof(DeleteVisibility));
                NotifyOfPropertyChange(nameof(EditVisibility));
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
        public CustomerSettengsViewModel() {

            this.Title = "Customers";
            this.Content = new CustomerSettingsView() { DataContext = this };
            Customers = new ObservableCollection<Customer>(StateManager.Get<Customer>());
            _AddEditCustomerViewModel = new AddEditCustomerViewModel(this);
            _AddEditCustomerView = new AddEditCustomerView() { DataContext = _AddEditCustomerViewModel };
            DailogContent = _AddEditCustomerView;
        }
        public bool DeleteVisibility { get => SelectedCustomer != null; }
        public bool EditVisibility { get => SelectedCustomer != null; }
        public void AddCustomerAction() {
            _AddEditCustomerViewModel.NewCustome();
            _AddEditCustomerViewModel.ChangeDailogSatate(true);
        }
        public void EditCustomerAction()
        {
            _AddEditCustomerViewModel.ChangeCustomer(SelectedCustomer);
            _AddEditCustomerViewModel.ChangeDailogSatate(true);
        }
        public void DeleteCustomerAction() {
            if (SelectedCustomer == null)
            {
                ToastNotification.Notify("Selected Customer First", NotificationType.Warning);
            }


            DailogContent = new DialogView()
            {
                DataContext = new DialogViewModel("Are you sure to delete this Customer ?", "Check", "Ok", "Close", "No",
                (e) =>
                {
                    if (StateManager.Delete<Customer>(SelectedCustomer))
                    {

                        Customers.Remove(SelectedCustomer);
                        SelectedCustomer = null;
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
