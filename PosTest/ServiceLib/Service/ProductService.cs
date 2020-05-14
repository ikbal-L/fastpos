using Newtonsoft.Json;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;

using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLib.Service
{
    [Export(typeof(IProductService))]
    class ProductService : IProductService
    {

        public ICollection<Product> GetAllProducts()
        {
            return FakeServices.Products;
        }

        public List<Product> createProducts()
        {
            Product p1 = new Product { Id = 1, Name = "AAAA", Type = "A", Color = "blue" };
            Product p2 = new Product { Id = 2, Name = "BBBB", Type = "A", Color = "white" };
            Product p3 = new Product { Id = 3, Name = "CCCC", Type = "B", Color = "gray" };
            var products = new List<Product>
            {
                p1,
                p2,
                p3
            };
            return products;
        }

        public  async Task<List<Product>> getProductsREST()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://127.0.0.1:5000/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Console.WriteLine(11);

            List<Product> taskPproducts = await GetProductAsync(client, "/products");
            Console.WriteLine(12);
          
            return taskPproducts;
        }

        private async Task<List<Product>> GetProductAsync(HttpClient client, string path)
        {
            List<Product> products = null;

            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {

                products = await response.Content.ReadAsAsync<List<Product>>();

            }
            return products;
        }

        public bool SaveProduct(Product product)
        {
            string json = JsonConvert.SerializeObject(product,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = "http://127.0.0.1:5000/SaveProduct";
            var client = new HttpClient();

            var response =  client.PostAsync(url, data).Result;

            if (response.IsSuccessStatusCode)
                return true;

            return false;
        }

        public bool SaveProducts(IEnumerable<Product> products)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Product GetProduct(long id)
        {
            Product product=null;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://127.0.0.1:5000/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync($"/product/{id}").Result;

            if (response.IsSuccessStatusCode)
            {

                product =  response.Content.ReadAsAsync<Product>().Result;

            }
            return product;

        }
    }
}
