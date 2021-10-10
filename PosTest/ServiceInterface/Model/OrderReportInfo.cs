using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace ServiceInterface.Model
{
   
    [DataContract]
    public class OrderReportInfo
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public long OrderNumber { get; set; }
        [DataMember]
        public decimal Total { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
    }
}