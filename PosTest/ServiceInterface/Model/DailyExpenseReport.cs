using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Caliburn.Micro;

namespace ServiceInterface.Model
{
    [DataContract]
    public class DailyExpenseReport:PropertyChangedBase
    {
        [DataMember]
        public DateTime IssuedDate { get; set; }

        public Dictionary<string, decimal> CashPayments;

        public Dictionary<string, decimal> DeliveryPayments;
        
        public Dictionary<string, decimal> Expenses;

        [DataMember]
        public decimal CashRegisterInitialAmount { get; set; }

        [DataMember]
        public decimal CashRegisterDepositedAmount { get; set; }
        [DataMember]
        public decimal CashRegisterWithDrawnAmount { get; set; }

        [DataMember]
        public decimal CashRegisterExpectedAmount { get; set; }
        [DataMember]
        public decimal CashRegisterActualAmount { get; set; }   
    }
}