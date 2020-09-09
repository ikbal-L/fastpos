using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.Events
{
    public class AssignOrderTypeEventArgs
    {
        public OrderType OrderType { get; set; }
    }
}
