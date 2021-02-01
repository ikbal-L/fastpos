using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
        public long? Id { get; set; }
        [DataMember]
        public long CashOperationId;
        [DataMember]
        public long DeliveryManId;

    }
}
