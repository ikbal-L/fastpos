using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Model
{
    public class Order : PropertyChangedBase
    {
        private decimal _total;
        private decimal _newTotal;
        private OrderItem _selectedOrderItem;
        private decimal _discount = 0;
        private decimal _payedAmount;
        private decimal _returnedAmount;

        public Order()
        {
            OrderItems = new BindableCollection<OrderItem>();
        }
        public Order(string buyerId) : this()
        {
            BuyerId = buyerId;
        }

        public int Id { get; set; }
        public string BuyerId { get; set; }

        public DateTime dateOrder { get; set; }

        public String Name { get; set; }
        public String Type { get; set; }
        public String Color { get; set; }

        public decimal Total 
        {
            get => _total;
            set
            {
                _total = value;
                NewTotal = _total - _discount;
                NotifyOfPropertyChange(() => Total);
            }
                //OrderItems.Cast<OrderItem>().ToList().Sum(item => item.Total); 
        }
        
        public decimal NewTotal 
        {
            get => _newTotal;
            set
            {
                _newTotal = value;
                NotifyOfPropertyChange(() => NewTotal);
            }
                //OrderItems.Cast<OrderItem>().ToList().Sum(item => item.Total); 
        }

        public decimal Discount 
        {
            get => _discount;
            set
            {
                _discount = value;
                NotifyOfPropertyChange(() => Discount);
            }
                //OrderItems.Cast<OrderItem>().ToList().Sum(item => item.Total); 
        }

        public decimal PayedAmount 
        {
            get => _payedAmount;
            set
            {
                _payedAmount = value;
                NotifyOfPropertyChange(() => PayedAmount);
            }
                //OrderItems.Cast<OrderItem>().ToList().Sum(item => item.Total); 
        }

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
        public BindableCollection<OrderItem> OrderItems { get; set; }

        public void AddItem(Product product, decimal unitPrice, bool setSelected, int quantity = 1)
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
                SelectedOrderItem = item;
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

        public void DeleteOrderItem(OrderItem item)
        {
            Total -= item.Total;
            OrderItems.Remove(item);
        }
    }
}
