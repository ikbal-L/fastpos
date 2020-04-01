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
        

        public List<Product> createProducts()
        {
            Product p1 = new Product { Id = 1, Name = "AAAA", Type = "A", Color = "blue" };
            Product p2 = new Product { Id = 2, Name = "BBBB", Type = "A", Color = "white" };
            Product p3 = new Product { Id = 3, Name = "CCCC", Type = "B", Color = "gray" };
            var products = new List<Product>();
            products.Add(p1);
            products.Add(p2);
            products.Add(p3);
            return products;
        }

        public  async Task<List<Product>> getProductsREST()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://127.0.0.1:8080/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            List<Product> taskPproducts = await GetProductAsync(client, "/Products");
          
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
    }
}
