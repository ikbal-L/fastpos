using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PosTest.Helpers;
using ServiceInterface.Model;
using Xunit;
using XUnitTesting.ViewModelTesting;

namespace XUnitTesting.HelpersTesting
{
    public class RankedItemsCollectionHelperTest
    {
        
        [Fact]
        public void InsertTElementInPositionOf_IncomingArgRankEqualsNullAndTargetArgIdNotEqualToNull_ThrowsInvalidOperationException()
        {
            //arrange
            IList<Category> list = MockingHelpers.GetAllCategories().ToList();
            Category incomingArg = new Category(){Id = 5,Rank = null};
            Category targetArg = list.First(category => category.Id == 5);//Id 5 Rank 8 Name Cat1
            
            //act
            Action act = () => RankedItemsCollectionHelper.InsertTElementInPositionOf(ref incomingArg,ref targetArg,ref list);
            
            //assert
            var e =Assert.Throws<InvalidOperationException>(act);
            Assert.Equal("Must Select an empty target", e.Message);

        }

        [Fact]
        public void InsertTElementInPositionOf_IncomingArgRankEqualsNullAndTargetArgIdEqualsNull_ReturnTargetArgEqualsNullAndIncomingArgRankUpdated()
        {
            //arrange
            IList<Category> list = MockingHelpers.GetAllCategories().ToList();
            list.Add(new Category(){Id = null,Rank = 22,Name = "Empty Category 1"});
            list.Add(new Category(){Id = null,Rank = 21,Name = "Empty Category 2"});
            Category incomingArg = new Category() { Id = 5, Rank = null };
            Category targetArg = list.First(category => category.Rank == 21);//Id null Rank 21 Name Empty Category 2

            //act
            RankedItemsCollectionHelper.InsertTElementInPositionOf(ref incomingArg, ref targetArg, ref list);

            //assert
            Assert.Null(targetArg);
            Assert.Equal(21,incomingArg.Rank);

        }

        [Fact]
        public void InsertTElementInPositionOf_IncomingArgRankAndTargetArgIdNotEqualToNull_ReturnTargetArgRankAndIncomingArgRankUpdated()
        {
            //arrange
            IList<Category> list = MockingHelpers.GetAllCategories().ToList();
            Category incomingArg = list.First(category => category.Id == 2); //{Id = 02, Rank = 18, Name = "Cat2"};
            Category targetArg = list.First(category => category.Id == 5);//{Id = 05, Rank = 08, Name = "Cat1"}

            //act
            RankedItemsCollectionHelper.InsertTElementInPositionOf(ref incomingArg, ref targetArg, ref list);

            //assert
            Assert.Equal(18,targetArg.Rank);
            Assert.Equal(8, incomingArg.Rank);

        }

        [Fact]
        public void InsertTElementInPositionOf_IncomingArgEqualsNull_ThrowsNullReferenceException()
        {
            //arrange
            IList<Category> list = MockingHelpers.GetAllCategories().ToList();
            Category incomingArg = null;
            Category targetArg = list.First(category => category.Id == 5);//{Id = 05, Rank = 08, Name = "Cat1"}

            //act
            Action act = () => RankedItemsCollectionHelper.InsertTElementInPositionOf(ref incomingArg, ref targetArg, ref list);

            //assert
            var e = Assert.Throws<NullReferenceException>(act);
            Assert.Equal("Incoming Arg must not be null", e.Message);

        }

    }
}
