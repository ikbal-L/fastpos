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
