using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Windows.Media;

namespace ServiceInterface.Model
{
    public class Additive : Ranked //, IEquatable<Additive>
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
            set
            {
                _background = (SolidColorBrush)value;
            }
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

        private SolidColorBrush _background;
        private Color? _backgroundColor;
        public bool IsDark
        {
            get
            {
                var c = BackgroundColor.GetValueOrDefault();
                var d = (5 * c.G + 2 * c.R + c.B) <= 8 * 128;
                return (5 * c.G + 2 * c.R + c.B) <= 8 * 140;
            }
        }

        public virtual Color? BackgroundColor
        {

            get
            {
                if (_backgroundColor == null)
                {
                    _backgroundColor = (Color)ColorConverter.ConvertFromString(BackgroundString);
                }
                return _backgroundColor;
            }
            set
            {
                _backgroundColor = value;
                BackgroundString = _backgroundColor.ToString();
                Background = new SolidColorBrush((Color)_backgroundColor);
                NotifyOfPropertyChange(() => IsDark);
            }

        }
    }
}