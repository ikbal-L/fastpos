using System;
using System.Collections.Generic;
using ServiceInterface.Model;
using ServiceLib.helpers;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;
using Xunit;

namespace XUnitTesting.ServiceLibTests
{
    public class AssociationManagerTest
    {
        public AssociationManagerTest()
        {
            var prodRepo = new ProductRepository();
            var catRepo = new CategoryBaseRepository();
            StateManager.Instance
                .Manage(prodRepo,false)
                .Manage(catRepo,false);
            var authService = new Authentification();
            var resp = authService.Authenticate("admin", "admin", new Annex { Id = 1 }, new Terminal { Id = 1 });
        }
        [Fact]
        public void test()
        {
            //arrange
            var associationManager = AssociationManager.Instance;

            //act

            associationManager
                .Register()
                .Associate<Product>()
                .With<Additive>()
                .Using<Product, Additive>(Map)
                .Build();
            var action = associationManager.GetMapper<Product, Additive>();
            

            //assert
            Assert.NotNull(action);
           
        }

        private void Map(IEnumerable<Product> arg1, IEnumerable<Additive> arg2)
        {
            Console.WriteLine("Do some mapping");
        }

        
    }
}