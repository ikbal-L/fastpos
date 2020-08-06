using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Model
{
    [DataContract]
    public class Table : PropertyChangedBase
    {
        private int _number;

        [DataMember]
        public long?  Id { get; set; }

        [DataMember]
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                NotifyOfPropertyChange(()=> Number);
            }
        }

        [DataMember]
        public int Seats { get; set; }

        public Place Place { get; set; }

        [DataMember]
        public Place PlaceId { get; set; }

        [DataMember]
        public bool IsVirtual { get; set; } = false;

        public BindableCollection<Order> TableOrders { get; set; }

        public void AddOrder(Order currentOrder)
        {
            if (TableOrders == null)
            {
                TableOrders = new BindableCollection<Order>();
            }
            TableOrders.Add(currentOrder);
        }

        public int RemoveOrder(Order currentOrder)
        {
            TableOrders.Remove(currentOrder);
            return TableOrders.Count;
        }
    }
}
