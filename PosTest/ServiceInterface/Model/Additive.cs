using Caliburn.Micro;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Windows.Media;

namespace ServiceInterface.Model
{
    public class Additive : PropertyChangedBase
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

        public override bool Equals(object additiveObj)
        {
            var additive = additiveObj as Additive;
            return additive != null &&
                   additive.Id == Id &&
                   additive.Description == Description &&
                   additive.BackgroundString == BackgroundString;
        }
    }
}