using Moq;
using PosTest.ViewModels;
using ServiceInterface.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTesting
{
    public class ProductsViewModelTest
    {
        ProductsViewModel _viewModel;
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
