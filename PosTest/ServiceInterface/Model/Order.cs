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

        public delegate void TotalChangedEvent(decimal newAmount);
        public event TotalChangedEvent TotalChanged;
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
                NotifyOfPropertyChange(() => Total);
            }
                //OrderItems.Cast<OrderItem>().ToList().Sum(item => item.Total); 
        }

        //private readonly List<OrdreItem> _items = new List<OrdreItem>();
        public BindableCollection<OrderItem> OrderItems { get; set; }

        public Order() {
            this.TotalChanged += TotalChangedEventHandler;
        }
        public Order(string buyerId) : this()
        {           
            BuyerId = buyerId;
        }

        public void OnTotalChanged(decimal newAmount)
        {
            TotalChanged(newAmount);
        }

        protected virtual void TotalChangedEventHandler(decimal newAmount)
        {            
            Total += newAmount;
        }
        public void AddItem(Product product, decimal unitPrice, int quantity = 1)
        {
            if (!OrderItems.Any(p => p.Product.Equals(product)))
            {
                OrderItems.Add(new OrderItem(product, quantity, unitPrice, this));
                return;
            }
            var existingItem = OrderItems.FirstOrDefault(p => p.Product.Equals(product));
            existingItem.AddQuantity(quantity);
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

    }
}
