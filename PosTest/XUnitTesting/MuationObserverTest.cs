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
            
            var orderItem1 = new OrderItem() { Quantity = 5};
            var orderItem2 = new OrderItem() { Quantity = 4};
            var orderItem3 = new OrderItem() { Quantity = 3};
            var orderItem4 = new OrderItem() { Quantity = 7};


            var list = new List<OrderItem>() { orderItem1, orderItem2 ,orderItem4};
            var collectionMutationObserver = new CollectionMutationObserver<OrderItem>(list,true,nameof(OrderItem.Quantity));
            list.Remove(orderItem1);
            list.Add(orderItem3);
            orderItem4.Quantity = 10;
            collectionMutationObserver.Commit(list);

            Assert.True(collectionMutationObserver.IsMutated());
            Assert.Contains(CollectionMutationType.ItemsAdded, collectionMutationObserver.GetMutationTypes());
            Assert.Contains(CollectionMutationType.ItemsRemoved, collectionMutationObserver.GetMutationTypes());
            Assert.Contains(CollectionMutationType.ItemsMutated, collectionMutationObserver.GetMutationTypes());


        }

        [Fact]
        public void TestOrder()
        {

         

            var order = new Order();
            order.AddOrderItem(new Product(),false);
            order.AddOrderItem(new Product(),false);
            order.AddOrderItem(new Product(),false);
            var collectionMutationObserver = new CollectionMutationObserver<OrderItem>(order.OrderItems, true, nameof(OrderItem.Quantity));
            order.AddOrderItem(new Product(), false);
            order.OrderItems.RemoveAt(0);
            order.OrderItems[1].Quantity = 20;


            Assert.False(collectionMutationObserver.IsMutated());
            Assert.DoesNotContain(CollectionMutationType.ItemsAdded, collectionMutationObserver.GetMutationTypes());
            Assert.DoesNotContain(CollectionMutationType.ItemsRemoved, collectionMutationObserver.GetMutationTypes());
            Assert.DoesNotContain(CollectionMutationType.ItemsMutated, collectionMutationObserver.GetMutationTypes());

            collectionMutationObserver.Commit(order.OrderItems);

            Assert.True(collectionMutationObserver.IsMutated());
            Assert.Contains(CollectionMutationType.ItemsAdded, collectionMutationObserver.GetMutationTypes());
            Assert.Contains(CollectionMutationType.ItemsRemoved, collectionMutationObserver.GetMutationTypes());
            Assert.Contains(CollectionMutationType.ItemsMutated, collectionMutationObserver.GetMutationTypes());

            collectionMutationObserver.Push();

            Assert.False(collectionMutationObserver.IsMutated());
            Assert.DoesNotContain(CollectionMutationType.ItemsAdded, collectionMutationObserver.GetMutationTypes());
            Assert.DoesNotContain(CollectionMutationType.ItemsRemoved, collectionMutationObserver.GetMutationTypes());
            Assert.DoesNotContain(CollectionMutationType.ItemsMutated, collectionMutationObserver.GetMutationTypes());


            order.OrderItems.Last().Quantity = 50;
            Assert.True(collectionMutationObserver.IsMutated());
            Assert.Contains(CollectionMutationType.ItemsMutated, collectionMutationObserver.GetMutationTypes());
            Assert.DoesNotContain(CollectionMutationType.ItemsAdded, collectionMutationObserver.GetMutationTypes());
            Assert.DoesNotContain(CollectionMutationType.ItemsRemoved, collectionMutationObserver.GetMutationTypes());


        }
    }
}
