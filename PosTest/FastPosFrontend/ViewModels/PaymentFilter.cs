using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FastPosFrontend.ViewModels
{
    public class PaymentFilter :Filter
    {
        [DataMember]
        public DateTime? Date { get; set; }
        [DataMember]
        public long? DeliverymanId { get; set; }
        [DataMember]
        public IEnumerable<long>? DeliverymanIds { get; set; }
    }
}
