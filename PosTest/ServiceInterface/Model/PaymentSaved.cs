using System.Collections.Generic;
using System.Runtime.Serialization;

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
        [DataMember]

        public long? Id { get; set; }
    }
}
