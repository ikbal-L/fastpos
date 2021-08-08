using Moq;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.Exceptions;
using ServiceLib.Service;
using System;
using ServiceLib.Service.StateManager;
using Xunit;

namespace XUnitTesting.ServiceLibTests
{
    public class StateManagerTest 
    {
        public StateManagerTest()
        {
            var productRepository = new Mock<IProductRepository>();
            var categoryRepository = new Mock<ICategoryRepository>();
            
            productRepository.Setup(pr => pr.Get()).Returns((200, MockingHelpers.GetAllProducts()));
            categoryRepository.Setup(cr => cr.Get()).Returns((200, MockingHelpers.GetAllCategories()));
            
            StateManager.Instance
                .Manage(productRepository.Object)
                .Manage(categoryRepository.Object);
            var authService = new Authentification();
            var resp = authService.Authenticate("admin", "admin", new Annex { Id = 1 }, new Terminal { Id = 1 });


        }

        [Fact]
        public void GetTState_TStateIsUnmanagedState_ThrowsStateNotManagedExceptionTState()
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
        [Fact]
        public void DeleteTState_IdListNotNullOrEmpty_TargetEntitiesAreDeleted()
        {
            var tablesRepository = new TableRepository();
            long[] ids = {1, 2, 3, 5};
            StateManager.Instance.Manage(tablesRepository);
            var result = StateManager.Delete<Table,long>(ids);
            Assert.True(result);
        }

        
    }
}
