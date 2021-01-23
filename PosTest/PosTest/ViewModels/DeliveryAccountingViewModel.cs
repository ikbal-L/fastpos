using Caliburn.Micro;
using PosTest.Helpers;
using ServiceInterface.Model;
using ServiceLib.Service;
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
        private string _NumericZone ;

        public string NumericZone
        {
            get { return _NumericZone; }
            set {
                _NumericZone = value;
                NotifyOfPropertyChange(nameof(NumericZone));
            }
        }
        private Order _CurrentOrder;


        public Order CurrentOrder
        {
            get { return _CurrentOrder; }
            set { _CurrentOrder = value;
                NotifyOfPropertyChange(nameof(CurrentOrder));
            }
        }

        public decimal Total
        {
            get { return Orders?.Sum(x => x.Total) ?? 0;  }
            
        }


        public ObservableCollection<Order> Orders
        {
            get { return new ObservableCollection<Order>(StateManager.Get<Order>()); }
            
        }
        private Deliveryman _selectedDeliveryman;

        public Deliveryman SelectedDeliveryman
        {
            get { return _selectedDeliveryman; }
            set { _selectedDeliveryman = value;
                NotifyOfPropertyChange(nameof(SelectedDeliveryman));
                NotifyOfPropertyChange(nameof(Total));
            }
        }

        public  DeliveryAccountingViewModel() {
            StateManager.Fetch();
            StateManager.Associate<Additive, Product>();
            StateManager.Associate<Product, Category>();
            StateManager.Associate<Order, Table>();
            StateManager.Associate<Order, Product>();
            StateManager.Associate<Order, Deliveryman>();
            StateManager.Associate<Order, Waiter>();
            StateManager.Associate<Order, Customer>();
            Deliverymans =  new ObservableCollection<Deliveryman>(StateManager.Get<Deliveryman>());
            SelectedDeliveryman = Deliverymans.FirstOrDefault();
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
                    break;

             
            }
        }

        private void PayementAction()
        {
        }
       public void ViewOrderItems(Order order) {
            order.ProductsVisibility = !order.ProductsVisibility;
          //  NotifyOfPropertyChange(() => order.ProductsVisibility);
        }
       public  enum ActionButton
        {
            Payment,
            Backspase,
            BackOut
        }
    }

}
