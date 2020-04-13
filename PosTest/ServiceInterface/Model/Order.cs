using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Model
{
    public class Order 
    {
        public Order()
        {

        }
        public int Id { get; set; }
        public string BuyerId { get; set; }

        public DateTime dateOrder { get; set; }

        public String Name { get; set; }
        public String Type { get; set; }
        public String Color { get; set; }
        
        private readonly List<OrdreItem> _items = new List<OrdreItem>();
        public BindableCollection<OrdreItem> OrderItems { get; set; }

        public Order(string buyerId)
        {
            BuyerId = buyerId;
        }

        public void AddItem(Product product, decimal unitPrice, int quantity = 1)
        {
            if (!OrderItems.Any(p => p.Product.Equals(product)))
            {
                OrderItems.Add(new OrdreItem(product, quantity, unitPrice));
                return;
            }
            var existingItem = OrderItems.FirstOrDefault(p => p.Product.Equals(product));
            existingItem.AddQuantity(quantity);
        }

        public void RemoveEmptyItems()
        {
            _items.RemoveAll(i => i.Quantity == 0);
        }

        public void SetNewBuyerId(string buyerId)
        {
            BuyerId = buyerId;
        }

    }
}
