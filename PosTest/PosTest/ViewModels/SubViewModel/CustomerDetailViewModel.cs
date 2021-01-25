using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using ServiceInterface.Model;

namespace PosTest.ViewModels.SubViewModel
{
    public class CustomerDetailViewModel : PropertyChangedBase, INotifyDataErrorInfo
    {
        private Customer _customer;
        private readonly Dictionary<string, ICollection<string>> _validationErrors =
            new Dictionary<string, ICollection<string>>();

        public CustomerDetailViewModel(Customer customer)
        {
            Customer = new Customer(){Id = customer.Id,Name = customer.Name,Mobile = customer.Mobile};
            ValidateModelProperty(Customer,Customer.Name,nameof(Customer.Name));
            ValidateModelProperty(Customer,Customer.Mobile,nameof(Customer.Mobile));
        }
        
        public Customer Customer
        {
            get => _customer;
            set => Set(ref _customer, value);
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)
                || !_validationErrors.ContainsKey(propertyName))
                return null;

            return _validationErrors[propertyName];
        }

        public bool HasErrors => _validationErrors.Any();
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
        }
        public void Save()
        {
            RaiseCommandExecuted();
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