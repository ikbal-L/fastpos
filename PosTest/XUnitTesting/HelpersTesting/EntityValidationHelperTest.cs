using FastPosFrontend.Helpers;
using ServiceInterface.Model;
using Xunit;

namespace XUnitTesting.HelpersTesting
{
    public class EntityValidationHelperTest
    {
        [Fact]
        public void Validate_InstanceArgHasValidationErrors_ReturnStringCollectionOfValidationErrors()
        {
            var product = new Product();
            var category = new Category();
            // var order = new Order();

            var productValidationResults = EntityValidationHelper.Validate(product);
            var categoryValidationResults = EntityValidationHelper.Validate(category);
            // var orderValidationResults = EntityValidationHelper.Validate(order);

            Assert.NotEmpty(productValidationResults);
            Assert.NotEmpty(categoryValidationResults);
            // Assert.NotEmpty(orderValidationResults);
        }
    }
}