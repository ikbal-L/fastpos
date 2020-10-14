using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media;

namespace ServiceInterface.Model
{
    public class Category : Ranked
    {
        private string _backGroundString=null;
        private SolidColorBrush _backGround;
        private Color? _backgroundColor;
        private string _name;
        private string _description;

        [DataMember(IsRequired = true)]
        public long? Id { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Name must not be Null or Empty")]
        [MaxLength(10,ErrorMessage = "Name must not exceed 10 characters ")]
        [MinLength(5,ErrorMessage = "Name must not be under 5 characters ")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must only contain letters (a-z, A-Z).")]
        public string Name
        {
            get => _name;
            set { Set(ref _name, value); }
        }
        [Required(ErrorMessage = "Description must not be Null or Empty")]
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        [DataMember]
        public string BackgroundString 
        { 
            get => _backGroundString ?? "#ffffff";
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
                Set(ref _backGroundString, this._backGround.Color.ToString(), nameof(BackgroundString));
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

        public void MappingAfterReceiving(ref ICollection<Product> products)
        {
            if (ProductIds != null && products != null)
            {
                Products = new List<Product>();
                foreach (var id in ProductIds)
                {
                    var prod = products.Where(p => id == p.Id).FirstOrDefault();
                    if (prod != null)
                    {
                        //since prod belongs to this category  
                        prod.Category = this;
                        Products.Add(prod);
                    }
                    else
                    {
                        throw new MappingException("Product does not exist");
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

        

    }
}
