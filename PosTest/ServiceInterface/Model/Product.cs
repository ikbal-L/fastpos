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
    public class Product : Ranked,IState<long>
    {
        private BindableCollection<Additive> _additives;
        private string _backgroundString = null;
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
        [MinLength(5, ErrorMessage = "Product Name must not be under 5 characters ")]
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
        // [Required(ErrorMessage = "Price is required")]
        // [RegularExpression(@"\-?\d+\.\d+",ErrorMessage = "Product Price must be a Decimal")]
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
        public long? CategoryId
        {
            get => _categoryId;
            set
            {
                Set(ref _categoryId, value);
         
            }
        }


        [DataMember]
        public List<Ingredient> Ingredients { get; set; }

        //public List<long> IdIngredients { get; set; }
        public BindableCollection<Additive> Additives
        {
            get => _additives;
            set
            {
                _additives = value;
                NotifyOfPropertyChange(() => Additives);
            }
        }



        [DataMember]
        public List<long> IdAdditives { get; set; } = new List<long>();

        public Category Category
        {
            get => _category;
            set
            {
                _category = value;
                CategoryId = _category?.Id;
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
        private long? _categoryId;

        public Product()
        {
            
        }

        public Product(Product platProduct) : this()
        {
            AvailableStock = platProduct.AvailableStock;
            BackgroundString = platProduct.BackgroundString;
            CategoryId = platProduct.CategoryId;
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
            IdAdditives = platProduct.IdAdditives;
            Additives = platProduct.Additives;
            Ingredients = platProduct.Ingredients;
        }

        [DataMember]
        [Required]
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
            if (this.CategoryId <= 0 && this.Category != null)
            {
                this.CategoryId = (long) this.Category.Id;
            }




            if (this.IsPlatter)
            {
                if (this.Additives != null)
                {
                    if (this.IdAdditives == null)
                    {
                        this.IdAdditives = new List<long>();
                    }
                    else
                    {
                        this.IdAdditives.Clear();
                    }

                    foreach (var a in this.Additives)
                    {
                        this.IdAdditives.Add((long)a.Id);
                    }
                }

                if (this.Ingredients != null)
                {
                    foreach (var ing in this.Ingredients)
                    {
                        if (ing.Product != null)
                        {
                            ing.ProductId = (long)ing.Product.Id;
                        }
                    }
                } 
            }
                


            if (this.CategoryId == null)
            {
                this.Rank = null;
            }

            if (this.Rank == null)
            {
                this.CategoryId = null;
            }
        }

        public void MappingAfterReceiving(Category category, List<Additive> additives)
        {
            if (category != null && this.CategoryId == category.Id)
            {
                this.Category = category;
                this.CategoryId = category.Id;
            }

            //else
            //{
            //    throw new MappingException("Id category different of CategoryId of the related Product");
            //}
            if (this.IsPlatter &&
                this.IdAdditives != null && additives != null )
            {
                this.Additives = new BindableCollection<Additive>();
                foreach (var a in additives)
                {
                    if (this.IdAdditives.Any(id => id == a.Id))
                    {
                        this.Additives.Add(a);
                    }
                    //else
                    //{
                    //    throw new MappingException("Additive Id does not exist in the list of ids");
                    //}
                }
            }
        }

        bool IsEquqlsTo(Product product)
        {
            throw new NotImplementedException();
        }
    }
}