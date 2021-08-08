using System.Collections.Generic;

namespace ServiceInterface.Model
{
    public class PageList<T>
    {
        public List<T> page { get; set; }
        public long count { get; set; }
    }
}
