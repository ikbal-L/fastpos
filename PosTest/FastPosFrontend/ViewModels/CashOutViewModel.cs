using Caliburn.Micro;
using FastPosFrontend.Helpers;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace FastPosFrontend.ViewModels
{
    public class CashOutViewModel: DialogContent
    {
        private string _selectedEmployee;
        private ExpenseDescription _selectedExpenditureDescription;

        public string SelectedEmployee
        {
            get { return _selectedEmployee; }
            set 
            { 
                Set(ref _selectedEmployee , value);
                (SaveCommand as DelegateCommandBase).RaiseCanExecuteChanged();
            }
        }

        public ExpenseDescription SelectedExpenditureDescription
        {
            get { return _selectedExpenditureDescription; }
            set { Set(ref _selectedExpenditureDescription , value);

                (SaveCommand as DelegateCommandBase).RaiseCanExecuteChanged();
            }
        }

        private string _expenditureAmount;

        public string ExpenditureAmount
        {
            get { return _expenditureAmount; }
            set 
            { 
                Set(ref _expenditureAmount , value);
                (SaveCommand as DelegateCommandBase).RaiseCanExecuteChanged();
            }
        }

        private string _addedExpenseDescription = null;
            
        public string AddedExpenseDescription
        {
            get { return _addedExpenseDescription; }
            set 
            {
                var changed = value != _addedExpenseDescription;
                Set(ref _addedExpenseDescription , value);
                if (changed)
                {
                    (AddExpenseDescriptionCommand as DelegateCommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }

            
        public ObservableCollection<ExpenseDescription> ExpenseDescriptions { get; set; }
        public ObservableCollection<string> Employees { get; set; }

        public CashOutViewModel():base()
        {
            NumericKeyboardCommand = new DelegateCommandBase(NumericKeyboard);
            SaveCommand = new DelegateCommandBase(AddExpense, CanAddExpense);
            AddExpenseDescriptionCommand = new DelegateCommandBase(AddExpenseDescription,CanAddExpenseDescription);
            RemoveExpenseDescriptionCommand = new DelegateCommandBase(RemoveExpenseDescription);
            AddedExpenseDescription = string.Empty;
            var descriptions = StateManager.Get<ExpenseDescription>();
            ExpenseDescriptions = new ObservableCollection<ExpenseDescription>(descriptions);
            var employees = StateManager.GetService<CashRegisterExpense, ICashRegisterExpenseRepository>().GetEmployees();
            Employees = new ObservableCollection<string>(employees);

        }

        public void AddExpense(object obj)
        {
            if (!decimal.TryParse(ExpenditureAmount,out var amount))
            {
                ToastNotification.Notify("قم بادخال القيمة المناسبة");
                return;
            }

            var expense = new CashRegisterExpense() 
            { 
                Amount = amount, 
                Employee = SelectedEmployee,
                Description = SelectedExpenditureDescription.Description,
                IssuedDate = DateTime.Now
            };

            if (StateManager.Save(expense))
            {
                ToastNotification.Notify("Saved",NotificationType.Success);
            }
            this.Close();
        }

        public bool CanAddExpense(object obj)
        {
            return !(
                string.IsNullOrEmpty(SelectedEmployee) || 
                string.IsNullOrEmpty(ExpenditureAmount)|| 
                SelectedExpenditureDescription == null);
        }

        public void AddExpenseDescription(object obj)
        {
            var expenseDescription = new ExpenseDescription() { Description = _addedExpenseDescription };
            StateManager.Save(expenseDescription);
            ExpenseDescriptions.Add(expenseDescription);
            SelectedExpenditureDescription = expenseDescription;
            AddedExpenseDescription = string.Empty;
        }

        public void RemoveExpenseDescription(object obj)
        {
            if (StateManager.Delete(SelectedExpenditureDescription))
            {
                ExpenseDescriptions?.Remove(SelectedExpenditureDescription);

                ToastNotification.Notify("Item was removed Successfuly",NotificationType.Success);
            }
        }

        public bool CanAddExpenseDescription(object obj)
        {
            return !(string.IsNullOrEmpty(AddedExpenseDescription) || string.IsNullOrWhiteSpace(AddedExpenseDescription));
        }

        public void AddEmployee()
        {

        }

        public void NumericKeyboard(object obj)
        {
            if (obj is string number)
            {
                NumericKeypad.NumericKeyboard(number, ref _expenditureAmount);
                //ExpenditureAmount = _expenditureAmount;
                NotifyOfPropertyChange(nameof(ExpenditureAmount));
                (SaveCommand as DelegateCommandBase).RaiseCanExecuteChanged();
            }
        }

        public ICommand NumericKeyboardCommand { get; set; }
        public ICommand AddExpenseDescriptionCommand { get; set; }
        public ICommand RemoveExpenseDescriptionCommand { get; set; }





    }

    public class NumericKeypad
    {

        public static readonly string NUM_PAD_0 = "0";
        public static readonly string NUM_PAD_1 = "1";
        public static readonly string NUM_PAD_2 = "2";
        public static readonly string NUM_PAD_3 = "3";
        public static readonly string NUM_PAD_4 = "4";
        public static readonly string NUM_PAD_5 = "5";
        public static readonly string NUM_PAD_6 = "6";
        public static readonly string NUM_PAD_7 = "7";
        public static readonly string NUM_PAD_8 = "8";
        public static readonly string NUM_PAD_9 = "9";
        public static readonly string NUM_PAD_PERCENT = "%";
        public static readonly string NUM_PAD_DOT = ".";
        public static readonly string NUM_PAD_BACKSPACE = "BACKSPACE";
        public static readonly string[] NUM_PAD_KEYS = 
         {
            NUM_PAD_0,
            NUM_PAD_1,
            NUM_PAD_2,
            NUM_PAD_3,
            NUM_PAD_4,
            NUM_PAD_5,
            NUM_PAD_6,
            NUM_PAD_7, 
            NUM_PAD_8, 
            NUM_PAD_9,
            NUM_PAD_DOT,
            NUM_PAD_PERCENT,
            NUM_PAD_BACKSPACE
        };

        
        public static void NumericKeyboard(string key, ref string  numericZone)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (!NUM_PAD_KEYS.Contains(key))
                return;

            if (key.Equals(NUM_PAD_BACKSPACE)&& numericZone?.Length>=1)
            {
                numericZone = numericZone.Remove(numericZone.Length - 1, 1);
                return;
            }

            if (key.Length > 1)
                return;
            
            
            if (numericZone == null)
                numericZone = string.Empty;
           

            if (key.Equals(NUM_PAD_DOT))
            {
                numericZone = numericZone.Contains(".") ? numericZone : numericZone + ".";
                return;
            }

            if (key.Equals(NUM_PAD_PERCENT))
            {
                numericZone = numericZone.Contains("%") ? numericZone : numericZone + "%";
                return;
            }

            if (numericZone.Contains(NUM_PAD_PERCENT))
            {
                var percentStr = numericZone.Remove(numericZone.Length - 1, 1) + key;
                var percent = Convert.ToDecimal(percentStr);
                if (percent < 0 || percent > 100)
                {
                    ToastNotification.Notify("Invalid value for Percentagte", NotificationType.Warning);
                }
                else
                {
                    numericZone = numericZone.Remove(numericZone.Length - 1, 1) + key + "%";
                }

                return;
            }

            numericZone += key;
        }

   
    }
}
    