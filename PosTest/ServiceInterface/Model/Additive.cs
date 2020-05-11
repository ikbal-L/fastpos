using System.Collections.Generic;
using System.Windows.Media;

namespace ServiceInterface.Model
{
    public class Additive
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

        public int Id { get; set; }
        public string Description { get; set; }
        public List<Ingredient> Ingrediants { get; set; }
        public OrderItem ParentOrderItem { get; set; }
        public string BackgroundString
        {
            get => _backgroundString ?? "#00f39c12";
            set => _backgroundString = value;
        }
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