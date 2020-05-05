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
        private BindableCollection<Additive> _additives;
        private Additive __selectedAdditive;
        private bool _canAddAdditives;

        public OrderItem() 
        {
            Additives = new BindableCollection<Additive>();
        }

        public OrderItem(Product product, int quantity, decimal unitPrice, Order order) : this()
        {
            Order = order;
            Product = product;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

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
                var oldQuqntity = _quantity;
                _quantity = value;
                NotifyOfPropertyChange(() => Quantity);
                NotifyOfPropertyChange(() => Total);
                Order.Total = Order.Total + UnitPrice * (_quantity - oldQuqntity);
                //decimal total = 0;
                //Order.OrderItems.Cast<OrderItem>().ToList().ForEach(item => total += item.Total);
                //Order.Total = total;
            } 
        }
        public decimal Total {
            get => Quantity * UnitPrice;          
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
        public Product Product { get; set; }

        public void AddAdditives(Additive additive)
        {
            /* if (this.Additives == null)
                 this.Additives = new BindableCollection<Additive>();*/
            var additive1 = new Additive(additive);
            additive1.ParentOrderItem = this; 
            this.Additives.Add(additive1);
        }

        public void RemoveAdditives(Additive additive)
        {
            Additives.Remove(additive);
        }
    }
}
