using Caliburn.Micro;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using ServiceInterface.jsonConverters;
using Utilities.Attributes;
using System.Threading;

namespace ServiceInterface.Model
{
    public class Order : SyncModel, IValidatableObject, IState<long>,ILockable
    {

        private OrderItem _selectedOrderItem;
        private decimal _discountAmount = 0;
        private decimal _givenAmount;
        private decimal _returnedAmount;
        private decimal _discountPercentage;
        private BindableCollection<OrderItem> _orderItems;
        private Table _table;
        private Order _splittedFrom;
        private OrderState? _state;
        private OrderType? _type;
        private TimeSpan _elapsedTime;
        private BindableCollection<Order> _orders;
        private Deliveryman _deliveryman;
        private Waiter _waiter;

        public Order()
        {
            OrderItems = new BindableCollection<OrderItem>();

            OrderItems.CollectionChanged +=
                (s, e) =>
                {
                    if (OrderItems.Count == 0) DiscountAmount = 0;
                    NotifyOfPropertyChange(() => Total);
                    NotifyOfPropertyChange(() => TotalDiscountAmount);
                    NotifyOfPropertyChange(() => NewTotal);
                };
            OrderTime = DateTime.Now;
        }

        public Order(string buyerId) : this()
        {
            BuyerId = buyerId;
        }

        public Order(BindableCollection<Order> orders) : this()
        {
            Orders = orders;
            ProductsVisibility = true;
            ShownAdditivesPage = new BindableCollection<Additive>();
        }

        public BindableCollection<Order> Orders { get; set; }

        [DataMember(IsRequired = true)]
        public long? Id { get; set; }

        [DataMember]
        public int OrderNumber { get; set; }

        [DataMember]
        public String OrderCode { get; set; }

        [DataMember] public string BuyerId { get; set; }

        [DataMember]
        [Required]
        public DateTime OrderTime
        {
            get => _orderTime;
            set => Set(ref _orderTime, value);
        }

        public Deliveryman Deliveryman
        {
            get => _deliveryman;
            set
            {
                Set(ref _deliveryman, value);
                DeliverymanId = value?.Id;
            }
        }

        [DataMember] public long? DeliverymanId { get; set; }

        public Waiter Waiter
        {
            get => _waiter;
            set
            {
                Set(ref _waiter, value);
                WaiterId = value?.Id;
            }
        }

        [DataMember] public long? WaiterId { get; set; }

        [DataMember]
        [Required]
        [JsonConverter(typeof(TimespanConverter))]
        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public TimeSpan ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                _elapsedTime = value;

