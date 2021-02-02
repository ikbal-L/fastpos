using Caliburn.Micro;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PosTest.ViewModels.DeliveryAccounting
{
   public class NotPaidOrdersViewModel: PropertyChangedBase
    {
        public decimal Total
        {
            get { return Orders?.Sum(x => x.Total) ?? 0; }

        }

        private ObservableCollection<Order> _Orders;


        private Deliveryman _SelectedDeliveryman { get; set; }
        public ObservableCollection<Order> Orders
        {
            get { return _Orders; }
            set
            {
                _Orders = value;
                NotifyOfPropertyChange(nameof(Orders));
                NotifyOfAllPropertyChange();
            }
        }
        public void ViewOrderItems(Order order)
        {
            order.ProductsVisibility = !order.ProductsVisibility;
            //  NotifyOfPropertyChange(() => order.ProductsVisibility);
        }
   
        private DeliveryAccountingViewModel Parent { get;  set; }

        public NotPaidOrdersViewModel(DeliveryAccountingViewModel deliveryAccountingViewModel) {
            this.Parent = deliveryAccountingViewModel;
        }

        public void UpdateOrderNotPaid()
        {

            if (this._SelectedDeliveryman != null)
            {
                 var res = StateManager.getService<Order, IOrderRepository>().GetOrderByStates(new string[] { OrderState.Delivered.ToString() }, this._SelectedDeliveryman.Id.Value);
                if (res != null)
                {
                    Orders = res != null ? new ObservableCollection<Order>(res) : new ObservableCollection<Order>();
                    
                    NotifyOfAllPropertyChange();
                }
                else
                {
                    Orders =  new ObservableCollection<Order>();
                }
            }

        }
   
      public void  ChangeSelectedDeliveryman(Deliveryman selectedDeliveryman)
        {
            _SelectedDeliveryman = selectedDeliveryman;
            UpdateOrderNotPaid();
        }
        public void NotifyOfAllPropertyChange() {
            NotifyOfPropertyChange(nameof(Total));
            Parent.NotifyTotal();

        }
    }
}
