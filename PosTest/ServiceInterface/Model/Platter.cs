using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Model
{
    
    public class Platter : Product
    {
        public List<Ingredient> Ingredients { get; set; }
        [DataMember]
        public List<long> IdIngredients { get; set; }
        public List<Additive> Additives { get; set; }
        [DataMember]
        public List<long> IdAdditives { get; set; }
    }
}
