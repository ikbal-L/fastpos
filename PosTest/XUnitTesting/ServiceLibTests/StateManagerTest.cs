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
            Action act1 = () => StateManager.Get<Table>();
            Action act2 = () => StateManager.Get<Table,long>();
            Action act3 = () => StateManager.Get<Table>(1);
            Action act4 = () => StateManager.Get<Table,long>(1);

            //Assert 
            var ex1 = Assert.Throws<StateNotManagedException<Table>>(act1);
            var ex2 = Assert.Throws<StateNotManagedException<Table>>(act2);
            var ex3 = Assert.Throws<StateNotManagedException<Table>>(act3);
            var ex4 = Assert.Throws<StateNotManagedException<Table>>(act4);
        }
    }
}
