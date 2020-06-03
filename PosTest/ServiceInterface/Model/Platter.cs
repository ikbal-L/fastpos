using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ServiceInterface.Model
{
    
    public class Platter : Product
    {
        public Platter()
        {
            IsPlatter = true;
        }
        [DataMember]
        public List<Ingredient> Ingredients { get; set; }

        //public List<long> IdIngredients { get; set; }
        public List<Additive> Additives { get; set; }
        public int aaa { get => 100;  }
        [DataMember]
        public List<long> IdAdditives { get; set; }

        
    }
}
