﻿using Caliburn.Micro;
using PosTest.Helpers;
using PosTest.ViewModels.DeliveryAccounting;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using ServiceLib.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.ViewModels.DeliveryAccounting
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
        private string _NumericZone ;
        public string NumericZone
        {
            get { return _NumericZone; }
            set {
                _NumericZone = value;
                NotifyOfPropertyChange(nameof(NumericZone));
            }
        }

        internal void NotifyTotal()
        {
            NotifyOfPropertyChange(nameof(Total));
        }


        public decimal Total
        {
            get { return NotPaidOrdersViewModel.Total; }
          
        }

        private EnumActiveTab _ActiveTab= EnumActiveTab.NotPaidOrders;

        public EnumActiveTab ActiveTab
        {
            get { return _ActiveTab; }
            set { _ActiveTab = value;
                NotifyOfPropertyChange(nameof(ActiveTab));
            }
        }

        private Deliveryman _selectedDeliveryman;

        public Deliveryman SelectedDeliveryman
        {
            get { return _selectedDeliveryman; }
            set { _selectedDeliveryman = value;
                NotifyOfPropertyChange(nameof(SelectedDeliveryman));
                NotPaidOrdersViewModel?.ChangeSelectedDeliveryman(SelectedDeliveryman);
                AllOrdersViewModel?.ChangeSelectedDeliveryman(SelectedDeliveryman);
                PaymentHistoryViewModel?.ChangeSelectedDeliveryman(SelectedDeliveryman);
            }
        }
        private NotPaidOrdersViewModel _NotPaidOrdersViewModel;

        public NotPaidOrdersViewModel NotPaidOrdersViewModel
        {
            get { return _NotPaidOrdersViewModel; }
            set { _NotPaidOrdersViewModel = value;
                NotifyOfPropertyChange(nameof(NotPaidOrdersViewModel));
            }
        }
        private AllOrdersViewModel _AllOrdersViewModel;

        public AllOrdersViewModel AllOrdersViewModel
        {
            get { return _AllOrdersViewModel; }
            set { _AllOrdersViewModel = value; 
                NotifyOfPropertyChange(nameof(AllOrdersViewModel));
            }
        }
        private PaymentHistoryViewModel _PaymentHistoryViewModel;

        public PaymentHistoryViewModel PaymentHistoryViewModel
        {
            get { return _PaymentHistoryViewModel; }
            set { _PaymentHistoryViewModel = value;
                NotifyOfPropertyChange(nameof(PaymentHistoryViewModel));
            }
        }

        public  DeliveryAccountingViewModel() {
            StateManager.Fetch();
            Deliverymans =  new ObservableCollection<Deliveryman>(StateManager.Get<Deliveryman>());
            initialize();
        }
        public  void initialize(){
          Task.Run(()=> {
              NotPaidOrdersViewModel = new NotPaidOrdersViewModel(this);
              AllOrdersViewModel = new AllOrdersViewModel(this);
              PaymentHistoryViewModel = new PaymentHistoryViewModel(this);
          });
        }
        public void NumericKeyboard(string number)
        {
            if (String.IsNullOrEmpty(number))
                return;
            if (number.Length > 1)
                return;
            var numbers = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ".", "%" };
            if (!numbers.Contains(number))
                return;
            if (NumericZone == null)
                NumericZone = String.Empty;

            if (number.Equals("."))
            {
                NumericZone = NumericZone.Contains(".") ? NumericZone : NumericZone + ".";
                return;
            }

            if (number.Equals("%"))
            {
                NumericZone = NumericZone.Contains("%") ? NumericZone : NumericZone + "%";
                return;
            }

            if (NumericZone.Contains("%"))
            {
                var percentStr = NumericZone.Remove(NumericZone.Length - 1, 1) + number;
                var percent = Convert.ToDecimal(percentStr);
             if (percent < 0 || percent > 100)
                {
          /*          if (IsRunningFromXUnit)
                    {
                        throw new Exception("Invalid value for Percentagte");
                    }
                    else
                    {*/
                        ToastNotification.Notify("Invalid value for Percentagte", NotificationType.Warning);
                  //  }
                }
                else
                {
                    NumericZone = NumericZone.Remove(NumericZone.Length - 1, 1) + number + "%";
                }

                return;
            }

            NumericZone += number;
        }
        public void ActionKeyboard(ActionButton cmd)
        {
         
            switch (cmd)
            {
                case ActionButton.Backspase:
                    NumericZone = String.IsNullOrEmpty(NumericZone)
                        ? String.Empty
                        : NumericZone.Remove(NumericZone.Length - 1);
                    break;

         
                case ActionButton.Payment:
               

                    PayementAction();
                    break;
                case ActionButton.BackOut:
                    LoginViewModel loginvm = new LoginViewModel();
                    loginvm.Parent = this.Parent;
                    (this.Parent as Conductor<object>).ActivateItem(loginvm);
                    break;

             
            }
        }

        private void PayementAction()
        {
            
            var payedAmount = Convert.ToDecimal(NumericZone);
            if (payedAmount < 0)
            {
                NumericZone = "";
                return;
            }

           if (ActiveTab == EnumActiveTab.NotPaidOrders)
            {
    
               var res=   StateManager.SaveAndReturn<Payment,PaymentSaved>(new Payment() { Amount = payedAmount,Date=DateTime.Now, DeliveryManId = SelectedDeliveryman.Id.Value });
               if (res.Item1)
                {
                    NumericZone = "";
                    SelectedDeliveryman.Balance = res.Item2.Deliveryman.Balance;
                    res.Item2.PaidOrders?.ForEach(x => NotPaidOrdersViewModel.Orders.Remove(NotPaidOrdersViewModel.Orders.FirstOrDefault(o => x.Id == o.Id)));
                    res.Item2.NotPaidOrders?.ForEach(x => NotPaidOrdersViewModel.Orders.Add(x));
                    NotPaidOrdersViewModel.NotifyOfAllPropertyChange();
                    res.Item2.NotPaidOrders?.ForEach(x => AllOrdersViewModel.Orders.Remove(NotPaidOrdersViewModel.Orders.FirstOrDefault(o => x.Id == o.Id)));
                    res.Item2.PaidOrders?.ForEach(x => AllOrdersViewModel.Orders.Add(x));
                    PaymentHistoryViewModel.Payments.Insert(0, res.Item2.Payment);
                }
       
            }
      

        }

       public void DeletePayment() {
            PaymentHistoryViewModel.DeletePayment();
        }
        public void EditPayment()
        {
            PaymentHistoryViewModel.EditPayment();
        }
        public  enum ActionButton
        {
            Payment,
            Backspase,
            BackOut
        }

    }
   public enum PaginationAction
    {
        Next,
        Previous
    }
    public enum EnumActiveTab
    {
        NotPaidOrders=0,
        AllOrders=1,
        
    }
}
