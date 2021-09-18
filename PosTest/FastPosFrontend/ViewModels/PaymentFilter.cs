using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FastPosFrontend.ViewModels
{
    public class PaymentFilter
    {
        [DataMember]
        public long? DeliverymanId { get; set; }
        public IEnumerable<long>? DeliverymanIds { get; set; }
    }
}
