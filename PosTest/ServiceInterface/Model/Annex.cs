using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
