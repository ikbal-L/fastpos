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
        private long? _reportId;
        public DailyExpenseReportInputDataViewModel(Screen owner,DailyExpenseReport report = null)
        {
            _owner = owner;
           
           ReportInputData = new DailyExpenseReportInputData();
            if (report!= null)
            {
                _reportId = report.Id;
                CashRegisterInitialAmount = report.CashRegisterInitialAmount.ToString();
                CashRegisterActualAmount = report.CashRegisterActualAmount.ToString();
                Expenses = new ObservableCollection<Expense>(report.Expenses.Select((kvp)=>new Expense() {Description=kvp.Key,Amount=kvp.Value }));
            }
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
                if (Expenses.Any(e=>e.Description == ExpenseForm.ExpenseDescription))
                {
                    ToastNotification.Notify("The added Item already exists!");
                    return;
                }
                Expenses.Add(new Expense() { Description = ExpenseForm.ExpenseDescription, Amount = amount });
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
            string url = "";
            var api = new RestApis();
            ReportInputData.CashRegisterActualAmount = decimal.Parse(CashRegisterActualAmount);
            ReportInputData.CashRegisterInitialAmount = decimal.Parse(CashRegisterInitialAmount);
            ReportInputData.Expenses = Expenses.ToDictionary(expense => expense.Description, expense => expense.Amount);
            if (_reportId!= null)
            {
                url = api.Action("dailyExpenseReport", EndPoint.Put, arg: _reportId);
            }
            else
            {
                url = api.Action("dailyExpenseReport", EndPoint.Save);
            }
            int status = -1; DailyExpenseReport result = null ;
            if (_reportId == null)
            {
                 (status,result) =GenericRest.PostThing<DailyExpenseReport>(api.Action("dailyExpenseReport", EndPoint.Save), ReportInputData);
            }
            else
            {
                (status, result) = GenericRest.UpdateThing<DailyExpenseReport>(url, ReportInputData);
            }
          
            if (status == 201|| status ==200)
            {
                
                _reportGenerated?.Invoke(result);
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
}