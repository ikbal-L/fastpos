using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FastPosFrontend.EventManagement
{
    public class UnlockMessage
    {
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public IList<long> Ids { get; set; }
    }
}
