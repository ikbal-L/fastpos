using Caliburn.Micro;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ServiceInterface.Model
{
    [DataContract]
   public class Payment: PropertyChangedBase, IState<long>
    {

   
        private decimal _amount;
        [DataMember]
        public decimal Amount
        {
            get { return _amount; }
            set { _amount = value; 
                NotifyOfPropertyChange(nameof(Amount));

            }
        }

        private decimal? _discountAmount;
        [DataMember]
        public decimal? DiscountAmount
        {
            get { return _discountAmount; }
            set
            {
                _discountAmount = value;
                NotifyOfPropertyChange(nameof(DiscountAmount));

            }
        }

        [DataMember]
       public DateTime Date { get; set; }
        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        public long CashOperationId;

        [DataMember]
        public long? DeliveryManId { get; set; }

        [DataMember]
        public long? CustomerId { get; set; }

        [DataMember]
        public List<Order> Orders { get; set; }
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentSource PaymentSource { get; set; }

    }

    public enum PaymentSource
    {
        Regular, Delivery, Customer
    }
}   
