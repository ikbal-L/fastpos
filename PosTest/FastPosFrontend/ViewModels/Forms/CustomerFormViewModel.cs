﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using FastPosFrontend.Helpers;
using Caliburn.Micro;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels.Forms
{
    public class CustomerFormViewModel : PropertyChangedBase, INotifyDataErrorInfo
    {
        private Customer _customer;
        private readonly Dictionary<string, ICollection<string>> _validationErrors =
            new Dictionary<string, ICollection<string>>();

        private bool _isSaveEnabled;
        private string _name = string.Empty;
        private string _mobile;
        private string _selectedPhoneNumber;
        private bool _isEditingPhone = false;

        public CustomerFormViewModel(Customer customer)
        {
            Customer = new Customer()
            {
                Id = customer.Id,
                Name = customer.Name,
                Mobile = customer.Mobile,
                PhoneNumbers = customer.PhoneNumbers,
                Address = customer.Address 
            };
         
            Name = Customer.Name;
            Mobile = Customer.Mobile;
            Address = Customer.Address;
            ValidateModelProperty(this, Name, nameof(Name));
            ValidateModelProperty(Customer, Mobile, nameof(Mobile));
            ValidateModelProperty(Customer, Address, nameof(Address));

        }

        private void Customer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Customer.Name))
            {
                ValidateModelProperty(Customer, Customer.Name, nameof(Customer.Name));
            }

            if (e.PropertyName == nameof(Customer.Mobile))
            {
                ValidateModelProperty(Customer, Customer.Mobile, nameof(Customer.Mobile));
            }
        }

        public Customer Customer
        {
            get => _customer;
            set => Set(ref _customer, value);
        }

        public string Name
        {
            get => _name;
            set
            {
                Set(ref _name, value);
                ValidateModelProperty(this, Name, nameof(Name));
            }
        }

        public string Mobile
        {
            get => _mobile;
            set
            {
                Set(ref _mobile, value);

                ValidateModelProperty(Customer, Mobile, nameof(Mobile));
            }
        }

        private string _address;

        public string Address
        {
            get { return _address; }
            set 
            { 
                Set(ref _address , value);
                ValidateModelProperty(Customer, Address, nameof(Address));
            }
        }


        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)
                || !_validationErrors.ContainsKey(propertyName))
                return null;

            return _validationErrors[propertyName];
        }

        public bool HasErrors => _validationErrors.Any();

        public bool IsSaveEnabled
        {
            get => _isSaveEnabled;
            set => Set(ref _isSaveEnabled, value);
        }

        public bool IsEditingPhone
        {
            get => _isEditingPhone;
            set => Set(ref _isEditingPhone, value);
        }

        public string SelectedPhoneNumber
        {
            get => _selectedPhoneNumber;
            set => Set(ref _selectedPhoneNumber, value);
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public event EventHandler<CommandExecutedEventArgs> CommandExecuted;
        private void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void RaiseCommandExecuted([CallerMemberName]string commandName = "")
        {
            CommandExecuted?.Invoke(this, new CommandExecutedEventArgs(commandName));
        }

        protected void ValidateModelProperty(object instance, object value, string propertyName)
        {
            if (_validationErrors.ContainsKey(propertyName))
                _validationErrors.Remove(propertyName);

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext =
                new ValidationContext(instance, null, null) { MemberName = propertyName };
            if (!Validator.TryValidateProperty(value, validationContext, validationResults))
            {
                _validationErrors.Add(propertyName, new List<string>());
                foreach (ValidationResult validationResult in validationResults)
                {
                    _validationErrors[propertyName].Add(validationResult.ErrorMessage);
                }
            }

            
            RaiseErrorsChanged(propertyName);
            IsSaveEnabled = !_validationErrors.Any();
        }

        public void AddPhone()
        {
            if (!_validationErrors.ContainsKey(nameof(Mobile)))
            {
                AddPhoneToCustomerPhoneList(Mobile);
                return;
            }
            ToastNotification.Notify("Phone Number has validation errors ");
        }

        private void AddPhoneToCustomerPhoneList(string mobile)
        {
            if (!Customer.PhoneNumbers.Contains(mobile))
            {
                Customer.PhoneNumbers.Add(mobile);
            }
            else
            {
                ToastNotification.Notify("Phone number is already added", NotificationType.Warning);
            }
        }

        public void RemovePhone()
        {
            if (SelectedPhoneNumber!= null)
            {
                Customer.PhoneNumbers.Remove(SelectedPhoneNumber);
            }
        }

        public void EditPhone()
        {

            if (IsEditingPhone)
            {
                if (!_validationErrors.ContainsKey(nameof(Mobile)))
                {
                    SelectedPhoneNumber = Mobile;
                    IsEditingPhone = false;
                    return;
                }
                ToastNotification.Notify("Phone Number has validation errors ");
            }
            else
            {
                Mobile = SelectedPhoneNumber;
                IsEditingPhone = true;
            }
            
        }
        public void Save()
        {
            Customer.Mobile = Mobile;
            Customer.Name = Name;
            Customer.Address = Address;
            Customer.PhoneNumbers.Add(Customer.Mobile);
            if (StateManager.Save(Customer))
            {
                ToastNotification.Notify("Customer saved successfully",NotificationType.Success);
                RaiseCommandExecuted();
                return;
            }
            else
            {
                Customer.PhoneNumbers.Clear();
                Customer.Mobile = null;
            }

            ToastNotification.Notify("Something happened", NotificationType.Error);


        }

        public void Cancel()
        {
            RaiseCommandExecuted();
        }
    }

    public class CommandExecutedEventArgs : EventArgs
    {
        public string CommandName { get; }

        public CommandExecutedEventArgs(string commandName)
        {
            CommandName = commandName;
        }
    }

    
}