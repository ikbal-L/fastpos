using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Model
{
   public class PageList<T>
    {
        public List<T> page { get; set; }
        public long count { get; set; }
    }
}
