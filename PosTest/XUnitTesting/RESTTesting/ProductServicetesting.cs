using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace XUnitTesting.RESTTesting
{
    public class ProductServicetesting
    {
        private readonly ITestOutputHelper _output;

        public ProductServicetesting(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task PaginateProducts_Async()
        {
            string json = JsonConvert.SerializeObject("sss");

            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = "http://127.0.0.1:5000/guid";
            var client = new HttpClient();

            var response = await client.PostAsync(url, data);
            
           //var result = response.Content.ReadAsStringAsync().Result;
            _output.WriteLine(response.StatusCode.ToString());
        }
    }
}
