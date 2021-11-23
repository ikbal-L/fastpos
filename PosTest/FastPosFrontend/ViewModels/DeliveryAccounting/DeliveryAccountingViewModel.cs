using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;
using FastPosFrontend.Navigation;
namespace FastPosFrontend.ViewModels.DeliveryAccounting
{


    //[NavigationItem("Delivery Accounting", typeof(DeliveryAccountingViewModel),"")]
    public class DeliveryAccountingViewModel: AppScreen
    {
        private ObservableCollection<Deliveryman> _Deliverymans;

        public ObservableCollection<Deliveryman> Deliverymans
        {
            get { return _Deliverymans; }
            set { _Deliverymans = value;
                NotifyOfPropertyChange(nameof(Deliverymans));
            }
        }
        public ObservableCollection<Deliveryman> FilterDeliverymens { get => new ObservableCollection<Deliveryman>(Deliverymans.Where(x => x.Name.ToLower().Contains(SearchDeliveryMan.ToLower()))); }
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
        private string _SearchDeliveryMan="";

        public string SearchDeliveryMan
        {
            get { return _SearchDeliveryMan; }
            set { _SearchDeliveryMan = value;
                NotifyOfPropertyChange(nameof(SearchDeliveryMan));
                NotifyOfPropertyChange(nameof(FilterDeliverymens));
            }
        }

        //TODO null exception 
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
                LoadDataTab();
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
                LoadDataTab();
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
                    ToastNotification.Notify("Invalid value for Percentagte", NotificationType.Warning);
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

         
                case ActionButton.Enter:

                    if (ActiveTab == EnumActiveTab.NotPaidOrders)
                    {
                        PayementAction();
                        RelaodDeliveryMan();
                    }
                    else
                    {
                        EditPayment();
                        RelaodDeliveryMan();
                    }
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
    
               var res=   StateManager.Save(new Payment() { Amount = payedAmount,Date=DateTime.Now, DeliveryManId = SelectedDeliveryman.Id.Value });
               if (res)
                {
                    NumericZone = "";
                    LoadDataTab();
                }
       
            }
      

        }

        public void EditPayment()
        {
            PaymentHistoryViewModel.EditPayment();
        }

        private void LoadDataTab()
        {
            switch (ActiveTab)
            {
                case EnumActiveTab.NotPaidOrders:
                    NotPaidOrdersViewModel.UpdateDatas();
                    break;
                case EnumActiveTab.AllOrders:
                    AllOrdersViewModel.UpdateDatas();
                    break;
                case EnumActiveTab.PaymentHistory:
                    PaymentHistoryViewModel.UpdateDatas();
                    break;

            }
        }
        public  enum ActionButton
        {
            Enter,
            Backspase,
            BackOut
        }

        public void RelaodDeliveryMan() {
           var res= StateManager.GetService<Deliveryman, IDeliverymanRepository>().GetById(SelectedDeliveryman.Id.Value);
            if(res.status==(int)HttpStatusCode.OK)
            {
               var index= Deliverymans.IndexOf(Deliverymans?.FirstOrDefault(x => x.Id == SelectedDeliveryman.Id));
                Deliverymans.RemoveAt(index);
                Deliverymans.Insert(index,res.Item2);
                NotifyOfPropertyChange(nameof(FilterDeliverymens));
                SelectedDeliveryman = res.Item2;
            }
        
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
        PaymentHistory=2
        
    }
}
