using Caliburn.Micro;
using PosTest.ViewModels;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTesting.CheckpointTesting
{
    public class CheckpointBasicMethodTests
    {
        [Fact]
        public void AddOrderItem_AddNullProduct()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();

            //Act
            checkoutVM.AddOrderItem(null);

            //Assert
            Assert.Single(checkoutVM.Orders);
            Assert.Empty(checkoutVM.CurrentOrder.OrderItems);
        }

        [Fact]
        public void AddOrderItem_AddNotNullProduct()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            var p = new Product { Id=1, Description="desc abc", Name="name", Price=5};

            //Act
            checkoutVM.AddOrderItem(p);

            //Assert
            Assert.Single(checkoutVM.Orders);
            Assert.Single(checkoutVM.CurrentOrder.OrderItems);
        }

        [Fact]
        public void AddOrderItem_AddTwoSameProducts()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            var p = new Product { Id=1, Description="desc abc", Name="name", Price=5};

            //Act
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddOrderItem(p);

            //Assert
            Assert.Single(checkoutVM.Orders);
            Assert.Single(checkoutVM.CurrentOrder.OrderItems);
        }

        [Fact]
        public void AddOrderItem_AddPlatterAndProduct()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            var p = new Product { Id=1, Description="desc abc", Name="name", Price=8};
            var p2 = new Platter { Id=2, Description="desc2", Name="awescome", Price=6};

            //Act
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddOrderItem(p2);

            //Assert
            Assert.Single(checkoutVM.Orders);
            Assert.Equal(2, checkoutVM.CurrentOrder.OrderItems.Count);
        }

        [Fact]
        public void DeleteOrderItem_DeleteNullItem()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();

            //Act
            checkoutVM.RemoveOrerItem(null);


            //Assert
            Assert.Single(checkoutVM.Orders);
            Assert.Empty(checkoutVM.CurrentOrder.OrderItems);
        }

        [Fact]
        public void DeleteOrderItem_DeletNotExistsItem()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            var p = new Platter { Id=2, Description="desc2", Name="awescome", Price=6};
            var orderItem = new OrderItem(p, 5, 120, new Order());

            //Act
            checkoutVM.RemoveOrerItem(orderItem);

            //Assert
            Assert.Single(checkoutVM.Orders);
            Assert.Empty(checkoutVM.CurrentOrder.OrderItems);
        }

        [Fact]
        public void DeleteOrderItem_DeleteExistsItem()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            var p = new Platter { Id=2, Description="desc2", Name="awescome", Price=160};
            checkoutVM.AddOrderItem(p);

            //Act
            Assert.Single(checkoutVM.CurrentOrder.OrderItems);
            checkoutVM.RemoveOrerItem(checkoutVM.CurrentOrder.OrderItems[0]);

            //Assert
            Assert.Single(checkoutVM.Orders);
            Assert.Empty(checkoutVM.CurrentOrder.OrderItems);
        }

        [Fact]
        public void AddOrderItem_AddTwoSamePlatters_BothHaveAdditives()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            var additives = new BindableCollection<Additive>
            {
                new Additive{Id=1, Description="abc"},
                new Additive{Id=2, Description="abc"},
            };
            var p = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = 6, Additives=additives };

            //Act
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddOrderItem(p);

            //Assert
            Assert.Single(checkoutVM.Orders);
            Assert.Equal(2, checkoutVM.CurrentOrder.OrderItems.Count);
        }

        [Fact]
        public void AddOrderItem_AddTwoSamePlatters_BothDontHaveAdditives()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();

            var p = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = 6, Additives=null };

            //Act
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddOrderItem(p);

            //Assert
            Assert.Single(checkoutVM.Orders);
            Assert.Single(checkoutVM.CurrentOrder.OrderItems);
        }

        [Fact]
        public void AddAdditices_AddNull()
        {
            var checkoutVM = new CheckoutViewModel();
            var additives = new BindableCollection<Additive>
            {
                new Additive{Id=1, Description="add1"},
                new Additive{Id=2, Description="add2"},
                new Additive{Id=3, Description="add3"},
                new Additive{Id=4, Description="add4"},
            };

            var p = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = 6, Additives = additives };

            //Act
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddAditive(null);

            //Assert
            Assert.Null(checkoutVM.CurrentOrder.SelectedOrderItem.Additives);
        }

        [Fact]
        public void AddAdditices_AddTheSameAdditiveTwice()
        {
            var checkoutVM = new CheckoutViewModel();
            var additives = new BindableCollection<Additive>
            {
                new Additive{Id=1, Description="add1"},
                new Additive{Id=2, Description="add2"},
                new Additive{Id=3, Description="add3"},
                new Additive{Id=4, Description="add4"},
            };

            var p = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = 6, Additives = additives };

            //Act
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddAditive(additives[0]);

            //Assert
            Assert.Single(checkoutVM.CurrentOrder.SelectedOrderItem.Additives);
            
            //Act
            checkoutVM.AddAditive(additives[0]);
            checkoutVM.AddAditive(additives[0]);

            //Assert
            Assert.Single(checkoutVM.CurrentOrder.SelectedOrderItem.Additives);
            Assert.Equal(additives[0], checkoutVM.CurrentOrder.SelectedOrderItem.SelectedAdditive);
        }

        [Fact]
        public void RemoveAdditives_()
        {
            var checkoutVM = new CheckoutViewModel();
            var additives = new BindableCollection<Additive>
            {
                new Additive{Id=1, Description="add1"},
                new Additive{Id=2, Description="add2"},
                new Additive{Id=3, Description="add3"},
                new Additive{Id=4, Description="add4"},
            };

            var p = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = 6, Additives = additives };

            //Act
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddAditive(additives[0]);
            checkoutVM.AddAditive(additives[1]);
            checkoutVM.AddAditive(additives[2]);
            checkoutVM.CurrentOrder.SelectedOrderItem.SelectedAdditive = additives[1];

            //Assert
            Assert.Equal(3, checkoutVM.CurrentOrder.SelectedOrderItem.Additives.Count);

            //Act
            var addtv = checkoutVM.CurrentOrder.SelectedOrderItem.Additives[1];
            checkoutVM.RemoveAdditive(addtv);

            //Assert
            Assert.Equal(2, checkoutVM.CurrentOrder.SelectedOrderItem.Additives.Count);
            Assert.Null(checkoutVM.CurrentOrder.SelectedOrderItem.Additives.
                                        Where(addv => addv.Equals(addtv)).
                                        FirstOrDefault());

            //Act 
            //remove removed one
            checkoutVM.RemoveAdditive(addtv);
            
            //Assert nothing would happen

            //Act
            //remove null
            checkoutVM.RemoveAdditive(null);
            
            //Assert nothing would happen

            //Act
            //remove not existing additive
            checkoutVM.RemoveAdditive(new Additive());
            
            //Assert nothing would happen
        }

        [Fact]
        public void AddOneToQuantity_CheckTotal()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            var p = new Platter { Id = 1, Description = "desc2", Name = "awescome", Price = 160 };
            var p2 = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = 100 };
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddOrderItem(p2);

            //Act
            Assert.Equal(160, checkoutVM.CurrentOrder.OrderItems[0].Total);
            Assert.Equal(100+160, checkoutVM.CurrentOrder.Total);
            checkoutVM.AddOneToQuantity(checkoutVM.CurrentOrder.OrderItems[0]);

            //Assert
            Assert.Single(checkoutVM.Orders);
            Assert.Equal(2, checkoutVM.CurrentOrder.OrderItems.Count);
            Assert.Equal(100+160*2, checkoutVM.CurrentOrder.Total);
        }

        [Fact]
        public void SubtractOneFromQuantity_QuantityEqualsOne()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            var p = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = 160 };
            checkoutVM.AddOrderItem(p);

            //Act
            Assert.Single(checkoutVM.CurrentOrder.OrderItems);
            Assert.Equal(160, checkoutVM.CurrentOrder.OrderItems[0].Total);
            Assert.Equal(160, checkoutVM.CurrentOrder.Total);
            checkoutVM.SubtractOneFromQuantity(checkoutVM.CurrentOrder.OrderItems[0]);

            //Assert
            Assert.Single(checkoutVM.Orders);
            Assert.Single(checkoutVM.CurrentOrder.OrderItems);
            Assert.Equal(1, checkoutVM.CurrentOrder.OrderItems[0].Quantity);
        }

        [Fact]
        public void SubtractOneFromQuantity_CheckTotal()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            var p = new Platter { Id = 1, Description = "desc2", Name = "awescome", Price = 160 };
            var p2 = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = 100 };
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddOrderItem(p2);

            //Act
            Assert.Equal(160, checkoutVM.CurrentOrder.OrderItems[0].Total);
            Assert.Equal(160+100, checkoutVM.CurrentOrder.Total);
            checkoutVM.AddOneToQuantity(checkoutVM.CurrentOrder.OrderItems[0]);
            Assert.Equal(160*2+100, checkoutVM.CurrentOrder.Total);
            checkoutVM.SubtractOneFromQuantity(checkoutVM.CurrentOrder.OrderItems[0]);

            //Assert
            Assert.Single(checkoutVM.Orders);
            Assert.Equal(2, checkoutVM.CurrentOrder.OrderItems.Count);
            Assert.Equal(160+100, checkoutVM.CurrentOrder.Total);
        }

        [Fact]
        public void DiscountOnOrderItem_AmountDiscount_DiscountValueEmpty()
        {
            var checkoutVM = new CheckoutViewModel();
            var p = new Platter { Id = 1, Description = "desc2", Name = "awescome", Price = 160 };
            checkoutVM.AddOrderItem(p);

            //Act
            Assert.Equal(160, checkoutVM.CurrentOrder.OrderItems[0].Total);
            Assert.Equal(160, checkoutVM.CurrentOrder.Total);
            Assert.Same(checkoutVM.CurrentOrder.SelectedOrderItem, checkoutVM.CurrentOrder.OrderItems[0]);
            checkoutVM.NumericZone = String.Empty;
            checkoutVM.DiscountOnOrderItem(checkoutVM.CurrentOrder.SelectedOrderItem);

            //Assert
            Assert.Empty(checkoutVM.NumericZone);
            Assert.Single(checkoutVM.Orders);
            Assert.Single(checkoutVM.CurrentOrder.OrderItems);
            Assert.Equal(160, checkoutVM.CurrentOrder.Total);
            Assert.Equal(160, checkoutVM.CurrentOrder.NewTotal);
            Assert.Equal(0, checkoutVM.CurrentOrder.SelectedOrderItem.DiscountAmount);
        }

        [Fact]
        public void DiscountOnOrderItem_AmountDiscount_DiscountValueGreaterThanOrderItemTotal()
        {
            var checkoutVM = new CheckoutViewModel();
            var p = new Platter { Id = 1, Description = "desc2", Name = "awescome", Price = 160 };
            checkoutVM.AddOrderItem(p);

            //Act
            Assert.Equal(160, checkoutVM.CurrentOrder.OrderItems[0].Total);
            Assert.Equal(160, checkoutVM.CurrentOrder.Total);
            Assert.Same(checkoutVM.CurrentOrder.SelectedOrderItem, checkoutVM.CurrentOrder.OrderItems[0]);
            checkoutVM.NumericZone = "161";

            //Assert
            Assert.Throws<Exception>(() => checkoutVM.DiscountOnOrderItem(checkoutVM.CurrentOrder.SelectedOrderItem));
            Assert.Empty(checkoutVM.NumericZone);
            Assert.Equal(0, checkoutVM.CurrentOrder.SelectedOrderItem.DiscountAmount);
        }

        [Fact]
        public void DiscountOnOrderItem_AmountDiscount_DiscountValueNotNumericString()
        {
            var checkoutVM = new CheckoutViewModel();
            var p = new Platter { Id = 1, Description = "desc2", Name = "awescome", Price = 160 };
            checkoutVM.AddOrderItem(p);

            //Act
            Assert.Equal(160, checkoutVM.CurrentOrder.OrderItems[0].Total);
            Assert.Equal(160, checkoutVM.CurrentOrder.Total);
            Assert.Same(checkoutVM.CurrentOrder.SelectedOrderItem, checkoutVM.CurrentOrder.OrderItems[0]);
            checkoutVM.NumericZone = "aa161";

            //Assert
            Assert.Throws<FormatException>(() => checkoutVM.DiscountOnOrderItem(checkoutVM.CurrentOrder.SelectedOrderItem));
            Assert.Empty(checkoutVM.NumericZone);
            Assert.Equal(0, checkoutVM.CurrentOrder.SelectedOrderItem.DiscountAmount);
        }

        [Fact]
        public void DiscountOnOrderItem_AmountDiscount_ApplyDiscountTwoTimes()
        {
            var checkoutVM = new CheckoutViewModel();
            var p = new Platter { Id = 1, Description = "desc2", Name = "awescome", Price = 160 };
            checkoutVM.AddOrderItem(p);

            //Act
            Assert.Equal(160, checkoutVM.CurrentOrder.OrderItems[0].Total);
            Assert.Equal(160, checkoutVM.CurrentOrder.Total);
            Assert.Same(checkoutVM.CurrentOrder.SelectedOrderItem, checkoutVM.CurrentOrder.OrderItems[0]);
            checkoutVM.NumericZone = "50";
            checkoutVM.DiscountOnOrderItem(checkoutVM.CurrentOrder.SelectedOrderItem);
            checkoutVM.NumericZone = "20";
            checkoutVM.DiscountOnOrderItem(checkoutVM.CurrentOrder.SelectedOrderItem);

            //Assert
            Assert.Equal(160-20, checkoutVM.CurrentOrder.NewTotal);
            Assert.Equal(checkoutVM.CurrentOrder.Total - 20, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);
            Assert.Equal(20, checkoutVM.CurrentOrder.SelectedOrderItem.DiscountAmount);
        }

        [Fact]
        public void DiscountOnOrderItem_AmountDiscount_CheckNewTotal()
        {
            var checkoutVM = new CheckoutViewModel();
            decimal price1 = 100,
                price2 = 160;
            var p2 = new Platter { Id = 1, Description = "desc2", Name = "awescome", Price = price1 };
            var p = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = price2 };
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddOrderItem(p2);
            var discount = 20;

            //Act
            Assert.Equal(160, checkoutVM.CurrentOrder.OrderItems[0].Total);
            Assert.Equal(160+100, checkoutVM.CurrentOrder.Total);
            Assert.Equal(160+100, checkoutVM.CurrentOrder.NewTotal);
            
            checkoutVM.NumericZone = Convert.ToString(discount);
            checkoutVM.DiscountOnOrderItem(checkoutVM.CurrentOrder.SelectedOrderItem);

            //Assert
            Assert.Equal(160 + 100 - discount, checkoutVM.CurrentOrder.NewTotal);
            Assert.Equal(checkoutVM.CurrentOrder.Total - discount, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);
            Assert.Equal(20, checkoutVM.CurrentOrder.SelectedOrderItem.DiscountAmount);
        }

        [Fact]
        public void DiscountOnOrderItem_PercentageDiscount_CheckNewTotal()
        {
            var checkoutVM = new CheckoutViewModel();
            decimal price1 = 150,
                price2 = 160;
            var p2 = new Platter { Id = 1, Description = "desc2", Name = "awescome", Price = price1 };
            var p = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = price2 };
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddOrderItem(p2);
            var discountpercent = 20;

            //Act
            Assert.Equal(price1, checkoutVM.CurrentOrder.SelectedOrderItem.Total);
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(price1 + price2, checkoutVM.CurrentOrder.NewTotal);
            
            checkoutVM.NumericZone = Convert.ToString(discountpercent) +"%";
            checkoutVM.DiscountOnOrderItem(checkoutVM.CurrentOrder.SelectedOrderItem);

            //Assert
            Assert.Equal(price1 + price2 - price1*discountpercent/100, checkoutVM.CurrentOrder.NewTotal);
            Assert.Equal(checkoutVM.CurrentOrder.Total - price1 * discountpercent / 100, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);
            Assert.Equal(price1 * discountpercent / 100, checkoutVM.CurrentOrder.SelectedOrderItem.DiscountAmount);
        }

        [Fact]
        public void DiscountOnOrderItem_PercentageDiscount_ApplyDiscountTwice()
        {
            var checkoutVM = new CheckoutViewModel();
            decimal price1 = 150,
                price2 = 160;
            var p2 = new Platter { Id = 1, Description = "desc2", Name = "awescome", Price = price1 };
            var p = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = price2 };
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddOrderItem(p2);

            //Act
            Assert.Equal(price1, checkoutVM.CurrentOrder.SelectedOrderItem.Total);
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(price1 + price2, checkoutVM.CurrentOrder.NewTotal);

            var discountpercent = 25;
            checkoutVM.NumericZone = Convert.ToString(discountpercent) + "%";
            checkoutVM.DiscountOnOrderItem(checkoutVM.CurrentOrder.SelectedOrderItem);
            discountpercent = 15;
            checkoutVM.NumericZone = Convert.ToString(discountpercent) + "%";
            checkoutVM.DiscountOnOrderItem(checkoutVM.CurrentOrder.SelectedOrderItem);

            //Assert
            Assert.Equal(price1 + price2 - price1 * discountpercent / 100, checkoutVM.CurrentOrder.NewTotal);
            Assert.Equal(checkoutVM.CurrentOrder.Total - price1 * discountpercent / 100, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);
            Assert.Equal(price1 * discountpercent / 100, checkoutVM.CurrentOrder.SelectedOrderItem.DiscountAmount);
        }

        [Fact]
        public void DiscountOnOrderItem_PercentageDiscount_DiscountContainsOnlyPercentCharacter()
        {
            var checkoutVM = new CheckoutViewModel();
            decimal price1 = 150,
                price2 = 160;
            var p2 = new Platter { Id = 1, Description = "desc2", Name = "awescome", Price = price1 };
            var p = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = price2 };
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddOrderItem(p2);

            //Act
            Assert.Equal(price1, checkoutVM.CurrentOrder.SelectedOrderItem.Total);
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(price1 + price2, checkoutVM.CurrentOrder.NewTotal);
            
            checkoutVM.NumericZone = "%";
            checkoutVM.DiscountOnOrderItem(checkoutVM.CurrentOrder.SelectedOrderItem);

            //Assert
            Assert.Equal(price1 + price2, checkoutVM.CurrentOrder.NewTotal);
            Assert.Equal(checkoutVM.CurrentOrder.Total , checkoutVM.CurrentOrder.NewTotal);
            Assert.Equal("%", checkoutVM.NumericZone);
            Assert.Equal(0, checkoutVM.CurrentOrder.SelectedOrderItem.DiscountAmount);
        }

        [Fact]
        public void ActionKeyboard_Delete()
        {
            var checkoutVM = new CheckoutViewModel();
            checkoutVM.ActionKeyboard(ActionButton.Del);

            Assert.Null(checkoutVM.NumericZone);

            checkoutVM.NumericKeyboard("1");
            checkoutVM.ActionKeyboard(ActionButton.Del);

            Assert.Empty(checkoutVM.NumericZone);

            checkoutVM.NumericKeyboard("1");
            checkoutVM.NumericKeyboard("2");
            checkoutVM.NumericKeyboard("3");
            checkoutVM.ActionKeyboard(ActionButton.Del);

            Assert.Equal("12", checkoutVM.NumericZone);

            checkoutVM.ActionKeyboard(ActionButton.Del);

            Assert.Equal("1", checkoutVM.NumericZone);
        }

        [Fact]
        public void ActionKeyboard_Qty_IncreaseAndDecreaseQuantity_OrQuantityIsZero_CheckTotals()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            decimal price1 = 150,
                    price2 = 160;
            var p = new Platter { Id = 1, Description = "desc2", Name = "awescome", Price = price1 };
            var p2 = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = price2 };
            
            //Act
            checkoutVM.NumericZone = "5";
            checkoutVM.ActionKeyboard(ActionButton.Qty);

            //Assert  
            //Selected item is null, no exception would be thrown
            Assert.Empty(checkoutVM.NumericZone);

            //Act
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddOrderItem(p2);
            Assert.Equal(price2, checkoutVM.CurrentOrder.SelectedOrderItem.Total);
            Assert.Equal(price1 + price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(price1 + price2, checkoutVM.CurrentOrder.NewTotal);
            checkoutVM.NumericZone = "5";
            checkoutVM.ActionKeyboard(ActionButton.Qty); 

            //Assert
            Assert.Equal(price2*5, checkoutVM.CurrentOrder.SelectedOrderItem.Total);
            Assert.Equal(price1 + price2*5, checkoutVM.CurrentOrder.Total);
            Assert.Equal(price1 + price2*5, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);

            //Act
            checkoutVM.NumericZone = "2";
            checkoutVM.ActionKeyboard(ActionButton.Qty);
            
            //Assert
            Assert.Equal(price2*2, checkoutVM.CurrentOrder.SelectedOrderItem.Total);
            Assert.Equal(price1 + price2*2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(price1 + price2*2, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);

            //Act
            checkoutVM.NumericZone = "0";
            checkoutVM.ActionKeyboard(ActionButton.Qty);
            
            //Assert  when Qty is 0, keeps the value of last quantity
            Assert.Equal(price2*2, checkoutVM.CurrentOrder.SelectedOrderItem.Total);
            Assert.Equal(price1 + price2*2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(price1 + price2*2, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);
        }

        [Fact]
        public void ActionKeyboard_Disc_IncreraseDecreaseDiscountAmount()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            decimal price1 = 150,
                    price2 = 160;
            var p = new Platter { Id = 1, Description = "desc2", Name = "awescome", Price = price1 };
            var p2 = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = price2 };

            //Act
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddOrderItem(p2);
            checkoutVM.NumericZone = "0";
            checkoutVM.ActionKeyboard(ActionButton.Disc);

            //Assert  
            Assert.Equal(0, checkoutVM.CurrentOrder.DiscountAmount);
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);

            //Act  increaase discount
            checkoutVM.NumericZone = "100";
            checkoutVM.ActionKeyboard(ActionButton.Disc);

            //Assert  
            Assert.Equal(100, checkoutVM.CurrentOrder.DiscountAmount);
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(price1+price2-100, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);

            //Act  decreaase discount
            checkoutVM.NumericZone = "50";
            checkoutVM.ActionKeyboard(ActionButton.Disc);

            //Assert  
            Assert.Equal(50, checkoutVM.CurrentOrder.DiscountAmount);
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(price1+price2-50, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);
        }

       [Fact]
        public void ActionKeyboard_Disc_IncreraseDecreaseDiscountPercentage()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            decimal price1 = 150,
                    price2 = 160;
            var p = new Platter { Id = 1, Description = "desc2", Name = "awescome", Price = price1 };
            var p2 = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = price2 };

            //Act
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddOrderItem(p2);
            checkoutVM.NumericZone = "0%";
            checkoutVM.ActionKeyboard(ActionButton.Disc);

            //Assert  
            Assert.Equal(0, checkoutVM.CurrentOrder.DiscountAmount);
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);

            //Act  increaase discount
            checkoutVM.NumericZone = "50%";
            checkoutVM.ActionKeyboard(ActionButton.Disc);

            //Assert  
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(checkoutVM.CurrentOrder.Total * 50 / 100, checkoutVM.CurrentOrder.DiscountAmount);
            Assert.Equal(checkoutVM.CurrentOrder.Total - checkoutVM.CurrentOrder.Total * 50 /100, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);

            //Act  decreaase discount
            checkoutVM.NumericZone = "20%";
            checkoutVM.ActionKeyboard(ActionButton.Disc);

            //Assert  
            Assert.Equal(price1 + price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(checkoutVM.CurrentOrder.Total * 20 / 100, checkoutVM.CurrentOrder.DiscountAmount);
            Assert.Equal(checkoutVM.CurrentOrder.Total - checkoutVM.CurrentOrder.Total * 20 / 100, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);
        }

        [Fact]
        public void ActionKeyboard_Disc_DiscountGreaterThanorEqualsTotal()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            decimal price1 = 150,
                    price2 = 160;
            var p = new Platter { Id = 1, Description = "desc2", Name = "awescome", Price = price1 };
            var p2 = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = price2 };

            //Act
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddOrderItem(p2);
            checkoutVM.NumericZone = Convert.ToString(price1+price2);
            checkoutVM.ActionKeyboard(ActionButton.Disc);

            //Assert  
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.DiscountAmount);
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(0, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);

            
            //Act  discount greater than Total
            checkoutVM.NumericZone = Convert.ToString(price1 + price2+0.001m); 
            var oldDiscount = checkoutVM.CurrentOrder.DiscountAmount;
           
            //Assert  
            Assert.Throws<Exception>(() => checkoutVM.ActionKeyboard(ActionButton.Disc));
            Assert.Equal(oldDiscount, checkoutVM.CurrentOrder.DiscountAmount);
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(price1+price2-oldDiscount, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);
        }

        [Fact]
        public void ActionKeyboard_Disc_DiscountPercentageGreaterThanorEqualsTotal()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            decimal price1 = 150,
                    price2 = 160;
            var p = new Platter { Id = 1, Description = "desc2", Name = "awescome", Price = price1 };
            var p2 = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = price2 };

            //Act
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddOrderItem(p2);
            checkoutVM.NumericZone = "100%";
            checkoutVM.ActionKeyboard(ActionButton.Disc);

            //Assert  
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.DiscountAmount);
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(0, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);

            //Act  increaase discount
            checkoutVM.NumericZone ="120%"; 
            var oldDiscount = checkoutVM.CurrentOrder.DiscountAmount;
            checkoutVM.ActionKeyboard(ActionButton.Disc);         
            //Assert  
            Assert.Equal(oldDiscount, checkoutVM.CurrentOrder.DiscountAmount);
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(price1+price2-oldDiscount, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);
        }

       [Fact]
        public void ActionKeyboard_Price_PriceGreaterThanorEqualsOrLessThanTotal()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            decimal price1 = 150,
                    price2 = 160;
            var p = new Platter { Id = 1, Description = "desc2", Name = "awescome", Price = price1 };
            var p2 = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = price2 };

            //Act  the same as total
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddOrderItem(p2);
            checkoutVM.NumericZone = Convert.ToString(price1+price2);
            checkoutVM.ActionKeyboard(ActionButton.Price);

            //Assert  
            Assert.Equal(0, checkoutVM.CurrentOrder.DiscountAmount);
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(checkoutVM.CurrentOrder.Total, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);

            //Act  prise is 0
            checkoutVM.NumericZone = Convert.ToString(0);
            checkoutVM.ActionKeyboard(ActionButton.Price);

            //Assert  
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(checkoutVM.CurrentOrder.Total, checkoutVM.CurrentOrder.DiscountAmount);
            Assert.Equal(0, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);

            //Act  prise is less than total
            checkoutVM.NumericZone = Convert.ToString(120);
            checkoutVM.ActionKeyboard(ActionButton.Price);

            //Assert  
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(checkoutVM.CurrentOrder.Total-120, checkoutVM.CurrentOrder.DiscountAmount);
            Assert.Equal(120, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);

            //Act price greater than total
            checkoutVM.NumericZone = Convert.ToString(price1 + price2+0.001m); 
            var oldDiscount = checkoutVM.CurrentOrder.DiscountAmount;
           
            //Assert  
            Assert.Throws<Exception>(() => checkoutVM.ActionKeyboard(ActionButton.Price));
            Assert.Equal(oldDiscount, checkoutVM.CurrentOrder.DiscountAmount);
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(price1+price2-oldDiscount, checkoutVM.CurrentOrder.NewTotal);
            Assert.Empty(checkoutVM.NumericZone);
        }

       [Fact]
        public void ActionKeyboard_Payement_PayedPriceLessThanorEqualsTotal()
        {
            //Arrange
            var checkoutVM = new CheckoutViewModel();
            decimal price1 = 150,
                    price2 = 160;
            var p = new Platter { Id = 1, Description = "desc2", Name = "awescome", Price = price1 };
            var p2 = new Platter { Id = 2, Description = "desc2", Name = "awescome", Price = price2 };

            //Act  the same as total
            checkoutVM.AddOrderItem(p);
            checkoutVM.AddOrderItem(p2);
            checkoutVM.NumericZone = Convert.ToString(price1+price2);
            checkoutVM.ActionKeyboard(ActionButton.Payment);

            //Assert  
            Assert.Equal(price1 + price2, checkoutVM.CurrentOrder.GivenAmount);
            Assert.Equal(0, checkoutVM.CurrentOrder.ReturnedAmount);
            Assert.Empty(checkoutVM.NumericZone);

            //Act  after discount
            checkoutVM.NumericZone = Convert.ToString(50);
            checkoutVM.ActionKeyboard(ActionButton.Disc);
            checkoutVM.NumericZone = Convert.ToString(price1+price2-50);
            checkoutVM.ActionKeyboard(ActionButton.Payment);

            //Assert  
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(price1+price2-50, checkoutVM.CurrentOrder.NewTotal);
            Assert.Equal(price1 + price2 - 50, checkoutVM.CurrentOrder.GivenAmount);
            Assert.Equal(0, checkoutVM.CurrentOrder.ReturnedAmount);
            Assert.Empty(checkoutVM.NumericZone);

            //Act price less than total
            checkoutVM.NumericZone = Convert.ToString(checkoutVM.CurrentOrder.NewTotal - 0.001m); 
            var oldDiscount = checkoutVM.CurrentOrder.DiscountAmount;
           
            //Assert  
            Assert.Throws<Exception>(() => checkoutVM.ActionKeyboard(ActionButton.Payment));
            Assert.Equal(oldDiscount, checkoutVM.CurrentOrder.DiscountAmount);
            Assert.Equal(price1+price2, checkoutVM.CurrentOrder.Total);
            Assert.Equal(checkoutVM.CurrentOrder.Total - oldDiscount, checkoutVM.CurrentOrder.NewTotal);
            //Assert.Empty(checkoutVM.NumericZone);
        }

        [Fact]
        public void NumericKeyboard_Numbers() 
        {
            var checkoutVM = new CheckoutViewModel();

            string str = checkoutVM.NumericZone;
            checkoutVM.NumericKeyboard(null);
            
            Assert.Equal(str, checkoutVM.NumericZone);

            checkoutVM.NumericKeyboard("x");

            Assert.Equal(str, checkoutVM.NumericZone);

            checkoutVM.NumericKeyboard("xy");

            Assert.Equal(str, checkoutVM.NumericZone);

            checkoutVM.NumericKeyboard("12");

            Assert.Equal(str, checkoutVM.NumericZone);

            checkoutVM.NumericKeyboard("1");

            Assert.Equal(str+"1", checkoutVM.NumericZone);

            str += "1";
            checkoutVM.NumericKeyboard("9");

            Assert.Equal(str+"9", checkoutVM.NumericZone);

            str += "9";
            checkoutVM.NumericKeyboard("8");

            Assert.Equal(str+"8", checkoutVM.NumericZone);

            str += "8";
            checkoutVM.NumericKeyboard(".");

            Assert.Equal(str+".", checkoutVM.NumericZone);

            str += ".";
            checkoutVM.NumericKeyboard(".");

            Assert.Equal(str, checkoutVM.NumericZone);
            
            checkoutVM.NumericKeyboard(".");

            Assert.Equal(str, checkoutVM.NumericZone);

        }

        [Fact]
        public void NumericKeyboard_Percentage() 
        {
            var checkoutVM = new CheckoutViewModel();

            checkoutVM.NumericZone = string.Empty;
            string str = checkoutVM.NumericZone;
            checkoutVM.NumericKeyboard("%");
            
           
            checkoutVM.NumericKeyboard("1");

            Assert.Equal("1%", checkoutVM.NumericZone);

            checkoutVM.NumericKeyboard("%");
            str = "1%";

            Assert.Equal(str, checkoutVM.NumericZone);

            checkoutVM.NumericKeyboard("2");
            str = "12%";

            Assert.Equal(str, checkoutVM.NumericZone);


        }

        [Fact]
        public void NumericKeyboard_PercentageGreaterThan100() 
        {
            var checkoutVM = new CheckoutViewModel();

            checkoutVM.NumericZone = string.Empty;
            string str = checkoutVM.NumericZone;
            checkoutVM.NumericKeyboard("%");
                    
            checkoutVM.NumericKeyboard("1");
            checkoutVM.NumericKeyboard("%");
            checkoutVM.NumericKeyboard("2");

            Assert.Throws<Exception>(()=> checkoutVM.NumericKeyboard("5"));
        }

    }
}
