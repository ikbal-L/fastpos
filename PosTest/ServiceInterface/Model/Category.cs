﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace ServiceInterface.Model
{
    public class Category : Ranked, IState<long>
    {
        private string _backGroundString=null;
        private SolidColorBrush _backGround;
        private Color? _backgroundColor;
        private string _name;
        private string _description;

        [DataMember(IsRequired = true)]
        public long? Id { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Name must not be Null or Empty",AllowEmptyStrings = false)]
        [MinLength(2,ErrorMessage = "Name must not be under 2 characters ")]
        //[RegularExpression(@"^[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z-0-9_\s]*$", ErrorMessage = "Use Latin, Arabic or Numeric Characters only ")]

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        //[Required(ErrorMessage = "Name must not be Null or Empty")]
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        [DataMember]
        [Required(AllowEmptyStrings = false)]
        public string BackgroundString 
        { 
            get => _backGroundString ?? " #DEDEDE";
            set 
            { 
                _backGroundString = value;
                NotifyOfPropertyChange(() => BackgroundString); 
                NotifyOfPropertyChange(() => Background); 
            } 
        }

        [DataMember]
        public List<long> ProductIds { get; set; }
        public List<Product> Products { get; set; }

        public Brush Background
        {
            get => _backGround ?? (_backGround = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundString)));
            set
            {
                Set(ref _backGround, (SolidColorBrush) value);
                Set(ref _backGroundString, _backGround.Color.ToString(), nameof(BackgroundString));
                Set(ref _backgroundColor, ((SolidColorBrush)value).Color, nameof(BackgroundColor));

            }
        }

        public void MappingBeforeSending()
        {
            if (Products != null)
            {
                if (ProductIds == null)
                {
                    ProductIds = new List<long>();
                }
                else
                {
                    ProductIds.Clear();
                }
                foreach (var p in Products)
                {
                    ProductIds.Add((long)p.Id);
                }
            }
        }

        public void MappingAfterReceiving( IEnumerable<Product> products)
        {
            if (ProductIds != null && products != null)
            {
                Products = new List<Product>();
                foreach (var id in ProductIds)
                {
                    var prod = products.FirstOrDefault(p => id == p.Id);
                    if (prod != null)
                    {
                        //since prod belongs to this category  
                        prod.Category = this;
                        Products.Add(prod);
                    }
                }
            }
        }

        public Color? BackgroundColor
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

        public bool IsDark
        {
            get
            {
                var c = BackgroundColor.GetValueOrDefault();
                var d = (5 * c.G + 2 * c.R + c.B) <= 8 * 128;
                return (5 * c.G + 2 * c.R + c.B) <= 8 * 140;
            }
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
