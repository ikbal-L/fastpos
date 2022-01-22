using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ServiceInterface.Model
{
    [DataContract]
    public class Page<T>
    {
        [DataMember]
        public int Size { get; set; }
        [DataMember]
        public List<T> Elements { get; set; }
        [DataMember]
        public int? TotalPages { get; set; }
    }
}