using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Model
{
    
    public class Platter : Product
    {
        public List<Ingredient> Ingredients { get; set; }
        public List<Additive> Additives { get; set; }
    }
}
