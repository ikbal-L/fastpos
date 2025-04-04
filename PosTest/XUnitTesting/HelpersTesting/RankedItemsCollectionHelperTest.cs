﻿using System;
using System.Collections.Generic;
using System.Linq;
using FastPosFrontend.Helpers;
using ServiceInterface.Model;
using Xunit;
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
            Action act = () => RankedItemsCollectionHelper.InsertTElementInPositionOf(ref incomingArg,ref targetArg, list);
            
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
            RankedItemsCollectionHelper.InsertTElementInPositionOf(ref incomingArg, ref targetArg,  list);

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
            RankedItemsCollectionHelper.InsertTElementInPositionOf(ref incomingArg, ref targetArg,  list);

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
            Action act = () => RankedItemsCollectionHelper.InsertTElementInPositionOf(ref incomingArg, ref targetArg,  list);

            //assert
            var e = Assert.Throws<NullReferenceException>(act);
            Assert.Equal("Incoming Arg must not be null", e.Message);

        }

        [Fact]
        public void InsertTElementInPositionOf_TargetArgEqualsNull_ThrowsNullReferenceException()
        {
            //arrange
            IList<Category> list = MockingHelpers.GetAllCategories().ToList();
            Category incomingArg = list.First(category => category.Id == 5);//{Id = 05, Rank = 08, Name = "Cat1"}
            Category targetArg = null;

            //act
            Action act = () => RankedItemsCollectionHelper.InsertTElementInPositionOf(ref incomingArg, ref targetArg,  list);

            //assert
            var e = Assert.Throws<NullReferenceException>(act);
            Assert.Equal("Target Arg must not be null", e.Message);

        }

        [Fact]
        public void LoadPagesFilled_SourceListNotEmptyNotEqualToNullAndSizeGreaterThanZero_TargetListFilledWithRankedEmptyItems()
        {
            
            //Arrange 
            List<Category> sourceList = MockingHelpers.GetAllCategories().Where(c => c.Rank != null).ToList();
            var comparer = new FastPosFrontend.ViewModels.Comparer<Category>();
            sourceList.Sort(comparer);
            IList<Category> targetList = new List<Category>();

            //Act 
            RankedItemsCollectionHelper.LoadPagesFilled(sourceList,targetList,30);
            
            
            //Assert 
            //the other filled items that do not exist in source 
            var filledItems = targetList.Where(c=>!sourceList.Contains(c));
            Assert.All(filledItems,Assert.NotNull);

        }

        [Fact]
        public void LoadPagesNotFilled_SourceListNotEmptyNotEqualToNullAndSizeGreaterThanZero_TargetListFilledWithNullItems()
        {

            //Arrange 
            List<Category> sourceList = MockingHelpers.GetAllCategories().Where(c => c.Rank != null).ToList();
            var comparer = new FastPosFrontend.ViewModels.Comparer<Category>();
            sourceList.Sort(comparer);
            IList<Category> targetList = new List<Category>();

            //Act 
            RankedItemsCollectionHelper.LoadPagesNotFilled(sourceList, targetList, 30);


            //Assert 
            //the other filled items that do not exist in source 
            var filledItems = targetList.Where(c => !sourceList.Contains(c));
            Assert.All(filledItems, Assert.Null);

        }


        //Testing the underlying private function, Not important if filled or not 
        [Fact]
        public void LoadPagesFilled_TypeOfTEqualsTypeOfProductAndParameterEqualsNull_ThrowsNullReferenceException()
        {

            //Arrange 
            List<Product> sourceList = MockingHelpers.GetAllProducts().Where(p => p.Rank != null).ToList();
            var comparer = new FastPosFrontend.ViewModels.Comparer<Product>();
            sourceList.Sort(comparer);
            IList<Product> targetList = new List<Product>();

            //Act 
            Action act = () => RankedItemsCollectionHelper.LoadPagesFilled(sourceList, targetList, 30);

            //Assert 
            var e =Assert.Throws<NullReferenceException>(act);
            Assert.Equal("Parameter must not be null if Type of T equals Type of  Product", e.Message);

        }

    }
}
