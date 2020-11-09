using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Model
{
    public class OrderItem : PropertyChangedBase
    {
        private static readonly bool IsRunningFromXUnit =
                   AppDomain.CurrentDomain.GetAssemblies().Any(
                       a => a.FullName.StartsWith("XUnitTesting"));
        private float _quantity;
        private BindableCollection<Additive> _additives;
        private Additive __selectedAdditive;
        private bool _canAddAdditives;
        private decimal _discountAmount = 0;
        private decimal _discountPercentage = 0;
        private bool _isSelected;

        public OrderItem() 
        {
            //Additives = new BindableCollection<Additive>();
        }

        public OrderItem(Product product, float quantity, decimal unitPrice, Order order) : this()
        {
            Order = order;
            Product = product;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        [DataMember]
        public long Id { set; get; }

        public Order Order { get; set; }
        
        [DataMember]
        public long? OrderId { get; set; }
        
        [DataMember]
        public long ProductId { get; set; }
        
        [DataMember]
        public decimal UnitPrice { get; set; }

        [DataMember]
        public float Quantity 
        { 
            get => _quantity;
            set
            {
                var oldQuqntity = _quantity;
                _quantity = value;
                NotifyOfPropertyChange(() => Quantity);
                NotifyOfPropertyChange(() => Total);
                NotifyOfPropertyChange(nameof(TotalDiscountAmount));
                Order.NotifyOfPropertyChange(nameof(Order.Total));
                Order.NotifyOfPropertyChange(nameof(Order.NewTotal));
                Order.NotifyOfPropertyChange(nameof(TotalDiscountAmount));
                //Order.Total = Order.Total + UnitPrice * (decimal)(_quantity - oldQuqntity);
            } 
        }

        [DataMember]
        public decimal Total {
            get => (decimal)Quantity * UnitPrice;          
        }

        [DataMember]
        public decimal TotalDiscountAmount
        {
            get 
            {
                //either _discountAmount==0 or _discountPercentage==0
                
                //NotifyOfPropertyChange(() => DiscountAmount);
                //Order.NotifyOfPropertyChange(nameof(Order.DiscountAmount));
                Order.NotifyOfPropertyChange(nameof(Order.TotalDiscountAmount));
                Order.NotifyOfPropertyChange(nameof(Order.NewTotal));
                return CalcTotalDiscount();
            }
        }

        public decimal CalcTotalDiscount()
        {
            var totalDiscount = _discountAmount * (decimal)Quantity + Total * _discountPercentage / 100;
            return totalDiscount;
        }

        [DataMember]
        public decimal DiscountAmount
        {
            get => _discountAmount;
            set
            {
                _discountAmount = value;
                _discountPercentage = 0;
                NotifyOfPropertyChange(() => DiscountAmount);
                NotifyOfPropertyChange(() => TotalDiscountAmount);
                //Order.NotifyOfPropertyChange(nameof(Order.DiscountAmount));
                Order.NotifyOfPropertyChange(nameof(Order.TotalDiscountAmount));
                Order.NotifyOfPropertyChange(nameof(Order.NewTotal));
            }
        }

        [DataMember]
        public decimal DiscountPercentatge
        {
            get => _discountPercentage;
            set
            {
                _discountPercentage = value;
                _discountAmount = 0;
                NotifyOfPropertyChange(() => DiscountPercentatge);
                NotifyOfPropertyChange(() => TotalDiscountAmount);
                //Order.NotifyOfPropertyChange(nameof(Order.DiscountAmount));
                Order.NotifyOfPropertyChange(nameof(Order.TotalDiscountAmount));
                Order.NotifyOfPropertyChange(nameof(Order.NewTotal));
            }
        }


        [DataMember]
        public DateTime? TimeStamp
        {
            get => _timeStamp;
            set => Set(ref _timeStamp, value);
        }
        [DataMember]
        public OrderItemState State
        {
            get => _state;
            set => Set(ref _state, value);
        }

        public Additive SelectedAdditive 
        { 
            get => __selectedAdditive;
            set
            {
                __selectedAdditive = value;
                Order.SelectedOrderItem = this;
                foreach (var oitem in Order.OrderItems)
                {
                    if(! oitem.Equals(this))
                        oitem.SetSelectedAdditive(null);
                }
                   
                NotifyOfPropertyChange(()=>SelectedAdditive);
            } 
        }

        private void SetSelectedAdditive(Additive additive)
        {
            __selectedAdditive = null;
            NotifyOfPropertyChange(()=>SelectedAdditive);
        }

        [DataMember]
        public List<long> AdditiveIds;
        private decimal _totalDiscountAmount;
        private DateTime? _timeStamp;
        private OrderItemState _state;

        public BindableCollection<Additive> Additives 
        { 
            get => _additives;
            set
            {
                _additives = value;
                NotifyOfPropertyChange(nameof(Additives));
            }
        }

        public bool CanAddAdditives
        { 
            get => Product is Platter && (Product as Platter).Additives != null;
            set
            {
                _canAddAdditives = value;
                   
            } 
        }

        
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(nameof(IsSelected));

            }

        }



        public Product Product { get; set; }

        //returns false if exists (did not add this additive)
        public bool AddAdditives(Additive additive)
        {
            if (this.Additives == null)
            {
                this.Additives = new BindableCollection<Additive>();
            }
            if (Additives.Count>0 && Additives.Any(a => a.Equals(additive)))
            {
                return false;
            }
            var additive1 = new Additive(additive);
            additive1.ParentOrderItem = this;
            additive1.State = AdditiveState.Added;
            this.Additives.Add(additive1);
            return true;
        }

        public void RemoveAdditive(Additive additive)
        {
            
           
        }

        
    }

    public enum OrderItemState
    {
        Added,Removed,IncrementedQuantity,DecrementedQuantity
    }
}
