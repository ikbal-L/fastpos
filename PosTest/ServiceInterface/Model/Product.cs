using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media;

namespace ServiceInterface.Model
{
    public class Product : Ranked
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

        [DataMember] public long? Id { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Product Name must not be Null or Empty")]
        [MaxLength(10, ErrorMessage = "Product Name must not exceed 10 characters ")]
        [MinLength(5, ErrorMessage = "Product Name must not be under 5 characters ")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Product Name must only contain letters (a-z, A-Z).")]
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
        [Required(ErrorMessage = "Product Description must not be Null or Empty")]
        [MaxLength(10, ErrorMessage = "Product Description must not exceed 10 characters ")]
        [MinLength(5, ErrorMessage = "Product Description must not be under 5 characters ")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Product Description must only contain letters (a-z, A-Z).")]
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }

        [DataMember]
        [Required(ErrorMessage = "Price is required")]
        [RegularExpression(@"\-?\d+\.\d+",ErrorMessage = "Product Price must be a Decimal")]
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
        [RegularExpression(@"[0-9]*$",ErrorMessage = "Product unit number must be a Number")]
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

        [DataMember] public long? CategorieId { get; set; }

        public Category Category
        {
            get => _category;
            set
            {
                _category = value;
                CategorieId = _category?.Id;
                NotifyOfPropertyChange(() => Category);
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange();
            }
        }


        [DataMember] public string Type { get; set; }
        [DataMember] public string Color { get; set; }
        [DataMember] public string PictureFileName { get; set; }
        [DataMember] public string PictureUri { get; set; }
        [DataMember] public int AvailableStock { get; set; }

        private Color? _backgroundColor;
        private bool _isSelected;

        public Product()
        {
        }

        public Product(Product platProduct) : this()
        {
            AvailableStock = platProduct.AvailableStock;
            BackgroundString = platProduct.BackgroundString;
            CategorieId = platProduct.CategorieId;
            Description = platProduct.Description;
            Id = platProduct.Id;
            IsMuchInDemand = platProduct.IsMuchInDemand;
            Name = platProduct.Name;
            PictureFileName = platProduct.PictureFileName;
            PictureUri = platProduct.PictureUri;
            Price = platProduct.Price;
            Type = platProduct.Type;
            Unit = platProduct.Unit;
            Rank = platProduct.Rank;
        }

        [DataMember]
        public string BackgroundString
        {
            get => _backgroundString ?? (_backgroundString = "#ff12eaf3");
            set => Set(ref _backgroundString, value);
        }

        public virtual Brush Background
        {
            get => _background ?? (_background =
                new SolidColorBrush((Color) ColorConverter.ConvertFromString(BackgroundString)));
            set
            {
                Set(ref _background, ((SolidColorBrush) value));
                Set(ref _backgroundString, this._background.Color.ToString(), nameof(BackgroundString));
            }
        }

        public virtual Color? BackgroundColor
        {
            get
            {
                if (_backgroundColor == null)
                {
                    _backgroundColor = (Color) ColorConverter.ConvertFromString(BackgroundString);
                }

                return _backgroundColor;
            }
            set
            {
                _backgroundColor = value;
                BackgroundString = _backgroundColor.ToString();
                NotifyOfPropertyChange(() => BackgroundString);
                Background = new SolidColorBrush((Color) _backgroundColor);
                NotifyOfPropertyChange(() => IsDark);
            }
        }

        [DataMember]
        //[DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsPlatter { get; set; } = false;

        public bool IsDark
        {
            get
            {
                var c = BackgroundColor.GetValueOrDefault();
                var d = (5 * c.G + 2 * c.R + c.B) <= 8 * 128;
                return (5 * c.G + 2 * c.R + c.B) <= 8 * 140;
            }
        }


        public void MappingBeforeSending()
        {
            if (this.CategorieId <= 0 && this.Category != null)
            {
                this.CategorieId = (long) this.Category.Id;
            }


            if (this is Platter plat)
            {
                if (plat.Additives != null)
                {
                    if (plat.IdAdditives == null)
                    {
                        plat.IdAdditives = new List<long>();
                    }
                    else
                    {
                        plat.IdAdditives.Clear();
                    }

                    foreach (var a in plat.Additives)
                    {
                        plat.IdAdditives.Add(a.Id);
                    }
                }

                if (plat.Ingredients != null)
                    foreach (var ing in plat.Ingredients)
                    {
                        if (ing.Product != null)
                        {
                            ing.ProductId = (long) ing.Product.Id;
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

            //else
            //{
            //    throw new MappingException("Id category different of CategoryId of the related Product");
            //}
            if (this is Platter plat &&
                plat.IdAdditives != null && additives != null &&
                additives.Count == plat.IdAdditives.Count)
            {
                plat.Additives = new BindableCollection<Additive>();
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

        bool IsEquqlsTo(Product product)
        {
            throw new NotImplementedException();
        }
    }
}