using Attributes;
using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
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
        //[RegularExpression(@"^[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z0-9-_\s]*$", ErrorMessage = "Use Latin or Arabic Characters only ")]
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
        [Min("1")]
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
        private bool _isPlatter = false;

        public Product()
        {
            
        }


        [DataMember]
        [Required]
        public string BackgroundString
        {
            get => _backgroundString ?? (_backgroundString = "#FFF5F5F5");
            set => Set(ref _backgroundString, value);
        }

        public virtual SolidColorBrush Background
        {
            get => _background ?? (_background =
                new SolidColorBrush((Color) ColorConverter.ConvertFromString(BackgroundString)));
            set
            {
                Set(ref _background, ((SolidColorBrush) value));
                Set(ref _backgroundString, _background.Color.ToString(), nameof(BackgroundString));
            }
        }

        

        [DataMember]
        //[DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsPlatter
        {
            get => _isPlatter;
            set
            {
                Set(ref _isPlatter, value);
                if (!value)
                {
                    Additives = null;
                    IdAdditives = null;
                }
                else
                {
                    Additives = new BindableCollection<Additive>();
                    IdAdditives = new List<long>();
                }
            }
        }

        public bool IsDark
        {
            get
            {
                var c = Background.Color;
                var d = (5 * c.G + 2 * c.R + c.B) <= 8 * 128;
                return (5 * c.G + 2 * c.R + c.B) <= 8 * 140;
            }
        }


        public void MappingBeforeSending()
        {
            if (CategoryId <= 0 && Category != null)
            {
                CategoryId = (long) Category.Id;
            }




            if (IsPlatter)
            {
                if (Additives != null)
                {
                    if (IdAdditives == null)
                    {
                        IdAdditives = new List<long>();
                    }
                    else
                    {
                        IdAdditives.Clear();
                    }

                    foreach (var a in Additives)
                    {
                        IdAdditives.Add((long)a.Id);
                    }
                }

                if (Ingredients != null)
                {
                    foreach (var ing in Ingredients)
                    {
                        if (ing.Product != null)
                        {
                            ing.ProductId = (long)ing.Product.Id;
                        }
                    }
                } 
            }
                


            if (CategoryId == null)
            {
                Rank = null;
            }

            if (Rank == null)
            {
                CategoryId = null;
            }
        }

        public void MappingAfterReceiving(Category category, List<Additive> additives)
        {
            if (category != null && CategoryId == category.Id)
            {
                Category = category;
                CategoryId = category.Id;
            }

            //else
            //{
            //    throw new MappingException("Id category different of CategoryId of the related Product");
            //}
            if (IsPlatter &&
                IdAdditives != null && additives != null )
            {
                Additives = new BindableCollection<Additive>();
                foreach (var a in additives)
                {
                    if (IdAdditives.Any(id => id == a.Id))
                    {
                        Additives.Add(a);
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

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}