                NotifyOfPropertyChange();
            }
        }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderType? Type
        {
            get => _type;
            set
            {
                _type = value;
                NotifyOfPropertyChange();
            }
        }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderState? State
        {
            get => _state;
            set
            {
                _state = value;
                if (State == null)
                {
                    return;
                }

                if (OrderStates == null)
                {
                    OrderStates = new BindableCollection<OrderStateElement>();
                }

                NotifyOfPropertyChange();
            }
        }


        public IList<OrderStateElement> OrderStates { get; set; }


        public string Color { get; set; }

        [DataMember]
        public decimal Total => OrderItems.Where(i => i.State != OrderItemState.Removed).Sum(i => i.Total);


        public Order SplittedFrom
        {
            get => _splittedFrom;
            set
            {
                _splittedFrom = value;
                SplittedFromId = value.Id;
            }
        }

        [DataMember] public long? SplittedFromId { get; set; }


        [DataMember]
        public decimal NewTotal
        {
            get
            {

                return Total - TotalDiscountAmount;
            }
        }

        [DataMember]
        public decimal TotalDiscountAmount
        {
            get
            {
                var sumItemDiscounts = 0m;
                if (DiscountAmount == 0 && DiscountPercentage == 0 && OrderItems != null)
                {
                    OrderItems.Where(i => i.State != OrderItemState.Removed).ToList()
                        .ForEach(item => sumItemDiscounts += item.CalcTotalDiscount());
                }

                var allDiscounts = _discountAmount + sumItemDiscounts;

                return allDiscounts;
            }
        }

        [DataMember]
        public decimal DiscountAmount
        {
            get => _discountAmount;
            set
            {
                _discountAmount = value;
                if (Total > 0)
                {
                    _discountPercentage = value * 100 / Total;
                }
                NotifyOfPropertyChange(() => DiscountAmount);
                NotifyOfPropertyChange(() => TotalDiscountAmount);
                NotifyOfPropertyChange(() => NewTotal);
            }
        }

        [DataMember]
        public decimal DiscountPercentage
        {
            get => _discountPercentage;
            set
            {
                _discountPercentage = value;
                if (Total > 0)
                {
                    _discountAmount = (Total * value) / 100;
                }
                NotifyOfPropertyChange(() => DiscountPercentage);
                NotifyOfPropertyChange(() => TotalDiscountAmount);
                NotifyOfPropertyChange(() => NewTotal);
            }
            //OrderItems.Cast<OrderItem>().ToList().Sum(item => item.Total); 
        }

        [DataMember]
        public decimal GivenAmount
        {
            get => _givenAmount;
            set
            {
                _givenAmount = value;
                NotifyOfPropertyChange(() => GivenAmount);
            }
            //OrderItems.Cast<OrderItem>().ToList().Sum(item => item.Total); 
        }

        [DataMember]
        public decimal ReturnedAmount
        {
            get => _returnedAmount;
            set
            {
                _returnedAmount = value;
                NotifyOfPropertyChange(() => ReturnedAmount);
            }
            //OrderItems.Cast<OrderItem>().ToList().Sum(item => item.Total); 
        }

        //private readonly List<OrdreItem> _items = new List<OrdreItem>();
        public OrderItem SelectedOrderItem
        {
            get => _selectedOrderItem;
            set
            {
                _selectedOrderItem = value;
                NotifyOfPropertyChange(() => SelectedOrderItem);
            }
        }

        [DataMember]
        [ObserveMutations(MutationObserverFlags.CollectionObservingItems)]
        public BindableCollection<OrderItem> OrderItems
        {
            get => _orderItems;
            set
            {
                _orderItems = value;
                NotifyOfPropertyChange(() => OrderItems);
            }
        }

        //[DataMember]
        public List<long> OrderItemIds { get; set; }

        public Customer Customer
        {
            get => _customer;
            set
            {
                _customer = value;
                CustomerId = _customer?.Id;
                NotifyOfPropertyChange(() => Customer);
            }
        }

        private Customer _customer;
        private DateTime _orderTime;

        [DataMember] public long? CustomerId { get; set; }

        public Table Table
        {
            get => _table;
            set
            {
                //if (value == null)
                //{
                //    (_table?.OrderViewSource?.Source as ICollection<Order>)?.Remove(this);
                //    _table?.OrderViewSource?.View?.Refresh();
                //}

                _table = value;
                TableId = _table?.Id;
                if (value != null)
                {
                    var orders = (_table?.OrderViewSource?.Source as ICollection<Order>);
                    if (orders != null && !orders.Any(o => o.OrderNumber == this.OrderNumber))
                    {
                        orders.Add(this);
                    }

                    _table?.OrderViewSource?.View?.Refresh();
                }

                NotifyOfPropertyChange(() => Table);
            }
        }

        [DataMember] public long? TableId { get; set; }

        public Category ShownCategory { get; set; }
        public BindableCollection<Additive> ShownAdditivesPage { get; set; } = new BindableCollection<Additive>();

        private bool _ProductsVisibility;

        public bool ProductsVisibility
        {
            get { return _ProductsVisibility; }
            set
            {
                _ProductsVisibility = value;
                NotifyOfPropertyChange(nameof(ProductsVisibility));
            }
        }

        public bool AdditivesVisibility { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Type == OrderType.Delivery && Deliveryman == null)
            {
                yield return new ValidationResult(
                    $"Delivery Man must not be null if Order Type is {OrderType.Delivery} ",
                    new[] { nameof(Deliveryman) });
            }

            if (Type == OrderType.OnTable && Table == null)
            {
                yield return new ValidationResult(
                    $"Table must not be null if Order Type is {OrderType.OnTable} ",
                    new[] { nameof(Table) });
            }

            if (Total < 0)
            {
                yield return new ValidationResult(
                    $"Total must be a positive number.",
                    new[] { nameof(Total) });
            }

            if (TotalDiscountAmount > Total)
            {
                yield return new ValidationResult(
                    $"Total discount amount must not exceed the value of Total.",
                    new[] { nameof(TotalDiscountAmount) });
            }

            if (TotalDiscountAmount < 0)
            {
                yield return new ValidationResult(
                    $"Total discount amount must be a positive number.",
                    new[] { nameof(TotalDiscountAmount) });
            }

            if (DiscountPercentage > 100)
            {
                yield return new ValidationResult(
                    $"Discount Percentage must not exceed 100%",
                    new[] { nameof(DiscountPercentage) });
            }

            if (DiscountPercentage < 0)
            {
                yield return new ValidationResult(
                    $"Discount Percentage must be a positive value",
                    new[] { nameof(TotalDiscountAmount) });
            }
        }


        public decimal Remaining => NewTotal - GivenAmount;

        public override string ToString()
        {
            return $"Order #{OrderNumber}";
        }
    }

    [DataContract]
    public class OrderStateElement
    {
        [DataMember] public long SessionId { get; set; }

        [DataMember] public DateTime StateTime { get; set; }


        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderState State { get; set; }
    }

    public enum OrderState
    {
        Ordered,
        Prepared,
        Ready,
        Delivered,
        Payed,
        Refunded,
        Splitted,
        Canceled,
        Removed,
        Served,
        Changed,
        DeliveredPaid,
        DeliveredReturned,
        Unprocessed,
        DeliveredPartiallyPaid,
        Credit,
        CreditRePaid,
        CreditPartiallyRePaid
    }

    public enum OrderType
    {
        Delivery,
        OnTable,
        TakeAway,
        InWaiting
    }

    public interface ILockable
    {
        bool IsLocked { get; set; }
        bool IsModifiable { get; }
        string LockedBy { get; set; }
    }

    [DataContract]
    public class SyncModel : PropertyChangedBase, ILockable
    {
        protected bool _isLocked;

        [DataMember]
        public virtual bool IsLocked
        {
            get { return _isLocked; }
            set
            {
                Set(ref _isLocked, value);
                NotifyOfPropertyChange(nameof(IsModifiable));
            }
        }

        private string _lockedBy;


        [DataMember]
        public string LockedBy
        {
            get { return _lockedBy; }
            set
            {
                Set(ref _lockedBy, value);
                NotifyOfPropertyChange(nameof(IsModifiable));
            }
        }

        public bool IsModifiable => !IsLocked || (IsLocked && LockedBy == Thread.CurrentPrincipal.Identity.Name);



    }

    public interface IMessage<  T>:IMessage
    {
        new T Content {  set; get;}
    }

    public interface IMessage 
    {
        object Content { get;  }
        string Source { get;  }
        string Type { get;  }
    }

    public class Message<T> : IMessage<T>
    {
        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public T Content { get; set; }
        object IMessage.Content => Content;
    }

    



    [DataContract]
    public class OrderData
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public decimal Total { get; set; }
        [DataMember]
        public decimal NewTotal { get; set; }
        [DataMember]
        public int OrderNumber { get; set; }
        [DataMember]
        public DateTime OrerTime { get; set; }

        [DataMember]
        public string CanceledBy { get; set; }
    }
}