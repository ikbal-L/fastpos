using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Caliburn.Micro;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServiceInterface.Model
{
    public class OrderItem : PropertyChangedBase, IValidatableObject, IState<long>
    {
        private static readonly bool IsRunningFromXUnit =
            AppDomain.CurrentDomain.GetAssemblies().Any(
                a => a.FullName.StartsWith("XUnitTesting"));

        private float _quantity;
        private BindableCollection<Additive> _additives;
        private Additive __selectedAdditive;
        private bool _canAddAdditives;
        private decimal _discountAmount;
        private decimal _discountPercentage;
        private bool _isSelected;

        public OrderItem()
        {
            //Additives = new BindableCollection<Additive>();
        }

        public OrderItem(Product product, float quantity, decimal unitPrice, Order order) : this()
        {
            Order = order;
            Product = product;
            ProductName = product?.Name;
            if (product?.Id != null)
            {
                ProductId = (long) product?.Id;
            }

            UnitPrice = unitPrice;
            Quantity = quantity;
            OrderItemAdditives = new List<OrderItemAdditive>();
            Additives = new BindableCollection<Additive>();
        }

        [DataMember] public long? Id { set; get; }

        public Order Order { get; set; }

        [DataMember] public long? OrderId { get; set; }

        [DataMember] public long ProductId { get; set; }

        [DataMember]
        public string ProductName
        {
            get => _productName;
            set => Set(ref _productName, value);
        }

        [DataMember]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [DataMember]
        [Range(1, float.MaxValue)]
        public float Quantity
        {
            get => _quantity;
            set
            {
                var oldQuqntity = _quantity;
                _quantity = value;
                NotifyOfPropertyChange(() => Quantity);
                NotifyOfPropertyChange(() => Total);
                NotifyOfPropertyChange(nameof(TotalDiscountAmount));
                Order?.NotifyOfPropertyChange(nameof(Order.Total));
                Order?.NotifyOfPropertyChange(nameof(Order.NewTotal));
                Order?.NotifyOfPropertyChange(nameof(TotalDiscountAmount));
           
            }
        }

        [DataMember]
        [Range(0, double.MaxValue)]
        public decimal Total
        {
            get => (decimal) Quantity * UnitPrice;
        }

        [DataMember]
        public decimal TotalDiscountAmount
        {
            get
            {
                Order?.NotifyOfPropertyChange(nameof(Order.TotalDiscountAmount));
                Order?.NotifyOfPropertyChange(nameof(Order.NewTotal));
                return CalcTotalDiscount();
            }
        }

        public decimal CalcTotalDiscount()
        {
            //var totalDiscount = _discountAmount * (decimal)Quantity + Total * _discountPercentage / 100;
            var totalDiscount = _discountAmount * (decimal) Quantity;
            return totalDiscount;
        }

        [DataMember]
        public decimal DiscountAmount
        {
            get => _discountAmount;
            set
            {
                _discountAmount = value;
                _discountPercentage = value * 100 / Total;
                NotifyOfPropertyChange(() => DiscountAmount);
                NotifyOfPropertyChange(() => TotalDiscountAmount);

                Order?.NotifyOfPropertyChange(nameof(Order.TotalDiscountAmount));
                Order?.NotifyOfPropertyChange(nameof(Order.NewTotal));
            }
        }

        [DataMember]
        public decimal DiscountPercentage
        {
            get => _discountPercentage;
            set
            {
                _discountPercentage = value;
                _discountAmount = (Total * _discountPercentage / 100);
                NotifyOfPropertyChange(() => DiscountPercentage);
                NotifyOfPropertyChange(() => TotalDiscountAmount);

                Order?.NotifyOfPropertyChange(nameof(Order.TotalDiscountAmount));
                Order?.NotifyOfPropertyChange(nameof(Order.NewTotal));
            }
        }


        [DataMember]
        public DateTime? TimeStamp
        {
            get => _timeStamp;
            set => Set(ref _timeStamp, value);
        }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderItemState State
        {
            get => _state;
            set => Set(ref _state, value);
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
                    if (!oitem.Equals(this))
                        oitem.SetSelectedAdditive(null);
                }

                NotifyOfPropertyChange(() => SelectedAdditive);
            }
        }

        private void SetSelectedAdditive(Additive additive)
        {
            __selectedAdditive = null;
            NotifyOfPropertyChange(() => SelectedAdditive);
        }

        [DataMember] public List<OrderItemAdditive> OrderItemAdditives { get; set; }

        private decimal _totalDiscountAmount;
        private DateTime? _timeStamp;
        private OrderItemState _state;
        private string _productName;

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
            get => (Product != null) && Product.IsPlatter && Product.Additives != null;
            set { _canAddAdditives = value; }
        }


        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(nameof(IsSelected));
            }
        }


        public Product Product { get; set; }


        public bool AddAdditives(Additive additive)
        {
            Additives ??= new BindableCollection<Additive>();
            if (Additives.Count > 0 && Additives.Any(a=> a.Equals(additive)))
            {
                return false;
            }

            var additive1 = new Additive(additive) {ParentOrderItem = this};
            Additives.Add(additive1);
            if (additive1.Id != null)
                OrderItemAdditives.Add(new OrderItemAdditive
                    {AdditiveId = (long) additive1.Id, OrderItemId = Id, State = AdditiveState.Added});
            return true;
        }

        public void RemoveAdditive(Additive additive)
        {
            if (Order.Id == null)
            {
                OrderItemAdditives.RemoveAll(orderItemAdditive => orderItemAdditive.AdditiveId == additive.Id);
            }
            else
            {
                OrderItemAdditives.Find(oia => oia.AdditiveId == additive.Id).State = AdditiveState.Removed;
            }

            Additives.Remove(additive);
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UnitPrice < 0)
            {
                yield return new ValidationResult(
                    "Unit price must be a positive number.",
                    new[] {nameof(UnitPrice)});
            }

            if (Quantity < 1)
            {
                yield return new ValidationResult(
                    "Quantity must be greater than or equal to 1.",
                    new[] {nameof(Quantity)});
            }

            if (Total < 0)
            {
                yield return new ValidationResult(
                    "Total must be a positive number.",
                    new[] {nameof(Total)});
            }

            if (TotalDiscountAmount > UnitPrice)
            {
                yield return new ValidationResult(
                    "Total discount amount must not exceed the value of Unit price ",
                    new[] {nameof(TotalDiscountAmount)});
            }

            if (TotalDiscountAmount < 0)
            {
                yield return new ValidationResult(
                    "Total discount amount must be a positive number.",
                    new[] {nameof(TotalDiscountAmount)});
            }

            if (DiscountPercentage > 100)
            {
                yield return new ValidationResult(
                    "Discount Percentage must not exceed 100%",
                    new[] {nameof(DiscountPercentage)});
            }

            if (DiscountPercentage < 0)
            {
                yield return new ValidationResult(
                    "Discount Percentage must be a positive value",
                    new[] {nameof(DiscountPercentage)});
            }
        }
    }

    public enum OrderItemState
    {
        Added,
        Removed,
        IncreasedQuantity,
        DecreasedQuantity
    }
}