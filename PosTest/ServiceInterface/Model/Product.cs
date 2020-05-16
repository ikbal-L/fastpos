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
        private string _description;
        private string _name;
        private decimal _price;
        private string _unit;
        private Category _category;

        [DataMember]
        public int Id { get; set; }
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
        [DataMember]
        public string Description { 
            get => _description;
            set
            {
                _description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }
        [DataMember]
        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                NotifyOfPropertyChange(() => Price);
            }
        }
        [DataMember]
        public string Unit
        {
            get => _unit;
            set
            {
                _unit = value;
                NotifyOfPropertyChange(() => Unit);
            }
        }
        [DataMember]
        public bool IsMuchInDemand 
        {
            get => _isMuchInDemand;
            set
            {
                _isMuchInDemand = value;
                NotifyOfPropertyChange(() => IsMuchInDemand);
            }
        }
        [DataMember]
        public int CategorieId
        {
            get => Category == null ? _categoryId : Category.Id;
            set { _categoryId = value; }
        }

        public Category Category
        {
            get => _category;
            set
            {
                _category = value;
                NotifyOfPropertyChange(() => Category);
            }
        }

       
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

        [DataMember]
        public virtual Brush Background { 
            get => new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundString));
        }

        
    }
}
