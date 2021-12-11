using System.Linq;
using FastPosFrontend.ViewModels;
using Moq;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using Xunit;

namespace XUnitTesting.ViewModelTesting
{
    public class CheckoutSettingsTest
    {
        private CheckoutSettingsViewModel _checkoutSettingsViewModel;


        public CheckoutSettingsTest()
        {
            _checkoutSettingsViewModel = new CheckoutSettingsViewModel();
        }

        public delegate void GetGetAllProductsCallback(ref int categStatusCode,
            ref int prodStatusCode);

        [Fact]
        public void PutCategoryInCellOf_BothCategoriesFromCurrentCategoriesList_PermutateCategories()
        {
            Category targetCategory =
                _checkoutSettingsViewModel.CurrentCategories.First(category => category.Id == 5); //Rank 8
            Category incomingCategory =
                _checkoutSettingsViewModel.CurrentCategories.First(category => category.Id == 23); //Rank 27 

            _checkoutSettingsViewModel.PutCategoryInCellOf(targetCategory, incomingCategory);


            Assert.Equal(27, targetCategory.Rank);
            Assert.Equal(8, incomingCategory.Rank);
        }

        [Fact]
        public void PutCategoryInCellOf_IncomingCategoryFromFreeCategoriesList_OverwriteTargetCategory()
        {
            Category targetCategory =
                _checkoutSettingsViewModel.CurrentCategories.First(category => category.Rank == 25); //Id null Rank 25
            Category incomingCategory =
                _checkoutSettingsViewModel.FreeCategories.First(category => category.Id == 19); // Id 19 Rank null 


            //Act
            _checkoutSettingsViewModel.PutCategoryInCellOf(targetCategory, incomingCategory);

            //Assert 
            Assert.Equal(null, targetCategory.Rank);
            Assert.Equal(25, incomingCategory.Rank);
        }

        [Fact]
        public void RemoveCategoryFromTList_CategoryEqualsNull_Return()
        {
            _checkoutSettingsViewModel.SelectedCategory = null;
            Category selectedFreeCategory = null;

            //act
            _checkoutSettingsViewModel.RemoveTElementFromTList(
                _checkoutSettingsViewModel.SelectedCategory,
                ref selectedFreeCategory, _checkoutSettingsViewModel.CurrentCategories,
                _checkoutSettingsViewModel.FreeCategories);
            //Assert

            Assert.Equal(null, _checkoutSettingsViewModel.SelectedFreeCategory);
        }


        [Fact]
        public void RemoveCategoryFromTList_CategoryDoesNotEqualsNull_CategoryRemoved()
        {
            //Arrange
            _checkoutSettingsViewModel.SelectedCategory =
                _checkoutSettingsViewModel.CurrentCategories.First(category => category.Id == 5);
            Category selectedCategory = _checkoutSettingsViewModel.SelectedCategory;
            Category selectedFreeCategory = null;

            //act
            _checkoutSettingsViewModel.RemoveTElementFromTList(
                _checkoutSettingsViewModel.SelectedCategory,
                ref selectedFreeCategory, _checkoutSettingsViewModel.CurrentCategories,
                _checkoutSettingsViewModel.FreeCategories);
            //Assert 
            Assert.Equal(null, _checkoutSettingsViewModel.SelectedCategory);
            Assert.Equal(selectedCategory.Id,selectedFreeCategory.Id);
        }
    }
}