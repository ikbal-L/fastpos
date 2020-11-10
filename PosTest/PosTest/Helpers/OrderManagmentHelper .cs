using System;
using System.Collections.Generic;
using System.Linq;
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
                var refItem = diff.First(d => d.Value.Product.Id == i.Product.Id && d.Key == i.GetHashCode()).Value;
                if (i.TimeStamp != null)
                {
                    if (i.Quantity > refItem.Quantity)
                    {
                        i.State = OrderItemState.IncrementedQuantity;
                    }
                    else if (i.Quantity < refItem.Quantity)
                    {
                        i.State = OrderItemState.DecrementedQuantity;
                    }
                }

                i.TimeStamp = timeStamp;
            });
            
            return true;
        }

        public static bool StampAdditives(DateTime timeStamp, IEnumerable<OrderItem> orderItems)
        {
            var items = orderItems;
            var changedAdditives = new List<Additive>();
                
            foreach (var item in items)
            {
                changedAdditives =changedAdditives.Concat(item.Additives?.Where(a => a.TimeStamp == null || a.State == AdditiveState.Removed) ?? Array.Empty<Additive>()).ToList();
            }

            if (changedAdditives.Count ==0) return false;
            changedAdditives.ForEach(a => a.TimeStamp = timeStamp);
            return true;


        }

        public static void TrackItemForChange(OrderItem item, Dictionary<int, OrderItem> diff)
        {
            if (!diff.ContainsKey(item.GetHashCode()))
            {
                var value = new OrderItem(item.Product, item.Quantity, item.UnitPrice, item.Order)
                    {TimeStamp = item.TimeStamp};
                diff.Add(item.GetHashCode(), value);
            }
        }
    }
}