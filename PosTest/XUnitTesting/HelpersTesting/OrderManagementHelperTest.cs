using System;
using System.Collections;
using System.Collections.Generic;
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
            Dictionary<int,OrderItem> diff = new Dictionary<int, OrderItem>();
           
            Action act = () => OrderManagementHelper.StampAndSetOrderState(timeStamp, orderItems, diff);

            var e = Assert.Throws<NullReferenceException>(act); 
            Assert.Equal("OrderItems must not be null",e.Message);
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





    }
}