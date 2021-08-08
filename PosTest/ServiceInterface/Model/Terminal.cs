using System.Runtime.Serialization;

namespace ServiceInterface.Model
{
    [DataContract]
    public class Terminal
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string LicenceKey { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        public Annex Annex { get; set; }

        [DataMember]
        public long AnnexId { get; set; }
    }
}
