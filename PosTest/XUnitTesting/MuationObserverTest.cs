using Caliburn.Micro;
using FastPosFrontend.Helpers;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Attributes;
using Utilities.Mutation.Observers;
using Xunit;

namespace XUnitTesting
{
    public class MuationObserverTest
    {
        [Fact]
        public void MutationObservers_CreateAGraphMutationObserverWithTypeNotDecoratedByObserveMutationsAttribute_ThrowArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => {

                var o = new DeepMutationObserver<Additive>(new Additive());
            });
            Assert.Contains($"contains no properties decorated by {nameof(ObserveMutationsAttribute)}", ex.Message);
        }

        [Fact]
        public void MutationObservers_CallCommitWhenObserverIsNotInitialized_ThrowsInvalidOperationException()
        {
            
            var orderItemObserver = new ShallowMutationObserver<OrderItem>(null,nameof(OrderItem.Quantity));
            var collectionObserver =new CollectionMutationObserver<OrderItem>(null);
            var graphObserver =new CollectionMutationObserver<OrderItem>(null);
            Assert.Throws<InvalidOperationException>(orderItemObserver.Commit);
            Assert.Throws<InvalidOperationException>(collectionObserver.Commit);
            Assert.Throws<InvalidOperationException>(graphObserver.Commit);

        }

        [Fact]
        public void MutationObservers_CallPushWhenObserverIsNotInitialized_ThrowsInvalidOperationException()
        {

            var orderItemObserver = new ShallowMutationObserver<OrderItem>(null, nameof(OrderItem.Quantity));
            var collectionObserver = new CollectionMutationObserver<OrderItem>(null);
            var graphObserver = new CollectionMutationObserver<OrderItem>(null);
            Assert.Throws<InvalidOperationException>(orderItemObserver.Push);
            Assert.Throws<InvalidOperationException>(collectionObserver.Push);
            Assert.Throws<InvalidOperationException>(graphObserver.Push);

        }

        public void Test()
        {
            
            var orderItem1 = new OrderItem() { Quantity = 5};
            var orderItem2 = new OrderItem() { Quantity = 4};
            var orderItem3 = new OrderItem() { Quantity = 3};
            var orderItem4 = new OrderItem() { Quantity = 7};


            var list = new List<OrderItem>() { orderItem1, orderItem2 ,orderItem4};
            var collectionMutationObserver = new CollectionMutationObserver<OrderItem>(list,isObservingItems:true,properties: nameof(OrderItem.Quantity));
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
            var collectionMutationObserver = new CollectionMutationObserver<OrderItem>(order.OrderItems,true) ;
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

            collectionMutationObserver.Commit();
            Assert.True(collectionMutationObserver.IsMutated());
            Assert.Contains(CollectionMutationType.ItemsMutated, collectionMutationObserver.GetMutationTypes());
            Assert.DoesNotContain(CollectionMutationType.ItemsAdded, collectionMutationObserver.GetMutationTypes());
            Assert.DoesNotContain(CollectionMutationType.ItemsRemoved, collectionMutationObserver.GetMutationTypes());


        }

        [Fact]
        public void TestItem()
        {

            
            var orderItem1 = new OrderItem() { Quantity = 5,Product = new Product() { Name ="p1"} };

            
            var prop = nameof(OrderItem.Quantity);
            var prop2 = nameof(Product.Name);
            var observer = new ShallowMutationObserver<OrderItem>(orderItem1,prop);
            orderItem1.Additives = new System.Collections.ObjectModel.ObservableCollection<Additive>();
            
            var observer2 = new ShallowMutationObserver<Product>(orderItem1.Product,prop2);
            orderItem1.Quantity = 15;
            orderItem1.Product.Name = "p14";
            observer.Commit();
            observer2.Commit();
            Assert.True(observer.IsMutated());
            Assert.True(observer2.IsMutated());

            //var propertyDiff = new PropertyDiff<float>((f1,f2)=> f2-f1);
            //var diffItem = DiffGenerator<OrderItem>.Generate(observer,(prop,propertyDiff));
            //Assert.Equal(10, diffItem.Quantity);
            
        }

        [Fact]
        public void  Test3()
        {
            
           
            var orderItem = new OrderItem() { Additives = new System.Collections.ObjectModel.ObservableCollection<Additive>()};
            orderItem.Additives.Add(new Additive() { Description="d1"});
            orderItem.Additives.Add(new Additive() { Description="d2"});

            var graphObserver = new DeepMutationObserver<OrderItem>(orderItem);
            orderItem.Product = new Product() { Name = "P1" };
            orderItem.Additives.RemoveAt(0);
            var productPropertyName = nameof(OrderItem.Product);
            var additivesPropertyName = nameof(OrderItem.Additives);
            graphObserver.LateInit<Product>(productPropertyName);
            
            orderItem.Product.Name = "P1";

            graphObserver.Commit();

            var mutationObserver1 = graphObserver[productPropertyName] ;
            var mutationObserver2 = graphObserver[additivesPropertyName] as CollectionMutationObserver<Additive>;

            Assert.True(graphObserver.IsMutated());
            Assert.False(mutationObserver1.IsMutated());
            Assert.True(mutationObserver2.IsMutated());
            Assert.Contains(CollectionMutationType.ItemsRemoved,mutationObserver2.GetMutationTypes());

        }

        [Fact]
        public void Test4()
        {
            var order = new Order();
            var orderItems = MockingHelpers.GetOrderItems();
            order.OrderItems = new BindableCollection<OrderItem>(orderItems);
            var CollectionObserver = new CollectionMutationObserver<OrderItem>(order.OrderItems,true);

        }


    }
   
    
}
