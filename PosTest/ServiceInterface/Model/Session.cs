using System;
using System.Runtime.Serialization;

namespace ServiceInterface.Model
{
    [DataContract]
    public class Session
    {
        [DataMember] 
        public long Id { get; set; }

        [DataMember]
        public DateTime OpenTime { get; set; }

        [DataMember]
        public DateTime CloseTime { get; set; }

        public User User  { get; set; }

        [DataMember]
        public long UserId { get; set; }

        public Terminal Terminal { get; set; }

        [DataMember]
        public long TerminalId { get; set; }

        [DataMember]
        public string IpAddress { get; set; }
 
        [DataMember]
        public string MacAddress { get; set; }
    }
}
