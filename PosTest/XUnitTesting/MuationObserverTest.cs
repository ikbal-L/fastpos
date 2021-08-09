using FastPosFrontend.Helpers;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTesting
{
    public class MuationObserverTest
    {
        [Fact]
        public void Test()
        {
            MutationObserver mutationObserver = new MutationObserver();
            var orderItem1 = new OrderItem() { Quantity = 5};
            var orderItem2 = new OrderItem() { Quantity = 4};
            var orderItem3 = new OrderItem() { Quantity = 3};

            var list = new List<OrderItem>() { orderItem1, orderItem2 };
            mutationObserver.ObserveCollection(1, list);
            list.Remove(orderItem1);
            list.Add(orderItem3);
            var check = mutationObserver.GetRemovedItems(1, list);
            var check2 = mutationObserver.GetAddedItems(1, list);
            Assert.True(check.Contains(orderItem1));
            Assert.True(check2.Contains(orderItem3));
            mutationObserver.Observe(orderItem1, nameof(OrderItem.Quantity));
            mutationObserver.InitCommit(orderItem1, nameof(OrderItem.Quantity));
            orderItem1.Quantity = 10;
            mutationObserver.Commit(orderItem1, nameof(OrderItem.Quantity),orderItem1.Quantity);
            var result = mutationObserver.IsMutated(orderItem1, nameof(OrderItem.Quantity));
            Assert.True(result);
            mutationObserver.Push(orderItem1);
            var result2 = mutationObserver.IsMutated(orderItem1, nameof(OrderItem.Quantity));
            Assert.False(result2);

        }
    }
}
