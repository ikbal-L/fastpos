using Caliburn.Micro;
using PosTest.Extensions;
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
using ServiceInterface.ExtentionsMethod;
using ServiceLib.Service.StateManager;

namespace PosTest.ViewModels.DeliveryAccounting
{
   public class PaymentHistoryViewModel : PropertyChangedBase
    {
    

        private ObservableCollection<Payment> _Payments;


        private int _PageNumber = 0;
        private int _PageSize = 20;
        private Deliveryman _SelectedDeliveryman { get; set; }
        public ObservableCollection<Payment> Payments
        {
            get { return _Payments; }
            set
            {
                _Payments = value;
                NotifyOfPropertyChange(nameof(Payments));
            }
        }
      
        private long Count=0;
        public bool PrevioussBtnEnabled { get =>  _PageNumber !=0; }
        public bool NextBtnEnabled { get => Count > (_PageNumber + 1) * _PageSize; }
        private Payment _SelectedPayment;

        public Payment SelectedPayment
        {
            get { return _SelectedPayment; }
            set { _SelectedPayment = value;
                NotifyOfPropertyChange(nameof(SelectedPayment));
            }
        }

        private DeliveryAccountingViewModel Parent { get; set; }
        public PaymentHistoryViewModel(DeliveryAccountingViewModel deliveryAccountingViewModel) {
            this.Parent = deliveryAccountingViewModel;
        }

        public void UpdateDatas()
        {

            if (this._SelectedDeliveryman != null)
            {
                var res = StateManager.GetService<Payment, IPaymentRepository>().getAllByDeliveryManPage( _PageNumber, _PageSize,_SelectedDeliveryman.Id.Value);
                
                    Payments = res != null ? new ObservableCollection<Payment>(res.page) : new ObservableCollection<Payment>();
                    Count = res != null ? res.count : 0;
                    NotifyOfAllPropertyChange();
                
            }
            else
            {
                Payments = new ObservableCollection<Payment>();
                Count = 0;
            }

        }
        public void Pagination(PaginationAction action)
        {
            _PageNumber += (action == PaginationAction.Next) ? 1 : -1;
            UpdateDatas();
        }
      public void  ChangeSelectedDeliveryman(Deliveryman selectedDeliveryman)
        {
            _SelectedDeliveryman = selectedDeliveryman;
      }
        public void NotifyOfAllPropertyChange() {
            NotifyOfPropertyChange(nameof(PrevioussBtnEnabled));
            NotifyOfPropertyChange(nameof(NextBtnEnabled));
        }
        public void EditPayment()
        {
            if (SelectedPayment != null)
            {
                var payedAmount = !string.IsNullOrEmpty(Parent.NumericZone) ? Convert.ToDecimal(Parent.NumericZone) : -1;
                if (payedAmount < 0)
                {
                    Parent.NumericZone = "";
                    return;
                }
                var localpayment = SelectedPayment.Clone();
                localpayment.Amount = payedAmount;
                var res = StateManager.Save<Payment>(localpayment);
                if (res)
                {
                    UpdateDatas();
                    Parent.NumericZone = "";
                }
            }
        }
        public void DeletePayment()
        {
          
            var res = StateManager.Delete<Payment>(SelectedPayment);
            if (res)
            {
                UpdateDatas();
                Parent.RelaodDeliveryMan();

            }
        }
    }
}
