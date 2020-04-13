using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Model
{
    public class OrdreItem : PropertyChangedBase
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
            } 
        }
        public decimal Total {
            get
            {
                return Quantity * UnitPrice;
            }           
        }

        public OrdreItem() { }

        public OrdreItem(Product product, int quantity, decimal unitPrice)
        {
            Product = product;
            Quantity = quantity;
            UnitPrice = unitPrice;
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
