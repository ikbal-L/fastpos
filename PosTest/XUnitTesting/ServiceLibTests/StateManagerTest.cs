using Moq;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.Exceptions;
using ServiceLib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTesting.ServiceLibTests
{
    public class StateManagerTest 
    {
        private Mock<IProductRepository> _productRepository;
        private Mock<ICategoryRepository> _categoryRepositroy;

        public StateManagerTest()
        {
            _productRepository = new Mock<IProductRepository>();
            _categoryRepositroy = new Mock<ICategoryRepository>();
            _productRepository.Setup(pr => pr.Get()).Returns((200, MockingHelpers.GetAllProducts()));
            _categoryRepositroy.Setup(cr => cr.Get()).Returns((200, MockingHelpers.GetAllCategories()));

            StateManager.Instance
                .Manage(_productRepository.Object)
                .Manage(_categoryRepositroy.Object);

        }

        [Fact]
        public void GetTState_TStateIsUmmangedState_ThrowsStateNotManagedExceptionTState()
        {
            //Arrange 

            //Act 
            Action act = () => StateManager.Get<Table>();

            //Assert 
            var e = Assert.Throws<StateNotManagedException<Table>>(act);
        }
    }
}
