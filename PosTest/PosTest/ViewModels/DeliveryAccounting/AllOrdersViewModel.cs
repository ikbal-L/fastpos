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
   public class AllOrdersViewModel: PropertyChangedBase
    {
        public decimal Total
        {
            get { return Orders?.Sum(x => x.Total) ?? 0; }

        }

        private ObservableCollection<Order> _Orders;


        private int _PageNumber = 0;
        private int _PageSize = 10;
        private Deliveryman _SelectedDeliveryman { get; set; }
        public ObservableCollection<Order> Orders
        {
            get { return _Orders; }
            set
            {
                _Orders = value;
                NotifyOfPropertyChange(nameof(Orders));
            }
        }
        public void ViewOrderItems(Order order)
        {
            order.ProductsVisibility = !order.ProductsVisibility;
            //  NotifyOfPropertyChange(() => order.ProductsVisibility);
        }
        private long Count=0;
        public bool PrevioussBtnEnabled { get =>  _PageNumber !=0; }
        public bool NextBtnEnabled { get => Count > (_PageNumber + 1) * _PageSize; }
        private DeliveryAccountingViewModel Parent { get; set; }
        public AllOrdersViewModel(DeliveryAccountingViewModel deliveryAccountingViewModel) {
            this.Parent = deliveryAccountingViewModel;
        }

        public void UpdateOrderNotPaid()
        {

            if (this._SelectedDeliveryman != null)
            {
                var res = StateManager.getService<Order, IOrderRepository>().getAllByDeliveryManPage( _PageNumber, _PageSize,_SelectedDeliveryman.Id.Value);
                if (res != null)
                {
                    Orders = res.page != null ? new ObservableCollection<Order>(res.page) : new ObservableCollection<Order>();
                    Count = res.count;
                    NotifyOfAllPropertyChange();
                }
            }

        }
        public void PaginationOrderNotPaid(PaginationAction action)
        {
            _PageNumber += (action == PaginationAction.Next) ? 1 : -1;
            UpdateOrderNotPaid();
        }
      public void  ChangeSelectedDeliveryman(Deliveryman selectedDeliveryman)
        {
            _SelectedDeliveryman = selectedDeliveryman;
            UpdateOrderNotPaid();
        }
        public void NotifyOfAllPropertyChange() {
            NotifyOfPropertyChange(nameof(PrevioussBtnEnabled));
            NotifyOfPropertyChange(nameof(NextBtnEnabled));
        }
    }
}
