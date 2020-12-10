using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServiceInterface.Model
{
    public class OrderItemAdditive
    {
        [DataMember]
        [JsonRequired]
        public long AdditiveId { get; set; }

        [DataMember]
        public long OrderItemId { get; set; }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public AdditiveState State { get; set; }

        [DataMember]
        public DateTime? Timestamp { get; set; }
    }
}