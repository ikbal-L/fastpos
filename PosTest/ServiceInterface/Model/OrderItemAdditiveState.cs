using System;
using System.Runtime.Serialization;
using Caliburn.Micro;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServiceInterface.Model
{
    public class OrderItemAdditive : PropertyChangedBase
    {
        private string _modifier;
        private AdditiveState _state;

        [DataMember]
        [JsonRequired]
        public long AdditiveId { get; set; }

        [DataMember]
        public long? OrderItemId { get; set; }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public AdditiveState State { get => _state; set => Set(ref _state , value); }
        [DataMember]
        public string Modifier
        {
            get => _modifier;
            set
            {
                Set(ref _modifier, value);
                NotifyOfPropertyChange(nameof(DisplayName));
            }
        }

        [DataMember]
        public DateTime? Timestamp { get; set; }

        public Additive Additive { get; set; }

        public OrderItem OrderItem { get; set; }

        public string DisplayName => $"{Additive.Description}{Modifier}";
    }
}