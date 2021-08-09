using Moq;
using FastPosFrontend.ViewModels;
using ServiceInterface.Interface;
using Xunit;

namespace XUnitTesting
{
    public class ProductsViewModelTest
    {
        
        private Mock<IProductService> _IProductServiceMock;

        public ProductsViewModelTest()
        {
            _IProductServiceMock = new Mock<IProductService>();
            //_viewModel = new ProductsViewModel(5,_IProductServiceMock.Object);
        }

        [Fact]
        public void ShouldLoadProduct()
        {
        }

        [Fact]
        public void ShouldAddedNewProduct()
        { 
            
        }

        [Fact]
        public void ShouldDeleteProduct()
        { 
            
        }


    }
}
