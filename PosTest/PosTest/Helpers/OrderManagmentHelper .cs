﻿using System;
using System.Collections.Generic;
using System.Linq;
using ServiceInterface.Model;

namespace PosTest.Helpers
{
    public class OrderManagementHelper
    {
        public static void StampAndSetOrderState(DateTime timeStamp, IEnumerable<OrderItem> orderItems,
            Dictionary<int, OrderItem> diff)
        {
            if (orderItems == null) throw new NullReferenceException("OrderItems must not be null");
            
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
        }

        public static void StampAdditives(DateTime timeStamp, IEnumerable<OrderItem> orderItems)
        {
            var items = orderItems;
            foreach (var item in items)
                item.Additives?.Where(a => a.TimeStamp == null || a.State == AdditiveState.Removed).ToList()
                    .ForEach(a => a.TimeStamp = timeStamp);
        }
    }
}