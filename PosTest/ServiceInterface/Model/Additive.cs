using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Windows.Media;

namespace ServiceInterface.Model
{
    public class Additive : PropertyChangedBase //, IEquatable<Additive>
    {
        private string _backgroundString = null;

        public Additive() { }

        public Additive(Additive additive)
        {
            Id = additive.Id;
            Description = additive.Description;
            //Ingrediants = additive.Ingrediants;
            //ParentOrderItem = additive.ParentOrderItem;
            BackgroundString = additive.BackgroundString;
        }
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Description { get; set; }

        public List<Ingredient> Ingrediants { get; set; }

        [DataMember]
        public List<long> IdIngrediants { get; set; }
        
        public OrderItem ParentOrderItem { get; set; }

        [DataMember]
        public string BackgroundString
        {
            get => _backgroundString ?? "#00f39c12";
          set
            {
                _backgroundString = value;
                NotifyOfPropertyChange(nameof(Background));
            }
        }

        [DataMember]
        public Brush Background
        {
            get => new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundString));
        }

        /*bool IEquatable<Additive>.Equals(Additive additive)
        {
            // var additive = additiveObj as Additive;
            var result = additive != null &&
                    additive.Id == Id &&
                    additive.Description == Description &&
                    additive.BackgroundString == BackgroundString;
            return result;
        }*/

        public override bool Equals(object other)
        {
            var additive = other as Additive;
            var result = additive != null &&
                    additive.Id == Id &&
                    additive.Description == Description &&
                    additive.BackgroundString == BackgroundString;
            return result;
            //           return Equals(other as Additive);
        }
    }
}