using FastPosFrontend.Helpers;
using FastPosFrontend.ViewModels;
using ServiceInterface.Model;
using System;
using Xunit;

namespace XUnitTesting.CheckpointTesting.SplitTesting
{
    public class CommandFunctionsTesting
    {
        [Fact]
        public void AddOrderItems_CheckTotals()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            var p = new Product { Id = 1, Description = "desc abc", Name = "name", Price = 150 };
            var p2 = new Product() { Id = 2, Description = "desc2", Name = "awescome", Price = 100 };

            //Act
            //checkoutVM.AddOrderItem(p);
            //checkoutVM.AddOrderItem(p2);
            //checkoutVM.ActionKeyboard(ActionButton.Split);
            SplitViewModel splitVm = new SplitViewModel(null);
            splitVm.SplittedOrder.AddOrderItem(product: p,  setSelected: true, quantity: 1);
            splitVm.SplittedOrder.AddOrderItem(product: p2,  setSelected: true, quantity: 1);

            //Assert
            Assert.Equal(2, splitVm.SplittedOrder.OrderItems.Count);
            Assert.Equal(250, splitVm.SplittedOrder.Total);
            Assert.Equal(250, splitVm.SplittedOrder.NewTotal);
            
        } 
        
        [Fact]
        public void AddOrderItems_WhenTotalPriceButtonIsChecked_CheckTotals()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            var p = new Product { Id = 1, Description = "desc abc", Name = "name", Price = 150 };
            var p2 = new Product() { Id = 2, Description = "desc2", Name = "awescome", Price = 100 };

            //Act
            //checkoutVM.AddOrderItem(p);
            //checkoutVM.AddOrderItem(p2);
            //checkoutVM.ActionKeyboard(ActionButton.Split);
            SplitViewModel splitVm = new SplitViewModel(null);
            splitVm.SplittedOrder.AddOrderItem(product: p,  setSelected: true, quantity: 1);
            splitVm.SplittedOrder.AddOrderItem(product: p2,  setSelected: true, quantity: 1);
            splitVm.IsTotalPriceChecked = true;
            splitVm.NumericZone = "1";

            //Assert
            Assert.Equal(1, splitVm.SplittedOrder.NewTotal);

            //Act
            splitVm.NumericZone += "2";

            //Assert
            Assert.Equal(12, splitVm.SplittedOrder.NewTotal);

            //Act
            splitVm.NumericZone += "3";

            //Assert
            Assert.Equal(123, splitVm.SplittedOrder.NewTotal);

            //Act
            Assert.Throws<Exception>(() => splitVm.NumericZone += "4");

            //Assert
            Assert.Equal(123, splitVm.SplittedOrder.NewTotal);

            //Act
            splitVm.NumericZone = "";

            //Assert
            Assert.Equal(0, splitVm.SplittedOrder.NewTotal);
        }

        [Fact]
        public void AddOrderItems_WhenDiscButtonIsChecked_DiscountByAmount_CheckTotals()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            var p = new Product { Id = 1, Description = "desc abc", Name = "name", Price = 150 };
            var p2 = new Product() { Id = 2, Description = "desc2", Name = "awescome", Price = 100 };

            //Act
            //checkoutVM.AddOrderItem(p);
            //checkoutVM.AddOrderItem(p2);
            //checkoutVM.ActionKeyboard(ActionButton.Split);
            SplitViewModel splitVm = new SplitViewModel(null);
            splitVm.SplittedOrder.AddOrderItem(product: p, setSelected: true, quantity: 1);
            splitVm.SplittedOrder.AddOrderItem(product: p2,  setSelected: true, quantity: 1);
            splitVm.IsDiscChecked = true;
            splitVm.NumericZone = "1";

            //Assert
            Assert.Equal(249, splitVm.SplittedOrder.NewTotal);

            //Act
            splitVm.NumericZone += "2";

            //Assert
            Assert.Equal(238, splitVm.SplittedOrder.NewTotal);

            //Act
            splitVm.NumericZone += "5";

            //Assert
            Assert.Equal(125, splitVm.SplittedOrder.NewTotal);

            //Act
            //splitVm.NumericZone += "4";

            //Assert
            Assert.Throws<Exception>(() => splitVm.NumericZone += "4");
            Assert.Equal("125", splitVm.NumericZone);
            Assert.Equal(125, splitVm.SplittedOrder.NewTotal);
        }

        [Fact]
        public void AddOrderItems_WhenDiscButtonIsChecked_DiscountByPercentage_CheckTotals()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            var p = new Product { Id = 1, Description = "desc abc", Name = "name", Price = 150 };
            var p2 = new Product() { Id = 2, Description = "desc2", Name = "awescome", Price = 100 };

            //Act
            //checkoutVM.AddOrderItem(p);
            //checkoutVM.AddOrderItem(p2);
            //checkoutVM.ActionKeyboard(ActionButton.Split);
            SplitViewModel splitVm = new SplitViewModel(null);
            splitVm.SplittedOrder.AddOrderItem(product: p,  setSelected: true, quantity: 1);
            splitVm.SplittedOrder.AddOrderItem(product: p2,  setSelected: true, quantity: 1);
            splitVm.IsDiscChecked = true;
            splitVm.NumericZone += "1";
            splitVm.NumericZone += "2";
            splitVm.NumericZone += "%";

            //Assert
            Assert.Equal(12, splitVm.SplittedOrder.DiscountPercentage);
            Assert.Equal(splitVm.SplittedOrder.Total*(1m-12m/100), splitVm.SplittedOrder.NewTotal);
            Assert.Equal(splitVm.SplittedOrder.Total- splitVm.SplittedOrder.TotalDiscountAmount, splitVm.SplittedOrder.NewTotal);

            //Act
            splitVm.NumericZone = splitVm.NumericZone.Remove(2, 1);

            //Assert
            Assert.Equal(12, splitVm.SplittedOrder.DiscountAmount);
            Assert.Equal(splitVm.SplittedOrder.Total-12m, splitVm.SplittedOrder.NewTotal);
            Assert.Equal(splitVm.SplittedOrder.Total- splitVm.SplittedOrder.TotalDiscountAmount, splitVm.SplittedOrder.NewTotal);

        }

    }
}
