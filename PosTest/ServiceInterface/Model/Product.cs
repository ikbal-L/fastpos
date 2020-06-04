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
        private string _description;
        private string _name;
        private decimal _price;
        private string _unit;
        private Category _category;
        private SolidColorBrush _background;

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
        public long CategorieId
        {
            get;
            set;
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

        public virtual Brush Background { 
            get => _background ?? (_background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundString)));
        }

        public void MappingBeforeSending()
        {
            if (this.CategorieId <= 0 && this.Category != null)
            {
                this.CategorieId = (long)this.Category.Id;
            }

            if (this is Platter plat)
            {
                if (plat.Additives != null)
                    foreach (var a in plat.Additives)
                    {
                        if (plat.IdAdditives == null)
                        {
                            plat.IdAdditives = new List<long>();
                        }
                        plat.IdAdditives.Add(a.Id);
                    }

                if (plat.Ingredients != null)
                    foreach (var ing in plat.Ingredients)
                    {
                        if (ing.Product != null)
                        {
                            ing.ProductId = (long)ing.Product.Id;
                        }
                    }
            }
        }

        public void MappingAfterReceiving(Category category, List<Additive> additives)
        {
            if (category != null && this.CategorieId == category.Id)
            {
                this.Category = category;
            }
            else
            {
                throw new MappingException("Id category different of CategoryId of the related Product");
            }
            if (this is Platter plat &&
                plat.IdAdditives != null && additives != null &&
                additives.Count == plat.IdAdditives.Count)
            {
                plat.Additives = new List<Additive>();
                foreach (var a in additives)
                {
                    if (plat.IdAdditives.Any(id => id == a.Id))
                    {
                        plat.Additives.Add(a);
                    }
                    else
                    {
                        throw new MappingException("Additive Id does not exist in the list of ids");
                    }
                }
            }
        }

    }
}
