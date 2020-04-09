using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Model
{
    public class OrdreItem
    {
        public int Id { set; get; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total {
            get
            {
                return Quantity * UnitPrice;
            }
            set
            {
                value = Quantity * UnitPrice; ;
            }
        }

        public OrdreItem()
        {

        }

        public OrdreItem(int productId, int quantity, decimal unitPrice)
        {
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public Product product { get; set; }

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
