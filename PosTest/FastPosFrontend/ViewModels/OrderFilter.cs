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
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public List<OrderState> States { get; set; } = new();

        [DataMember]
        public List<long>? DeliverymanIds { get; set; } = new();

        [DataMember]
        public List<long>? CustomerIds { get; set; } = new();
    }

    public class Filter
    {
        [DataMember]
        public int? PageSize { get; set; }
        [DataMember]
        public int? PageIndex { get; set; }
        [DataMember]
        public string? OrderBy { get; set; }
        [DataMember]
        public SortOrder? SortOrder { get; set; }
        [DataMember]
        public Dictionary<string, List<object>> In { get; set; }



    }

    

    public enum SortOrder
    {
        Asc,Desc,None
    }
}
