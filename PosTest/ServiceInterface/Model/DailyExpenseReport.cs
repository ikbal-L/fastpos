using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Caliburn.Micro;

namespace ServiceInterface.Model
{
    [DataContract]
    public class DailyExpenseReport:PropertyChangedBase,IState<long>
    {
        public long? Id { get; set; }

        [DataMember]
        public DateTime IssuedDate { get; set; }
        [DataMember]
        public Dictionary<string, decimal> CashPayments;

        [DataMember]
        public Dictionary<string, decimal> DeliveryPayments;

        [DataMember]
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