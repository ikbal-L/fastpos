using System.Runtime.Serialization;

namespace ServiceInterface.Model
{
    [DataContract]
    public class Annex
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string  Name { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string ServerLicenceKey { get; set; }
    }
}
