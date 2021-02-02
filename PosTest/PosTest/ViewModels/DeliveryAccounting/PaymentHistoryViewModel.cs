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

        public void UpdateOrderNotPaid()
        {

            if (this._SelectedDeliveryman != null)
            {
                var res = StateManager.getService<Payment, IPaymentRepository>().getAllByDeliveryManPage( _PageNumber, _PageSize,_SelectedDeliveryman.Id.Value);
                if (res != null)
                {
                    Payments = res.page != null ? new ObservableCollection<Payment>(res.page) : new ObservableCollection<Payment>();
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
        public void EditPayment()
        {
            var payedAmount =!string.IsNullOrEmpty(Parent.NumericZone)? Convert.ToDecimal(Parent.NumericZone) : -1;
            if (payedAmount < 0)
            {
                Parent.NumericZone = "";
                return;
            }
            var localpayment = SelectedPayment.Clone();
            localpayment.Amount = payedAmount;
            var res = StateManager.SaveAndReturn<Payment, PaymentSaved>(localpayment);
            if (res.Item1)
            {
                Parent.NumericZone = "";
                Parent.SelectedDeliveryman.Balance = res.Item2.Deliveryman.Balance;
                res.Item2.PaidOrders?.ForEach(x => Parent.NotPaidOrdersViewModel.Orders?.Remove(Parent.NotPaidOrdersViewModel.Orders?.FirstOrDefault(o => x.Id == o.Id)));
                res.Item2.NotPaidOrders?.ForEach(x => Parent.NotPaidOrdersViewModel.Orders?.Add(x));
                Parent.NotPaidOrdersViewModel.NotifyOfAllPropertyChange();

                res.Item2.NotPaidOrders?.ForEach(x => Parent.AllOrdersViewModel.Orders?.Remove(Parent.NotPaidOrdersViewModel.Orders?.FirstOrDefault(o => x.Id == o.Id)));
                res.Item2.PaidOrders?.ForEach(x => Parent.AllOrdersViewModel.Orders?.Add(x));
                var index = Parent.PaymentHistoryViewModel.Payments.IndexOf(SelectedPayment);
                Parent.PaymentHistoryViewModel.Payments.RemoveAt(index);
                Parent.PaymentHistoryViewModel.Payments.Insert(index, res.Item2.Payment);
            }
        }
        public void DeletePayment()
        {
          
            var res = StateManager.DeleteAndRetrun<Payment,PaymentSaved>(SelectedPayment);
            if (res.Item1)
            {
                Parent.SelectedDeliveryman.Balance = res.Item2.Deliveryman.Balance;
                res.Item2.PaidOrders?.ForEach(x => Parent.NotPaidOrdersViewModel.Orders?.Remove(Parent.NotPaidOrdersViewModel.Orders?.FirstOrDefault(o => x.Id == o.Id)));
                res.Item2.NotPaidOrders?.ForEach(x => Parent.NotPaidOrdersViewModel.Orders?.Add(x));
                Parent.NotPaidOrdersViewModel.NotifyOfAllPropertyChange();
                res.Item2.NotPaidOrders?.ForEach(x => Parent.AllOrdersViewModel.Orders?.Remove(Parent.NotPaidOrdersViewModel.Orders?.FirstOrDefault(o => x.Id == o.Id)));
                res.Item2.PaidOrders?.ForEach(x => Parent.AllOrdersViewModel.Orders?.Add(x));
                var index = Parent.PaymentHistoryViewModel.Payments.IndexOf(SelectedPayment);
                Parent.PaymentHistoryViewModel.Payments.RemoveAt(index);
            }
        }
    }
}
