using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using Attributes;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using Newtonsoft.Json;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using ServiceLib.Service;

namespace FastPosFrontend.ViewModels
{
    public class DailyExpenseReportInputDataViewModel : DialogContent, INotifyDataErrorInfo,IDisposable
    {
        private readonly Screen _owner;
        private DailyExpenseReportInputData _reportInputData;
        
        private Expense _selectedExpense;
        private IEnumerable<Expense> _selectedExpenses;
        

        private readonly Dictionary<string, ICollection<string>> _validationErrors =
            new Dictionary<string, ICollection<string>>();

        private string _cashRegisterActualAmount;
        private string _cashRegisterInitialAmount;
        private ExpenseForm _expenseForm;

        public DailyExpenseReportInputDataViewModel(Screen owner)
        {
            _owner = owner;
           ReportInputData = new DailyExpenseReportInputData();
           ValidateModelProperty(this,CashRegisterInitialAmount,nameof(CashRegisterInitialAmount));
           ValidateModelProperty(this,CashRegisterActualAmount,nameof(CashRegisterActualAmount));
           ExpenseForm = new ExpenseForm();
            ExpenseForm.ErrorsChanged += ExpenseForm_ErrorsChanged;
        }

        private void ExpenseForm_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            NotifyOfPropertyChange(nameof(CanSave));
        }

        public DailyExpenseReportInputData ReportInputData
        {
            get => _reportInputData;
            set => Set(ref _reportInputData, value);
        }

        

        [Min("1", ErrorMessage = "CashRegister Actual Amount Must Be greater than 0")]
        [Decimal]
        [Required(ErrorMessage = "CashRegister Actual Amount Is Required")]
        public string CashRegisterActualAmount
        {
            get => _cashRegisterActualAmount;
            set
            {
                Set(ref _cashRegisterActualAmount, value);
                ValidateModelProperty(this, value);
            }
        }

        [Min("1", ErrorMessage = "CashRegister Initial Amount Must Be greater than 0")]
        [Decimal]
        [Required(ErrorMessage = "CashRegister Initial Amount Is Required")]
        public string CashRegisterInitialAmount
        {
            get => _cashRegisterInitialAmount;
            set
            {
                Set(ref _cashRegisterInitialAmount, value);
                ValidateModelProperty(this,value);
            }
        }

        public Expense SelectedExpense
        {
            get => _selectedExpense;
            set => Set(ref _selectedExpense, value);
        }

        public IEnumerable<Expense> SelectedExpenses
        {
            get => _selectedExpenses;
            set => Set(ref _selectedExpenses, value);
        }

        public ExpenseForm ExpenseForm
        {
            get => _expenseForm;
            set => Set(ref _expenseForm, value);
        }


        public ObservableCollection<Expense> Expenses { get; set; } = new ObservableCollection<Expense>();

        public void AddExpense()
        {
            if (decimal.TryParse(ExpenseForm.ExpenseAmount, out decimal amount))
            {
                Expenses.Add(new Expense() {Description = ExpenseForm.ExpenseDescription, Amount = amount});
            }
        }

        public void RemoveExpense(Collection<object> expenses)
        {
            expenses?.Cast<Expense>()?.ToList().ForEach(expense => { Expenses.Remove(expense); });
        }

        public void OnReportGenerated(Action<DailyExpenseReport> handler)
        {
            _reportGenerated = handler;
        }

        private Action<DailyExpenseReport> _reportGenerated;

        protected void ValidateModelProperty(object instance, object value, [CallerMemberName] string propertyName = "")
        {
            if (_validationErrors.ContainsKey(propertyName))
                _validationErrors.Remove(propertyName);

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext =
                new ValidationContext(instance, null, null) {MemberName = propertyName};
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

        private void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            NotifyOfPropertyChange(() => HasErrors);
            NotifyOfPropertyChange(() => CanSave);
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)
                || !_validationErrors.ContainsKey(propertyName))
                return null;

            return _validationErrors[propertyName];
        }

        public bool HasErrors =>  _validationErrors.Any();

        public bool CanSave =>!(ExpenseForm.HasErrors ||HasErrors);
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public void Save()
        {
            ReportInputData.CashRegisterActualAmount = decimal.Parse(CashRegisterActualAmount);
            ReportInputData.CashRegisterInitialAmount = decimal.Parse(CashRegisterInitialAmount);
            ReportInputData.Expenses = Expenses.ToDictionary(expense => expense.Description, expense => expense.Amount);
            var api = new RestApis();
            var result =
                GenericRest.PostThing<DailyExpenseReport>(api.Action("dailyExpenseReport", EndPoint.Save), ReportInputData);
            if (result.status == 201)
            {
                
                _reportGenerated?.Invoke(result.Item2);
            }

            
            Host.Close(this);
        }

        public void Cancel()
        {
            Host?.Close(this);
        }


        private void ReleaseUnmanagedResources()
        {
            
            ExpenseForm.ErrorsChanged -= ExpenseForm_ErrorsChanged;
            ExpenseForm = null;
            _reportGenerated = null;
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~DailyExpenseReportInputDataViewModel()
        {
            ReleaseUnmanagedResources();
        }
    }

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