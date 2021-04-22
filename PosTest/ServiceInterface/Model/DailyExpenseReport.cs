using System;
using System.Collections.Generic;
using Caliburn.Micro;

namespace ServiceInterface.Model
{
    public class DailyExpenseReport:PropertyChangedBase
    {
        public DateTime IssuedDate { get; set; }

        public Dictionary<string, decimal> CashPayments;
        public Dictionary<string, decimal> DeliveryPayments;
        public Dictionary<string, decimal> Expenses;    

        public decimal CashRegisterInitialAmount { get; set; }

        public decimal CashRegisterDepositedAmount { get; set; }
        public decimal CashRegisterWithDrawnAmount { get; set; }

        public decimal CashRegisterExpectedAmount { get; set; }
        public decimal CashRegisterActualAmount { get; set; }   
    }
}