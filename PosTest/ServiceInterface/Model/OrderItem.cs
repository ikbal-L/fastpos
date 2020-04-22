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
                _quantity = value;
                NotifyOfPropertyChange(() => Quantity);
                NotifyOfPropertyChange(() => Total);
                Order.Total += UnitPrice;
            } 
        }
        public decimal Total {
            get
            {
                return Quantity * UnitPrice;
            }           
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

        public void AddQuantity(int quantity)
        {
            Quantity += quantity;
        }

        public void SetNewQuantity(int quantity)
        {
            Quantity = quantity;
        }

        public void AddAdditives(Additive additive)
        {
           /* if (this.Additives == null)
                this.Additives = new BindableCollection<Additive>();*/
           this.Additives.Add(additive);
        }

        public void RemoveAdditives(Additive additive)
        {
           this.Additives.Remove(additive);
        }
    }
}
