using System.Threading.Tasks;
using ServiceInterface.Model;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;
using Xunit;

namespace XUnitTesting.ServiceLibTests
{
    public class GenericRestAsyncTest
    {
        public GenericRestAsyncTest()
        {
            var authService = new RestAuthentification();
            var resp = authService.Authenticate("admin", "admin", new Annex { Id = 1 }, new Terminal { Id = 1 });
        }

        [Fact]
        public async Task GetThingAsync_AsynchronousSuccess_ResultNotEmpty()
        {
            
            var productRepository = new ProductRepository();
            StateManager.Instance.Manage(productRepository,false);
            var result = await StateManager.GetAsync<Product>();

            Assert.NotEmpty(result);

        }
    }
}