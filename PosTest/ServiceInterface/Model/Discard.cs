using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Model
{
    [DataContract]
    public class Discard
    {
        [DataMember]
        public long  Id { get; set; }

        public Product Product { get; set; }

        [DataMember]
        public long ProductId { get; set; }

        [DataMember]
        public DateTime DateTime { get; set; }

        [DataMember]
        public decimal Quantity { get; set; }
    }
}
