using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using ServiceInterface.Model;

namespace PosTest.Helpers
{
    public class OrderManagementHelper
    {
        public static bool StampAndSetOrderItemState(DateTime timeStamp, IEnumerable<OrderItem> orderItems,
            Dictionary<int, OrderItem> diff)
        {
            if (orderItems == null) throw new NullReferenceException("OrderItems must not be null");
            if (diff == null) throw new NullReferenceException("Diff must not be null");
            if (diff.Count == 0) return false;

            var items = orderItems
                .Where(i => diff.ContainsKey(i.GetHashCode())).ToList();

            items.ForEach(i =>
            {
                var refItem = diff.First(d => d.Value.ProductId == i.ProductId && d.Key == i.GetHashCode()).Value;
                if (i.TimeStamp != null)
                {
                    if (i.Quantity > refItem.Quantity)
                    {
                        i.State = OrderItemState.IncreasedQuantity;
                    }
                    else if (i.Quantity < refItem.Quantity)
                    {
                        i.State = OrderItemState.DecreasedQuantity;
                    }
                }

                i.TimeStamp = timeStamp;
            });
            
            return true;
        }

        public static bool StampAdditives(DateTime timeStamp, IEnumerable<OrderItem> orderItems)
        {
            var items = orderItems;
            var changedAdditives = new List<OrderItemAdditive>();
                
            foreach (var item in items)
            {
                changedAdditives =changedAdditives.Concat(item.OrderItemAdditives?.Where(a => a.Timestamp == null || a.State == AdditiveState.Removed) ?? Array.Empty<OrderItemAdditive>()).ToList();
            }

            if (changedAdditives.Count ==0) return false;
            changedAdditives.ForEach(a => a.Timestamp = timeStamp);
            return true;


        }

        public static void TrackItemForChange(OrderItem item, Dictionary<int, OrderItem> diff)
        {
            if (!diff.ContainsKey(item.GetHashCode()))
            {
                var value = new OrderItem(item.Product, item.Quantity, item.UnitPrice, item.Order)
                    {TimeStamp = item.TimeStamp,ProductName = item.ProductName,ProductId = item.ProductId};
                diff.Add(item.GetHashCode(), value);
            }
        }

        public static Order GetChangesFromOrder(Order order,Dictionary<int,OrderItem> diff)
        {
            DateTime? recent = order.OrderItems.Max(oi => oi.TimeStamp);
            var changedOrderItems = new List<OrderItem>();
            var orderItems = order.OrderItems
                .Where(oi =>
                    (oi.State == OrderItemState.IncreasedQuantity || oi.State == OrderItemState.DecreasedQuantity)&& oi.TimeStamp == recent);
            if (orderItems != null)
            {
                foreach (var orderItem in orderItems)
                {
                    var refItem = diff.First(d => d.Key == orderItem.GetHashCode()).Value;
                    var item = new OrderItem(orderItem.Product, orderItem.Quantity - refItem.Quantity, orderItem.UnitPrice, orderItem.Order){State = orderItem.State};
                    changedOrderItems.Add(item);
                } 
            }

            var addedOrRemovedItems = order.OrderItems.Where(oi =>
                oi.TimeStamp == recent && (oi.State == OrderItemState.Removed || oi.State == OrderItemState.Added));
            changedOrderItems = changedOrderItems.Concat(addedOrRemovedItems
            ).ToList();
            return  new Order(){OrderItems = new BindableCollection<OrderItem>(changedOrderItems)};
        }
    }
}