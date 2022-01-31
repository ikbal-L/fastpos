using Caliburn.Micro;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Extensions.Collections;

namespace FastPosFrontend.Helpers
{
    public static class OrderEx
    {
        public static OrderItem AddOrderItem(this Order order,Product product, bool setSelected, float quantity = 1,
           bool groupByProduct = true)
        {
            OrderItem item;
            if (product.IsPlatter && (product?.Additives != null) || order.OrderItems.All(p => p.ProductId != product.Id) ||
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
            order.State = updateSource.State;
            order.Type = updateSource.Type;
            if (order.WaiterId != updateSource.WaiterId) UpdateWaiter();
            if (order.CustomerId != updateSource.CustomerId) UpdateCustomer();
            if (order.DeliverymanId != updateSource.DeliverymanId) UpdateDeliveryman();
            if (order.TableId != updateSource.TableId) UpdateTable();

            //TODO Fix update observation issue 

            HandleAddedItems(order, updateSource);

            HandleRemovedOrModifiedItems(order, updateSource);



            void UpdateWaiter()
            {
                if (updateSource.WaiterId == null)
                {
                    order.Waiter = null;
                    order.WaiterId = null;
                    return;
                }
                var waiter = StateManager.GetById<Waiter>(updateSource.WaiterId.Value);
                order.WaiterId = updateSource.WaiterId;
                order.Waiter = waiter;
            }

            void UpdateCustomer()
            {
                if (updateSource.CustomerId == null)
                {
                    order.CustomerId = null;
                    order.Customer = null;
                    return;
                }

                var customer = StateManager.GetById<Customer>(updateSource.CustomerId.Value);
                order.CustomerId = updateSource.CustomerId;
                order.Customer = customer;
            }

            void UpdateDeliveryman()
            {
                if (updateSource.DeliverymanId == null)
                {
                    order.DeliverymanId = null;
                    order.Deliveryman = null;
                    return;
                }

                var deliveryman = StateManager.GetById<Deliveryman>(updateSource.DeliverymanId.Value);
                order.DeliverymanId = updateSource.DeliverymanId;
                order.Deliveryman = deliveryman;
            }
            void UpdateTable()
            {
                var oldTableValue = order.Table;
                if (updateSource.TableId == null)
                {
                    order.TableId = null;
                    order.Table = null;
                }
                else
                {
                    var table = StateManager.GetById<Table>(updateSource.TableId.Value);
                    order.TableId = updateSource.TableId;
                    order.Table = table;
                }
                oldTableValue?.OrderViewSource?.View.Refresh();
                order.Table?.OrderViewSource?.View.Refresh();
            }


        }

        private static void HandleAddedItems(Order order, Order updateSource)
        {
            foreach (var item in updateSource?.OrderItems?.ToList())
            {
                if (!order.OrderItems.Any(oi => oi.Id == item.Id))
                {
                    order.OrderItems.Add(item);
                }
            }
        }

        private static void HandleRemovedOrModifiedItems(Order order, Order updateSource)
        {
            foreach (var item in order.OrderItems.ToList())
            {
                var other = updateSource.OrderItems.FirstOrDefault(oi => item.Id == oi.Id);
                if (other != null&& other.State != OrderItemState.Removed)
                {
                    item.Quantity = other.Quantity;
                }

                if (item.State is not  OrderItemState.Removed &&  other.State is OrderItemState.Removed)
                {
                    order.OrderItems.Remove(item);
                }

            }
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
                oit.Order = order;
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
