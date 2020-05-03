using PosTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTesting.CheckpointTesting
{
    public class CheckpointTests
    {
        [Fact]
        public void NumericKeyboard() 
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
    }
}
