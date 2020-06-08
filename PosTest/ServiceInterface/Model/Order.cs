using Caliburn.Micro;
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

        public Order()
        {
            OrderItems = new BindableCollection<OrderItem>();
            OrderItems.CollectionChanged += new NotifyCollectionChangedEventHandler((s, e)=> NotifyOfPropertyChange(()=>Total));
        }
        public Order(string buyerId) : this()
        {
            BuyerId = buyerId;
        }

        [DataMember(IsRequired = true)]
        public long? Id { get; set; }

        [DataMember]
        public string BuyerId { get; set; }

        [DataMember]
        public DateTime OrderTime { get; set; }

        [DataMember]
        public OrderType Type { get; set; }

       [DataMember]
        public OrderState? State { get; set; }

        //get from state
        public string Color { get; set; }

        [DataMember]
        public decimal Total 
        {
            get
            {
                var sumItemTotals = 0m;
                OrderItems.ToList().ForEach(item => sumItemTotals += item.Total);
                return sumItemTotals;
            }
            /*set
            {
                _total = value;
                NotifyOfPropertyChange(() => NewTotal);
                NotifyOfPropertyChange(() => Total);
            }*/
        }

        [DataMember]
        public decimal NewTotal 
        {
            get
            {
                var sumItemDiscounts = 0m;
                OrderItems.ToList().ForEach(item => sumItemDiscounts += item.DiscountAmount);
                var allDiscounts = _discountAmount + sumItemDiscounts;
                return Total - allDiscounts;
            }
        }

        [DataMember]
        public decimal DiscountAmount 
        {
            get =>  _discountAmount ;
            set
            {
                _discountAmount = value;
                NotifyOfPropertyChange(() => DiscountAmount);
                NotifyOfPropertyChange(() => TotalDiscountAmount);
                NotifyOfPropertyChange(() => NewTotal);
            }
        }

        [DataMember]
        public decimal TotalDiscountAmount
        {
            get
            {
                var sumItemDiscounts = 0m;
                OrderItems.ToList().ForEach(item => sumItemDiscounts += item.DiscountAmount);
                var allDiscounts = _discountAmount + sumItemDiscounts;
                //NewTotal = Total - allDiscounts;
                return allDiscounts;
            }
        }

        [DataMember]
        public decimal DiscountPercentage 
        {
            get => _discountPercentage;
            set
            {
                _discountPercentage = value;
                DiscountAmount = Total * _discountPercentage / 100;
                NotifyOfPropertyChange(() => DiscountPercentage);
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
        public BindableCollection<OrderItem> OrderItems { get; set; }

        //[DataMember]
        public List<long> OrderItemIds { get; set; }

        public Session Session { get; set; }

        [DataMember]
        public long SessionId { get; set; }

        public Customer Customer { get; set; }

        [DataMember]
        public long? CustomerId { get; set; }

        public Table Table { get; set; }

        [DataMember]
        public long? TableId { get; set; }

        public Category ShownCategory { get; private set; }
        public ICollectionView ShownAdditivesPage { get; set; }

        public bool ProductsVisibility { get; set; }
        public bool AdditivesVisibility { get; set; }
        public void AddItem(Product product, decimal unitPrice, bool setSelected, float quantity = 1)
        {
            OrderItem item;
            if ((product is Platter && (product as Platter).Additives !=null )  || !OrderItems.Any(p => p.Product.Equals(product)))
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
            AddItem(orderItem.Product, orderItem.UnitPrice, false, orderItem.Quantity);
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
                oitem.OrderId = (long)Id;            
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

        public void SaveScreenState(Category category, ICollectionView additives,
            bool productsVisibility, bool additviesVisibility)
        {
            ShownCategory = category;
            ShownAdditivesPage = additives;
            ProductsVisibility = productsVisibility;
            AdditivesVisibility = additviesVisibility;
        }
    }

    public enum OrderState 
    {
        Ordered,
        Prepared,
        Ready,
        Delivered,
        Payed
    }
    public enum OrderType
    {
        Delivery,
        OnTable,
        Takeaway
    }
}
