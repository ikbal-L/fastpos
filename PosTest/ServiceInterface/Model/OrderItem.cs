using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Model
{
    public class OrderItem : PropertyChangedBase
    {
        private int _quantity;

        public int Id { set; get; }
        public Order Order { get; set; }
        public int OrderId { get => Order.Id; }
        public int ProductId { get => Product.Id; }
        public decimal UnitPrice { get; set; }
        public int Quantity 
        { 
            get => _quantity;
            set
            {
                _quantity = value;
                NotifyOfPropertyChange(() => Quantity);
                NotifyOfPropertyChange(() => Total);
                Order.OnTotalChanged(UnitPrice);
            } 
        }
        public decimal Total {
            get
            {
                return Quantity * UnitPrice;
            }           
        }

        public OrderItem() { }

        public OrderItem(Product product, int quantity, decimal unitPrice, Order order)
        {
            Order = order;
            Product = product;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        public Product Product { get; set; }

        public void AddQuantity(int quantity)
        {
            Quantity += quantity;
        }

        public void SetNewQuantity(int quantity)
        {
            Quantity = quantity;
        }

    }
}
