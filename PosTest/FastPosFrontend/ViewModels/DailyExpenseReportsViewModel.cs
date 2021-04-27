using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using NLog;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{
    [NavigationItemConfiguration(title: "Daily Expense Reports", target: typeof(DailyExpenseReportsViewModel))]
    public class DailyExpenseReportsViewModel : LazyScreen
    {
        
        private bool _isReportGenerated;
        private DailyExpenseReport _report;

        public DailyExpenseReportsViewModel()
        {
            Setup();
            OnReady();
            
        }

        protected override void Setup()
        {
            var reports = StateManager.GetAsync<DailyExpenseReport>();
            _data = new NotifyAllTasksCompletion(reports);
            
        }


        public override void Initialize()
        {
            var reports = StateManager.Get<DailyExpenseReport>();
            Reports = (reports != null && reports.Any())
                ? new BindableCollection<DailyExpenseReport>(reports)
                : new BindableCollection<DailyExpenseReport>();
            Report =  reports?.FirstOrDefault(r => r.IssuedDate == DateTime.Today.Date);
            if (Report!= null)
            {
                IsReportGenerated = true;
            }
           
        }

        

        public bool IsReportGenerated
        {
            get => _isReportGenerated;
            set => Set(ref _isReportGenerated, value);
        }

        public DailyExpenseReport Report
        {
            get => _report;
            set => Set(ref _report, value);
        }

        public BindableCollection<DailyExpenseReport> Reports { get; set; }


        public void Generate()
        {
            var parent = this.Parent as MainViewModel;
            var vm = new DailyExpenseReportInputDataViewModel(this);
            vm.OnReportGenerated(report =>
            {
                Report = report;
                IsReportGenerated = true;
            });

            parent?.OpenDialog(vm).OnClose(() =>
            {
                vm.Dispose();
            });
        }
    }

    [DataContract]
    public class DailyExpenseReportInputData : PropertyChangedBase
    {
        private decimal _cashRegisterInitialAmount;
        private decimal _cashRegisterActualAmount;
        private DateTime _workTimeStart;
        private DateTime _workTimeEnd;

        public DailyExpenseReportInputData()
        {
          
        }

        [DataMember]
        public decimal CashRegisterInitialAmount
        {
            get => _cashRegisterInitialAmount;
            set => Set(ref _cashRegisterInitialAmount, value);
        }

        [DataMember]
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
        [DataMember]
        public Dictionary<string,decimal> Expenses { get; set; }

        
    }

    public class Expense:PropertyChangedBase
    {
        public string Description { get; set; }

        public decimal Amount { get; set; }
    }
}