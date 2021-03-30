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
using ServiceLib.Service.StateManager;

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
        public FilterOrderState FilterOrderState  { get; set; }
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
            FilterOrderState = new FilterOrderState();
            FilterOrderState.PropertyChanged += FilterOrderState_PropertyChanged;
        }

        private void FilterOrderState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateDatas();
        }

        public void UpdateDatas()
        {

            if (this._SelectedDeliveryman != null)
            {
                var res = StateManager.GetService<Order, IOrderRepository>().getAllByDeliveryManAndStatePage( _PageNumber, _PageSize,_SelectedDeliveryman.Id.Value, FilterOrderState.GetStates());
  
                    Orders = res != null ? new ObservableCollection<Order>(res.page) : new ObservableCollection<Order>();
                    Count = res != null? res.count:0;
                    NotifyOfAllPropertyChange();
            }
            else
            {
                Orders = new ObservableCollection<Order>();
                Count = 0;
                NotifyOfAllPropertyChange();
            }

        }
        public void Pagination(PaginationAction action)
        {
            _PageNumber += (action == PaginationAction.Next) ? 1 : -1;
            UpdateDatas();
        }
        public void ChangeSelectedDeliveryman(Deliveryman selectedDeliveryman)
        {
            _SelectedDeliveryman = selectedDeliveryman;
        }
        public void NotifyOfAllPropertyChange() {
            NotifyOfPropertyChange(nameof(PrevioussBtnEnabled));
            NotifyOfPropertyChange(nameof(NextBtnEnabled));
        }

    }
    public class FilterOrderState: PropertyChangedBase
    {
        private bool _isDelivered;

        public bool IsDelivered
        {
            get { return _isDelivered; }
            set { _isDelivered = value;
                NotifyOfPropertyChange(nameof(IsDelivered));
            }
        }
        private bool _isDeliveredReturned;

        public bool IsDeliveredReturned
        {
            get { return _isDeliveredReturned; }
            set
            {
                _isDeliveredReturned = value;
                NotifyOfPropertyChange(nameof(IsDeliveredReturned));
            }
        }
        private bool _isDeliveredPaid;

        public bool IsDeliveredPaid
        {
            get { return _isDeliveredPaid; }
            set
            {
                _isDeliveredPaid = value;
                NotifyOfPropertyChange(nameof(IsDeliveredPaid));
            }
        }
        public string[] GetStates() {
            var list = new List<string>();
            if (IsDelivered)
                list.Add(OrderState.Delivered.ToString());
            if (IsDeliveredPaid)
                list.Add(OrderState.DeliveredPaid.ToString());
            if (IsDeliveredReturned)
                list.Add(OrderState.DeliveredReturned.ToString());
            return list.ToArray();
        }
    }
}
