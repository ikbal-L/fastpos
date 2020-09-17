using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Newtonsoft.Json;

namespace ServiceInterface.Model
{
    
    public class Platter : Product
    {
        private BindableCollection<Additive> _additives;

        public Platter()
        {
            IsPlatter = true;
        }

        public Platter(Platter platProduct) : base(platProduct)
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
            Rank = platProduct.Rank; IsPlatter = true;

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
        public List<long> IdAdditives { get; set; }

        
    }
}
