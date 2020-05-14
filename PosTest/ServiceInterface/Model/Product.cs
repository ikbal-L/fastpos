using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media;

namespace ServiceInterface.Model
{
    public class Product : PropertyChangedBase
    {
        private string _backgroundString = null;
        private long _categoryId;
        private bool _isMuchInDemand;


        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public string Unit { get; set; }
        [DataMember]
        public bool IsMuchInDemand 
        { 
            get => _isMuchInDemand;
            set
            {
                _isMuchInDemand = value;
            }
        }
        [DataMember]
        public long CategorieId
        {
            get => Category == null ? _categoryId : Category.Id;
            set { _categoryId = value; }
        }

        
        public Category Category { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Color { get; set; }
        [DataMember]
        public string PictureFileName { get; set; }
        [DataMember]
        public string PictureUri { get; set; }
        [DataMember]
        public int AvailableStock { get; set; }

        [DataMember]
        public string BackgroundString 
        { 
            get => _backgroundString ?? "#f39c12";
            set
            {
                _backgroundString = value;
                NotifyOfPropertyChange(nameof(Background));
            }
            
        }

        public virtual Brush Background { 
            get => new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundString));
        }

        
    }
}
