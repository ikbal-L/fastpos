using ServiceInterface.Model;
using System;
using System.Runtime.Serialization;

namespace ServiceInterface.Model
{
    public class CashRegisterExpense:IState<long>
    {

        public long? Id { get; set; }
        [DataMember]
        public decimal Amount { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Employee { get; set; }
        [DataMember]
        public DateTime IssuedDate { get; set; }
    }
}
    