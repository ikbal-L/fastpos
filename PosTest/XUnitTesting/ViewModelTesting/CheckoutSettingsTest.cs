using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using PosTest.ViewModels;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using Xunit;

namespace XUnitTesting.ViewModelTesting
{
    public class CheckoutSettingsTest
    {
        private CheckoutSettingsViewModel _checkoutSettingsViewModel;
        private Mock<IProductService> _productService;
        private Mock<ICategoryService> _categoryService;

        public CheckoutSettingsTest()
        {
            //_productService.Setup((ps => ps.GetAllProducts(ref productServiceStatusCode))).Returns(GetAllProducts().ToList);
            _productService = new Mock<IProductService>();
            _categoryService = new Mock<ICategoryService>();

            //
            //_categoryService
            //    .Setup((cs =>
            //        cs.GetAllCategoriesAndProducts(ref categoryServiceStatusCode, ref productServiceStatusCode)))
            //    .Returns((GetAllCategories(), GetAllProducts()));

            _categoryService
                .Setup((cs =>
                    cs.GetAllCategoriesAndProducts(ref It.Ref<int>.IsAny, ref It.Ref<int>.IsAny)))
                .Callback(new GetGetAllProductsCallback(
                    (ref int categoryServiceStatusCode, ref int productServiceStatusCode) =>
                    {
                        categoryServiceStatusCode = 200;
                        productServiceStatusCode = 200;
                    }
                )).Returns((GetAllCategories(), GetAllProducts()));


            _productService.Setup((ps => ps.UpdateProduct(new Product()))).Returns(200);
            _categoryService.Setup((cs => cs.UpdateCategory(new Category()))).Returns(200);
            int productPageSize = 30, categoryPageSize = 10;
            _checkoutSettingsViewModel = new CheckoutSettingsViewModel(productPageSize, categoryPageSize,
                _productService.Object, _categoryService.Object);
        }

        public delegate void GetGetAllProductsCallback(ref int categStatusCode,
            ref int prodStatusCode);

        public static IEnumerable<Product> GetAllProducts()
        {
            yield return new Product() {Id = 5, Rank = 6};
            yield return new Product() {Id = 2, Rank = 18};
            yield return new Product() {Id = 17, Rank = 6};
            yield return new Product() {Id = 23, Rank = 27};
        }

        public static IEnumerable<Category> GetAllCategories()
        {
            yield return new Category() {Id = 05, Rank = 08, Name = "Cat1"};
            yield return new Category() {Id = 02, Rank = 18, Name = "Cat2"};
            yield return new Category() {Id = 17, Rank = 06, Name = "Cat3"};
            yield return new Category() {Id = 23, Rank = 27, Name = "Cat4"};
            yield return new Category() {Id = 08, Rank = 24, Name = "Cat5"};
            yield return new Category() {Id = 19, Rank = null, Name = "Cat6"};
            yield return new Category() {Id = 14, Rank = null, Name = "Cat7"};
        }

        [Fact]
        public void PutCategoryInCellOf_BothCategoriesFromCurrentCategoriesList_PermutateCategories()
        {
            Category targetCategory = _checkoutSettingsViewModel.CurrentCategories.First(category => category.Id == 5); //Rank 8
            Category incomingCategory =
                _checkoutSettingsViewModel.CurrentCategories.First(category => category.Id == 23); //Rank 27 

            _checkoutSettingsViewModel.PutCategoryInCellOf(targetCategory, incomingCategory);


            Assert.Equal(27, targetCategory.Rank);
            Assert.Equal(8, incomingCategory.Rank);
        }


    }
}