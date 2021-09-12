using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FastPosFrontend.Helpers
{
    public static class OrderEx
    {
        public static OrderItem AddOrderItem(this Order order,Product product, bool setSelected, float quantity = 1,
           bool groupByProduct = true)
        {
            OrderItem item;
            if (product.IsPlatter && (product.Additives != null) || order.OrderItems.All(p => p.ProductId != product.Id) ||
                !groupByProduct ||
               order. OrderItems.Where(oi =>
                    oi.ProductId == product.Id
                ).All(oi => oi.State == OrderItemState.Removed)
            )
            {
                item = new OrderItem(product, quantity, order) {State = OrderItemState.Added, TimeStamp = null};
                order.OrderItems.Add(item);
            }
            else
            {
                item = order.OrderItems.FirstOrDefault(orderItem => orderItem.ProductId == product.Id && orderItem.State != OrderItemState.Removed);
                item.Quantity += quantity;
            }

            if (setSelected)
            {
                order.SelectedOrderItem = item;
            }

            return item;
        }

        public static OrderItem AddOrderItem(this Order order, OrderItem orderItem, bool setSelected = false)
        {
            return AddOrderItem(order,orderItem.Product, setSelected, orderItem.Quantity);
        }


        public static void RemoveEmptyItems(this Order order)
        {
            var orderitems = order.OrderItems.Cast<OrderItem>().ToList();
            orderitems.RemoveAll(item => item.Quantity == 0);
            order.OrderItems = new BindableCollection<OrderItem>(orderitems);
        }

        public static void SetNewBuyerId(this Order order,string buyerId)
        {
            order.BuyerId = buyerId;
        }

        public static void RemoveOrderItem(this Order order,OrderItem item)
        {
            if (item == null) return;


            order.OrderItems.Remove(item);

            //if (item.TimeStamp == null)
            //{
            //    order.OrderItems.Remove(item);

            //}
            //else
            //{
            //    item.State = OrderItemState.Removed;
            //}
        }

        public static void UpdateOrderFrom(this Order order,Order updateSource)
        {
            order?.OrderItems?.Clear();
            order?.OrderItems?.AddRange(updateSource.OrderItems);
        }

        public static void MappingBeforeSending(this Order order)
        {
            foreach (var orderItem in order.OrderItems)
            {
                if (orderItem.Product?.Id != null)
                {
                    orderItem.ProductId = (long)orderItem.Product?.Id;

                }
                orderItem.OrderId = order.Id;
            }


            order.TableId = order.Table?.Id;
            order.CustomerId = order.Customer?.Id;
            order.DeliverymanId = order.Deliveryman?.Id;
        }

        public static void MappingAfterReceiving(this Order order,IEnumerable<Product> products)
        {
            if (products == null)
            {
                throw new Exception("Order mapping products is null");
            }

            foreach (var oit in order.OrderItems)
            {
                var additives = new List<Additive>();
                var product = products.FirstOrDefault(p => p.Id == oit.ProductId);
                if (product != null)
                {
                    if (product.IsPlatter)
                    {
                        var additivesOfProduct = product?.Additives.Where(a =>
                                  oit.OrderItemAdditives.Any(orderItemAdditive => orderItemAdditive.AdditiveId == a.Id)).ToList();
                        foreach (var additive in additivesOfProduct)
                        {
                            var newAdditive = new Additive(additive) { ParentOrderItem = oit };
                            additives.Add(newAdditive);
                        }

                        oit.Order = order;
                    }

                    if (product == null)
                    {
                        throw new Exception("Order mapping product is null");
                    }
                    oit.Additives = new BindableCollection<Additive>(additives);

                    oit.Product = product;
                }
            }
        }

        public static void SaveScreenState(this Order order,Category category, BindableCollection<Additive> additives,
            bool productsVisibility, bool additviesVisibility)
        {
            order.ShownCategory = category;
            order.ShownAdditivesPage = additives;
            order.ProductsVisibility = productsVisibility;
            order.AdditivesVisibility = additviesVisibility;
        }
    } 
}
