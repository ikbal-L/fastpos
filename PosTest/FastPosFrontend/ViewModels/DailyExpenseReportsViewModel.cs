using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Attributes;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using NLog;

namespace FastPosFrontend.ViewModels
{
    [NavigationItemConfiguration(title: "Daily Expense Reports", target: typeof(DailyExpenseReportsViewModel))]
    public class DailyExpenseReportsViewModel : LazyScreen,INotifyDataErrorInfo
    {
        private DailyExpenseReportInputData _reportInputData;
        private bool _isReportGenerated;
        private string _expenseDescription;
        private string _expenseAmount;
        private Expense _selectedExpense;
        private IEnumerable<Expense> _selectedExpenses;
        private bool _isExpenseFormClear = true;
        private readonly Dictionary<string,ICollection<string>> _validationErrors = new Dictionary<string, ICollection<string>>();

        public DailyExpenseReportsViewModel()
        {
            Setup();
            OnReady();
        }

        protected override void Setup()
        {
            _data = new NotifyAllTasksCompletion(Task.Run(() => "Hello"));
            ReportInputData = new DailyExpenseReportInputData();
        }


        public override void Initialize()
        {
            IsReportGenerated = AppConfigurationManager.Configuration<bool>(nameof(IsReportGenerated));
            ValidateModelProperty(this,ExpenseAmount,nameof(ExpenseAmount));
            ValidateModelProperty(this,ExpenseDescription,nameof(ExpenseDescription));
        }

        public DailyExpenseReportInputData ReportInputData
        {
            get => _reportInputData;
            set => Set(ref _reportInputData, value);
        }

        public bool IsReportGenerated
        {
            get => _isReportGenerated;
            set => Set(ref _isReportGenerated, value);
        }

        [Required(ErrorMessage = "Expense Description Name must not be Null or Empty")]
        [MinLength(5, ErrorMessage = "Expense Description must not be under 5 characters ")]
        [RegularExpression(@"^[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z-_\s]*$", ErrorMessage = "Use Latin or Arabic Characters only ")]
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
                ValidateModelProperty(this,ExpenseDescription);
            }
        }

        [Min("1",ErrorMessage = "Expense Amount Must Be greater than 0")]
        [Decimal]
        [Required(ErrorMessage = "Expense Amount must not be Null or Empty")]
        public string ExpenseAmount
        {
            get => _expenseAmount;
            set
            {
                if (value!= null)
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
                ValidateModelProperty(this,ExpenseAmount);
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

        public void AddExpense()
        {
            if (decimal.TryParse(ExpenseAmount, out decimal amount))
            {
                ReportInputData.Expenses.Add(new Expense() { Description = ExpenseDescription, Amount = amount });
            }
           
        }

        public void RemoveExpense(Collection<Object> expenses)
        {
            expenses?.Cast<Expense>()?.ToList().ForEach(expense =>
            {
                ReportInputData.Expenses.Remove(expense);
            } );
        }
        protected void ValidateModelProperty(object instance, object value,[CallerMemberName] string propertyName ="")
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

    public class DailyExpenseReportInputData : PropertyChangedBase
    {
        private decimal _cashRegisterInitialAmount;
        private decimal _cashRegisterActualAmount;
        private DateTime _workTimeStart;
        private DateTime _workTimeEnd;

        public DailyExpenseReportInputData()
        {
            Expenses = new ObservableCollection<Expense>();
            Expenses.Add(new Expense(){Description = "Buy x",Amount = 3700});
            Expenses.Add(new Expense(){Description = "Buy y",Amount = 4800});
            Expenses.Add(new Expense(){Description = "Buy Z",Amount = 2600});
        }

        public decimal CashRegisterInitialAmount
        {
            get => _cashRegisterInitialAmount;
            set => Set(ref _cashRegisterInitialAmount, value);
        }


        public decimal CashRegisterActualAmount
        {
            get => _cashRegisterActualAmount;
            set => Set(ref _cashRegisterActualAmount, value);
        }

        public DateTime WorkTimeStart
        {
            get => _workTimeStart;
            set => Set(ref _workTimeStart, value);
        }

        public DateTime WorkTimeEnd
        {
            get => _workTimeEnd;
            set => Set(ref _workTimeEnd, value);
        }

        public ObservableCollection<Expense> Expenses { get; set; } = new ObservableCollection<Expense>();
    }

    public class Expense:PropertyChangedBase
    {
        public string Description { get; set; }

        public decimal Amount { get; set; }
    }
}