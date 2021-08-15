using Caliburn.Micro;
using ServiceInterface.ExtentionsMethod;
using ServiceInterface.Model;
using System;
using System.Linq;
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
                    //item.Quantity = committed - initial;
                    item.Quantity = Math.Abs(committed - initial);
                    return item;

                });

                var newOrder = og.Source.Clone();

                newOrder.Customer = og.Source.Customer;
                newOrder.Waiter = og.Source.Waiter;
                newOrder.Deliveryman = og.Source.Deliveryman;
                newOrder.Table = og.Source.Table;

                newOrder.OrderItems = new BindableCollection<OrderItem>();
                newOrder.OrderItems.AddRange(added);
                newOrder.OrderItems.AddRange(removed);
                newOrder.OrderItems.AddRange(mutated);
                return newOrder;
            }
            return null;
        }

    }
}
