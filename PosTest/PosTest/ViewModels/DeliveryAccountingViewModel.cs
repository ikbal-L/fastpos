using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.ViewModels
{
   public class DeliveryAccountingViewModel: Screen
    {
        private ObservableCollection<Deliveryman> _Deliverymans;

        public ObservableCollection<Deliveryman> Deliverymans
        {
            get { return _Deliverymans; }
            set { _Deliverymans = value;
                NotifyOfPropertyChange(nameof(Deliverymans));
            }
        }

      public  DeliveryAccountingViewModel() {
            Deliverymans = new ObservableCollection<Deliveryman>();

            for (int i = 0; i < 20; i++)
            {
                Deliverymans.Add(new Deliveryman() { Name = "Delivery man " + i });
            }
        }
    }
}
