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
        public Dictionary<int, decimal> CashPayments { get; set; }

        [DataMember]
        public Dictionary<int, decimal> DeliveryPayments { get; set; }

        [DataMember]
        public Dictionary<string, decimal> Expenses { get; set; }
        
        [DataMember]
        public List<EarningsCategoryGrouping> EarningsByCategory { get; set; }

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

        public int NumberOfPayments => (CashPayments?.Count ?? 0) + (DeliveryPayments?.Count ?? 0);

    }

    public class EarningsCategoryGrouping
    {
        public long id { get; set; }
        public String Category { get; set; }
        public int QuantityOfItems { get; set; }
        public decimal Amount { get; set; }
  
    }
}