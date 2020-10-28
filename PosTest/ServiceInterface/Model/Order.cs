using Caliburn.Micro;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServiceInterface.Authorisation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace ServiceInterface.Model
{
    public class Order : PropertyChangedBase
    {
        private decimal _total;
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
        private Delivereyman _delivereyman;
        private Waiter _waiter;

        public Order()
        {
            OrderItems = new BindableCollection<OrderItem>();
            
            OrderItems.CollectionChanged += 
                (s, e)=> 
                {
                    if (OrderItems.Count == 0) this.DiscountAmount = 0;
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
            this.Orders = orders;
        }

        public BindableCollection<Order> Orders { get; set; }

        [DataMember(IsRequired = true)]
        public long? Id { get; set; }

        [DataMember]
        public string BuyerId { get; set; }

        [DataMember]
        public DateTime OrderTime { get; set; }

        public Delivereyman Delivereyman
        {
            get => _delivereyman;
            set
            {
                Set(ref _delivereyman, value);
                DelivereymanId = value?.Id;
            }
        }

        [DataMember]
        public long? DelivereymanId { get; set; }

        public Waiter Waiter
        {
            get => _waiter;
            set
            {
                Set(ref _waiter, value);
                WaiterId = value?.Id;
            }
        }

        [DataMember]
        public long? WaiterId { get; set; }

        [DataMember]
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
                OrderStates.Add(new OrderStateElement
                {
                    State = (OrderState)_state,
                    StateTime = DateTime.Now,
                    SessionId = AuthProvider.Instance.SessionId
                });
                NotifyOfPropertyChange();
            }
        }

        [DataMember]
        public IList<OrderStateElement> OrderStates { get; set; }
        //get from state
        public string Color { get; set; }

        [DataMember]
        public decimal Total 
        {
            get
            {
                var sumItemTotals = 0m;
                if (OrderItems != null)
                {
                    OrderItems.ToList().ForEach(item => sumItemTotals += item.Total);
                }
                return sumItemTotals;
            }
            /*set
            {
                _total = value;
                NotifyOfPropertyChange(() => NewTotal);
                NotifyOfPropertyChange(() => Total);
            }*/
        } 
        
        public Order SplittedFrom
        {
            get => _splittedFrom;
            set
            {
                _splittedFrom = value;
                SplittedFromId = value.Id;
            }
        }
            
        [DataMember]
        public long? SplittedFromId
        {
            get ;
            set;
        }
            

        [DataMember]
        public decimal NewTotal 
        {
            get
            {
                /*var sumItemDiscounts = 0m;
                OrderItems.ToList().ForEach(item => sumItemDiscounts += item.DiscountAmount);
                var allDiscounts = _discountAmount + sumItemDiscounts;*/
                return Total - TotalDiscountAmount;
            }
        }

        [DataMember]
        public decimal TotalDiscountAmount
        {
            get
            {
                var sumItemDiscounts = 0m;
                if (OrderItems != null)
                {
                    OrderItems.ToList().ForEach(item => sumItemDiscounts += item.CalcTotalDiscount());
                }
                //either _discountAmount==0 or _discountPercentage==0
                var allDiscounts = _discountAmount + sumItemDiscounts + Total * _discountPercentage / 100;
                //NewTotal = Total - allDiscounts;
                return allDiscounts;
            }
        }

        [DataMember]
        public decimal DiscountAmount 
        {
            get =>  _discountAmount ;
            set
            {
                _discountAmount = value;
                _discountPercentage = 0;
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
                _discountAmount = 0;
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

        public Session Session { get; set; }

        [DataMember]
        public long SessionId { get; set; }

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

        [DataMember]
        public long? CustomerId { get; set; }

        public Table Table 
        {
            get => _table;
            set
            {
                _table = value;
                TableId = _table?.Id;
                NotifyOfPropertyChange(()=>Table);
            }
        }

        [DataMember]
        public long? TableId { get; set; }

        public Category ShownCategory { get; private set; }
        public BindableCollection<Additive> ShownAdditivesPage { get; set; }

        public bool ProductsVisibility { get; set; }
        public bool AdditivesVisibility { get; set; }
        public void AddOrderItem(Product product, decimal unitPrice, bool setSelected, float quantity = 1, bool groupByProduct = true)
        {
            OrderItem item;
            if ((product is Platter && (product as Platter).Additives !=null )  || 
                !OrderItems.Any(p => p.Product.Equals(product)) || !groupByProduct)
            {
                item = new OrderItem(product, quantity, unitPrice, this);
                OrderItems.Add(item);
                
            }
            else
            {
               item = OrderItems.FirstOrDefault(p => p.Product.Equals(product));
               item.Quantity+=quantity;
            }

            if (setSelected)
            {
                SelectedOrderItem = item;
            }
        }

        public void AddOrderItem(OrderItem orderItem)
        {
            AddOrderItem(orderItem.Product, orderItem.UnitPrice, false, orderItem.Quantity);
        }

        public void RemoveEmptyItems()
        {
            var orderitems = OrderItems.Cast<OrderItem>().ToList();
            orderitems.RemoveAll(item => item.Quantity == 0);
            OrderItems = new BindableCollection<OrderItem>(orderitems);
        }

        public void SetNewBuyerId(string buyerId)
        {
            BuyerId = buyerId;
        }

        public void RemoveOrderItem(OrderItem item)
        {
            if (item == null)
                return;
            //Total -= item.Total;
            
            OrderItems.Remove(item);
        }

        public void MappingBeforeSending()
        {
            foreach (var oitem in OrderItems) 
            { 
                oitem.ProductId = (long)oitem.Product.Id; 
                oitem.OrderId =Id;            
            }

            this.TableId = Table?.Id;
            this.CustomerId = Customer?.Id;
        }

        public void MappingAfterReceiving(IEnumerable<Product> products)
        {
            if (products==null)
            {
                throw new MappingException("Order mapping products is null");
            }

            foreach (var oit in OrderItems)
            {
                var product = products.Where(p => p.Id == oit.ProductId).FirstOrDefault();
                if (product == null)
                {
                    throw new MappingException("Order mapping product is null");
                }
                oit.Product = product;
            }
        }

        public void SaveScreenState(Category category, BindableCollection<Additive> additives,
            bool productsVisibility, bool additviesVisibility)
        {
            ShownCategory = category;
            ShownAdditivesPage = additives;
            ProductsVisibility = productsVisibility;
            AdditivesVisibility = additviesVisibility;
        }
    }

    [DataContract]
    public class OrderStateElement
    {

        [DataMember]
        public long SessionId { get; set; }

        [DataMember]
        public DateTime StateTime { get; set; }


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
        Splitted, 
        Canceled,
        Removed,
        Served,
        Changed
    }
    public enum OrderType
    {
        Delivery,
        OnTable,
        Takeaway,
        InWaiting
    }
}
