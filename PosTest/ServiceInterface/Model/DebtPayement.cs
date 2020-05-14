using System.Runtime.Serialization;

namespace ServiceInterface.Model
{
    [DataContract]
    public class DebtPayement
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public long CustomerId { get; set; }

        public Customer Customer { get; set; }

        [DataMember]
        public decimal  Amount { get; set; }
    }
}
