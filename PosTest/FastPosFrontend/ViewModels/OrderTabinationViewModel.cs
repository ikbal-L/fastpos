using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FastPosFrontend.ViewModels
{
    public class OrderTabinationViewModel
    {

        public OrderTabinationViewModel()
        {

        }

        public CollectionViewSource PendingOrdersCollection { get; set; }
        public CollectionViewSource TakeAwayOrdersCollection { get; set; }
        public CollectionViewSource DeliveryOrdersCollection { get; set; }
        public CollectionViewSource TablesCollection { get; set; }

    }
}
