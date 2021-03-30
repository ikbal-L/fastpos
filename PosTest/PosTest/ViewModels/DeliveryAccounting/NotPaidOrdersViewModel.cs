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
   public class NotPaidOrdersViewModel: PropertyChangedBase
    {
        public decimal Total
        {
            get { return Orders?.Sum(x => x.Total) ?? 0; }

        }
        private ObservableCollection<Order> _Orders;
        private Deliveryman _SelectedDeliveryman { get; set; }

        private ObservableCollection<Payment> _Payments;

        public ObservableCollection<Payment> Payments
        {
            get { return _Payments; }
            set { _Payments = value;

                NotifyOfPropertyChange(nameof(Payments));
            }
        }
        private bool _PaymentVisibility;

        public bool PaymentVisibility
        {
            get { return _PaymentVisibility; }
            set { _PaymentVisibility = value;
                NotifyOfPropertyChange(nameof(PaymentVisibility));
                NotifyOfPropertyChange(nameof(Width));
                NotifyOfPropertyChange(nameof(BorderVisibility));

            }
        }
        private bool _PaymentVisivilityBtn;
        public bool BorderVisibility { get => PaymentVisibility && PaymentVisivilityBtn; }
        public bool PaymentVisivilityBtn
        {
            get { return _PaymentVisivilityBtn; }
            set { _PaymentVisivilityBtn = value;
                NotifyOfPropertyChange(nameof(PaymentVisivilityBtn));
                NotifyOfPropertyChange(nameof(BorderVisibility));

            }
        }

        public double Width { get => !BorderVisibility ? 680 : 340; }
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
            Orders = new ObservableCollection<Order>();
        }
        public void UpdateDatas()
        {

            if (this._SelectedDeliveryman != null)
            {
                   var res = StateManager.GetService<Order, IOrderRepository>().GetOrderByStates(new string[] { OrderState.Delivered.ToString() }, this._SelectedDeliveryman.Id.Value);
                    Orders = res != null ? new ObservableCollection<Order>(res) : new ObservableCollection<Order>();
                 GetDeliveryManPayment();
                 NotifyOfAllPropertyChange();

            }
            else
            {
                Orders = new ObservableCollection<Order>();
         
            }

        }
        public void  ChangeSelectedDeliveryman(Deliveryman selectedDeliveryman)
        {
            _SelectedDeliveryman = selectedDeliveryman;
        }
        public void NotifyOfAllPropertyChange() {
            NotifyOfPropertyChange(nameof(Total));
            Parent.NotifyTotal();

        }
        public void GetDeliveryManPayment() {
                 var res = StateManager.GetService<Payment, IPaymentRepository>().getByDeliveryManAndDate(this._SelectedDeliveryman.Id.Value,DateTime.Now);
                     Payments = res != null ? new ObservableCollection<Payment>(res) : new ObservableCollection<Payment>();
                     PaymentVisibility = Payments.Count > 0;
                     PaymentVisivilityBtn = Payments.Count > 0 && Orders.Count > 0;

        }
        public void  TogglePaymentsBtn() {
            PaymentVisibility = !PaymentVisibility;
       }
       
    }
}
