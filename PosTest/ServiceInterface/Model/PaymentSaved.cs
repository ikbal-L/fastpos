using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Model
{
    [DataContract]
  public  class PaymentSaved
    {
        [DataMember]
        public Payment Payment { get; set; }
        [DataMember]
        public List<Order> PaidOrders { get; set; }
        [DataMember]
        public Deliveryman Deliveryman { get; set; }
        [DataMember]
        public List<Order> NotPaidOrders { get; set; }
    }
}
