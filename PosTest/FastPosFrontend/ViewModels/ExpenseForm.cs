using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using Attributes;
using Caliburn.Micro;
using Newtonsoft.Json;

namespace FastPosFrontend.ViewModels
{
    public class ExpenseForm : PropertyChangedBase,INotifyDataErrorInfo
    {
        private string _expenseDescription;
        private string _expenseAmount;
        private bool _isExpenseFormClear = true;

        private readonly Dictionary<string, ICollection<string>> _validationErrors =
            new Dictionary<string, ICollection<string>>();

        public ExpenseForm()
        {
            ValidateModelProperty(this, ExpenseAmount, nameof(ExpenseAmount));
            ValidateModelProperty(this, ExpenseDescription, nameof(ExpenseDescription));

        }

        public bool IsExpenseFormClear
        {
            get => _isExpenseFormClear;
            set
            {
                Set(ref _isExpenseFormClear, value);
                NotifyOfPropertyChange(() => HasErrors);
                RaiseErrorsChanged(nameof(ExpenseAmount));
                RaiseErrorsChanged(nameof(ExpenseDescription));
            }
        }

        [Required(ErrorMessage = "Expense Description is required")]
        [MinLength(5)]
        [RegularExpression(@"^[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z0-9-_\s]*$",
            ErrorMessage = "Use Latin, Arabic or Numeric Characters only")]
        public string ExpenseDescription
        {
            get => _expenseDescription;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    IsExpenseFormClear = false;
                }
                else
                {
                    if (ExpenseAmount == null)
                    {
                        IsExpenseFormClear = true;
                    }
                }

                Set(ref _expenseDescription, value);
                ValidateModelProperty(this, ExpenseDescription);
            }
        }

        [Min("1", ErrorMessage = "Expense Amount Must Be greater than 0")]
        [Decimal]
        [Required(ErrorMessage = "Expense Amount is required")]
        public string ExpenseAmount
        {
            get => _expenseAmount;
            set
            {
                if (value != null)
                {
                    IsExpenseFormClear = false;
                }
                else
                {
                    if (string.IsNullOrEmpty(ExpenseDescription))
                    {
                        IsExpenseFormClear = true;
                    }
                }

                Set(ref _expenseAmount, value);
                ValidateModelProperty(this, ExpenseAmount);
            }
        }


        protected void ValidateModelProperty(object instance, object value, [CallerMemberName] string propertyName = "")
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

            if (IsExpenseFormClear)
            {
                RaiseErrorsChanged(nameof(ExpenseAmount));
                RaiseErrorsChanged(nameof(ExpenseDescription));
                return;
            }

            RaiseErrorsChanged(propertyName);
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            NotifyOfPropertyChange(() => HasErrors);
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)
                || !_validationErrors.ContainsKey(propertyName))
                return null;

            return _validationErrors[propertyName];
        }

        public bool HasErrors => !IsExpenseFormClear && _validationErrors.Any();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    }
}