using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FastPosFrontend.ViewModels
{
    public class OrderFilter : Filter
    {
        [DataMember]
        public DateTime? OrderTime { get; set; }
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderState? State { get; set; }
        [DataMember]
        [JsonProperty( ItemConverterType = typeof(StringEnumConverter))]
        public IEnumerable<OrderState> States { get; set; }
        [DataMember]
        public long? DeliverymanId { get; set; }
        [DataMember]
        public IEnumerable<long>? DeliverymanIds { get; set; }
    }

    public abstract class Filter
    {
        [DataMember]
        public int? PageSize { get; set; }
        [DataMember]
        public int? PageIndex { get; set; }
        [DataMember]
        public string? OrderBy { get; set; }
        [DataMember]
        public bool? AscendingOrder { get; set; }
        [DataMember]
        public bool? DescendingOrder { get; set; }

    }
}
