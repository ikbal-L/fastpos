using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media;

namespace ServiceInterface.Model
{
    public class Category : PropertyChangedBase
    {
        private string _backGroundString=null;
        private Brush _backGround;
        private string _name;
        private Color? _backgroundColor;

        [DataMember(IsRequired = true)]
        public long? Id { get; set; }

        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public string Description { get; set; }

        [DataMember]
        public string BackgroundString 
        { 
            get => _backGroundString ?? "#f39c12";
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
                _backGround = (SolidColorBrush)value;
                NotifyOfPropertyChange(() => Background);
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

        public void MappingAfterReceiving(ICollection<Product> products)
        {
            if (ProductIds != null && products != null)
            {
                Products = new List<Product>();
                foreach (var id in ProductIds)
                {
                    var prod = products.Where(p => id == p.Id).FirstOrDefault();
                    if (prod != null)
                    {
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
                _backgroundColor = value;
                BackgroundString = _backgroundColor.ToString();
                Background = new SolidColorBrush((Color)_backgroundColor);
                NotifyOfPropertyChange(() => IsDark);
            }

        }

        public bool IsDark
        {
            get
            {
                var c = BackgroundColor.GetValueOrDefault();
                var d = (5 * c.G + 2 * c.R + c.B) <= 8 * 128;
                Console.Write(Name + " "); Console.WriteLine(d);
                return (5 * c.G + 2 * c.R + c.B) <= 8 * 140;
            }
        }
    }


}
