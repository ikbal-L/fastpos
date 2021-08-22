using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Caliburn.Micro;

namespace FastPosFrontend.ViewModels
{
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
}