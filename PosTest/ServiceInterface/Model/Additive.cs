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
        private string _description;
        private string _backgroundString;
        private SolidColorBrush _background;
        private Color? _backgroundColor;
        
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
        public long? Id { get; set; }

        [DataMember]
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        public List<Ingredient> Ingrediants { get; set; }

        [DataMember]
        public List<long> IdIngrediants { get; set; }
        
        public OrderItem ParentOrderItem { get; set; }

        [DataMember]
        public string BackgroundString
        {
            get => _backgroundString ?? "#00f39c12";
            set => Set(ref _backgroundString, value);
        }

        public Brush Background
        {
            get => new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundString));
            set
            {
                Set(ref _background, (SolidColorBrush)value);
                Set(ref _backgroundString, this._background.Color.ToString(), nameof(BackgroundString));
                Set(ref _backgroundColor, ((SolidColorBrush)value).Color, nameof(BackgroundColor));
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
                Set(ref _backgroundColor, value);
                NotifyOfPropertyChange(() => IsDark);
            }

        }
    }
}