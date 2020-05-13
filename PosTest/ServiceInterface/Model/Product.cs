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
        private int _categoryId;
        private bool _isMuchInDemand;

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public bool IsMuchInDemand 
        { 
            get => _isMuchInDemand;
            set
            {
                _isMuchInDemand = value;
            }
        }

        public int CategorieId
        {
            get => Category == null ? _categoryId : Category.Id;
            set { _categoryId = value; }
        }

        
        public Category Category { get; set; }

        public string Type { get; set; }
        public string Color { get; set; }
        
        public string PictureFileName { get; set; }
        public string PictureUri { get; set; }

        public int AvailableStock { get; set; }


        public string BackgroundString 
        { 
            get => _backgroundString ?? "#f39c12";
            set
            {
                _backgroundString = value;
                NotifyOfPropertyChange(nameof(Background));
            }
            
        }

        
        public Brush Background { 
            get => new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundString));
        }

        
    }
}
