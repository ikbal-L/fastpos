using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Model
{
    [DataContract]
    public class Restaurent
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Address { get; set; }

        public User Owner { get; set; }

        [DataMember]
        public User OwnerId { get; set; }

        public List<Annex> Annexes { get; set; }

        [DataMember]
        public List<long> AnnexeIds { get; set; }

    }
}
