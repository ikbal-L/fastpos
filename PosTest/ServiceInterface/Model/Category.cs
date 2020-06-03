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
        private Brush _backGroundColor;
        private string _name;

        [DataMember]
        public long Id { get; set; }

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
            get => _backGroundString; 
            set 
            { 
                _backGroundString = value;
                NotifyOfPropertyChange(() => BackgroundString); 
                NotifyOfPropertyChange(() => BackGroundColor); 
            } 
        }

        [DataMember]
        public List<long> ProductIds { get; set; }
        public List<Product> Products { get; set; }


        public Brush BackGroundColor => _backGroundColor ?? 
            (_backGroundColor=new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundString)));

        public void MappingBeforeSending()
        {
            if (Products != null)
                foreach (var p in Products)
                {
                    if (ProductIds == null)
                    {
                        ProductIds = new List<long>();
                    }
                    ProductIds.Add(p.Id);
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

    }


}
