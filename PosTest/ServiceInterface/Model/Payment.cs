using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ServiceInterface.Model
{
    [DataContract]
   public class Payment: PropertyChangedBase, IState<long>
    {

   
        private decimal _amount;
        [DataMember]
        public decimal Amount
        {
            get { return _amount; }
            set { _amount = value; 
                NotifyOfPropertyChange(nameof(Amount));

            }
        }
        [DataMember]
       public DateTime Date { get; set; }
        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        public long CashOperationId;

        [DataMember]
        public long DeliveryManId { get; set; }

        [DataMember]
        public List<long> OrderIds { get; set; }

    }
}   
