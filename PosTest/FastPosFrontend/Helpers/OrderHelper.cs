using Caliburn.Micro;
using ServiceInterface.ExtentionsMethod;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Mutation.Observers;

namespace FastPosFrontend.Helpers
{
    public class OrderHelper
    {
        public static Order GetOrderChanges(IMutationObserver o)
        {
            if (o is ObjectGraphMutationObserver<Order> og)
            {
                var orderItemCO = og[nameof(Order.OrderItems)] as CollectionMutationObserver<OrderItem>;
                var added = orderItemCO.GetAddedItems().ToList();
                var removed = orderItemCO.GetRemovedItems().ToList();
                removed.ForEach(i => i.State = OrderItemState.Removed);

                //og.Source.OrderItems.AddRange(removed);
                var mutated = orderItemCO.GetMutatedItems(t =>
                {

                    var src = t.Source;

                    var item = new OrderItem(src);
                    var propertyMutation = (t as IObjectMutationObserver).GetPropertyMutation(nameof(OrderItem.Quantity));
                    var initial = (float)propertyMutation.Initial;
                    var committed = (float)propertyMutation.Committed;
                    if (committed > initial)
                    {
                        src.State = OrderItemState.IncreasedQuantity;
                    }
                    else if (committed < initial)
                    {
                        src.State = OrderItemState.DecreasedQuantity;
                    }
                    item.State = src.State;
                    item.Quantity = committed - initial;
                    return item;

                });
                var newOrder = og.Source.Clone();
                newOrder.OrderItems = new BindableCollection<OrderItem>();
                newOrder.OrderItems.AddRange(added);
                newOrder.OrderItems.AddRange(removed);
                newOrder.OrderItems.AddRange(mutated);
                return newOrder;
            }
            return null;
        }

        public static bool IgnoreRemovedOrderItem()
        {

        }
    }
}
