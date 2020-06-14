﻿using Caliburn.Micro;
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
        private string _number;

        [DataMember]
        public long?  Id { get; set; }

        [DataMember]
        public string Number
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
    }
}
