using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PosTest.Helpers;
using ServiceInterface.Model;
using Xunit;

namespace XUnitTesting.HelpersTesting
{
    public class OrderManagementHelperTest
    {
        public OrderManagementHelperTest()
        {
        }

        [Fact]
        public void StampAndSetOrderState_OrderItemsEqualsNull_ThrowsNullReferenceException()
        {
            DateTime timeStamp = DateTime.Now;
            IEnumerable<OrderItem> orderItems = null;
            Dictionary<int, OrderItem> diff = new Dictionary<int, OrderItem>();

            Action act = () => OrderManagementHelper.StampAndSetOrderState(timeStamp, orderItems, diff);

            var e = Assert.Throws<NullReferenceException>(act);
            Assert.Equal("OrderItems must not be null", e.Message);
        }

        [Fact]
        public void StampAndSetOrderState_DiffEqualsNull_ThrowsNullReferenceException()
        {
            DateTime timeStamp = DateTime.Now;
            IEnumerable<OrderItem> orderItems = MockingHelpers.GetOrderItems();
            Dictionary<int, OrderItem> diff = null;

            Action act = () => OrderManagementHelper.StampAndSetOrderState(timeStamp, orderItems, diff);

            var e = Assert.Throws<NullReferenceException>(act);
            Assert.Equal("Diff must not be null", e.Message);
        }


        [Fact]
        public void StampAndSetOrderState_DiffDoesNotReferenceItemsFromOrderItems_OrderItemsUnchanged()
        {
            DateTime timeStamp = DateTime.Now;
            IEnumerable<OrderItem> orderItems = MockingHelpers.GetOrderItems();
            Dictionary<int, OrderItem> diff = new Dictionary<int, OrderItem>();

            Action act = () => OrderManagementHelper.StampAndSetOrderState(timeStamp, orderItems, diff);


            Assert.All(orderItems, item => Assert.NotEqual(timeStamp, item.TimeStamp));
        }

        [Fact]
        public void StampAndSetOrderState_NotAllItemsChanged_UnchangedItemsUnstampedWithTimeStampArg()
        {
            DateTime timeStamp = DateTime.Now;
            List<OrderItem> orderItems = MockingHelpers.GetOrderItems().ToList(); // length 7
            Dictionary<int, OrderItem> diff = new Dictionary<int, OrderItem>();
            MockingHelpers.ModifySomeItems(orderItems, diff);

            //act 
            OrderManagementHelper.StampAndSetOrderState(timeStamp, orderItems, diff);

            //Assert 
            var unchangedItems = orderItems.Where(i => !diff.ContainsKey(i.GetHashCode()));
            Assert.All(unchangedItems, item => Assert.NotEqual(timeStamp, item.TimeStamp));
        }

        [Fact]
        public void StampAndSetOrderState_SomeItemsChanged_changedItemsStampedWithTimeStampArg()
        {
            DateTime timeStamp = DateTime.Now;
            List<OrderItem> orderItems = MockingHelpers.GetOrderItems().ToList(); // length 7
            Dictionary<int, OrderItem> diff = new Dictionary<int, OrderItem>();
            MockingHelpers.ModifySomeItems(orderItems, diff);

            //act 
            OrderManagementHelper.StampAndSetOrderState(timeStamp, orderItems, diff);

            //Assert 
            var changedItems = orderItems.Where(i => diff.ContainsKey(i.GetHashCode()));
            Assert.All(changedItems, item => Assert.Equal(timeStamp, item.TimeStamp));
        }

        [Fact]
        public void StampAndSetOrderState_ItemsWithIncreasedQuantity_StateSetToIncrementedQuantity()
        {
            DateTime timeStamp = DateTime.Now;
            List<OrderItem> orderItems = MockingHelpers.GetOrderItems().ToList(); // length 7
            Dictionary<int, OrderItem> diff = new Dictionary<int, OrderItem>();
            MockingHelpers.ModifySomeItems(orderItems, diff);
            //act 
            OrderManagementHelper.StampAndSetOrderState(timeStamp, orderItems, diff);

            //Assert 
            var itemsWithIncreasedQuantity = orderItems.Where(oi => diff.ContainsKey(oi.GetHashCode())
                                                                    &&
                                                                    oi.Quantity > diff.First(d =>
                                                                        d.Value.Product.Id == oi.Product.Id &&
                                                                        d.Key == oi.GetHashCode()).Value.Quantity
            );
            Assert.All(itemsWithIncreasedQuantity, item => Assert.Equal(OrderItemState.IncrementedQuantity, item.State));
        }

        [Fact]
        public void StampAndSetOrderState_ItemsWithDecreasedQuantity_StateSetToDecrementedQuantity()
        {
            DateTime timeStamp = DateTime.Now;
            List<OrderItem> orderItems = MockingHelpers.GetOrderItems().ToList(); // length 7
            Dictionary<int, OrderItem> diff = new Dictionary<int, OrderItem>();
            MockingHelpers.ModifySomeItems(orderItems, diff);
            //act 
            OrderManagementHelper.StampAndSetOrderState(timeStamp, orderItems, diff);

            //Assert 
            var itemsWithIncreasedQuantity = orderItems.Where(oi => diff.ContainsKey(oi.GetHashCode())
                                                                    &&
                                                                    oi.Quantity < diff.First(d =>
                                                                        d.Value.Product.Id == oi.Product.Id &&
                                                                        d.Key == oi.GetHashCode()).Value.Quantity
            );
            Assert.All(itemsWithIncreasedQuantity, item => Assert.Equal(OrderItemState.DecrementedQuantity, item.State));
        }
    }
}