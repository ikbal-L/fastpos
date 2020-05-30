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
        [DataMember]
        public List<Ingredient> Ingredients { get; set; }

        public List<long> IdIngredients { get; set; }
        public List<Additive> Additives { get; set; }

        [DataMember]
        public List<long> IdAdditives { get; set; }

        [DataMember]
        //[DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool IsPlatter { get; set; } = true;
    }
}
