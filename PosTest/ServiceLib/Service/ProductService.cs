using Newtonsoft.Json;
using RestSharp;
using ServiceInterface.Authorisation;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.Http;

using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLib.Service
{
    [Export(typeof(IProductService))]
    class ProductService : IProductService
    {
        [Import(typeof(IAdditiveService))]
        IAdditiveService additiveService;

        public ICollection<Product> GetAllProducts()
        {
            string token = AuthProvider.Instance?.AuthorizationToken;
            var client = new RestClient("http://127.0.0.1:5000/product/getall");
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", token);
            request.AddHeader("accept", "application/json");
            request.AddHeader("content-type", "application/json");            
            IRestResponse response = client.Execute(request);
            ICollection<Product> products;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                products = JsonConvert.DeserializeObject<ICollection<Product>>(response.Content);
            }
            
            return FakeServices.Products;// ;products
        }

        public List<Product> createProducts()
        {
            var a = additiveService.GetAdditive(1);
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
            string token = AuthProvider.Instance.AuthorizationToken;
            product = MapProduct.MapProductToSend(product);
            string json = JsonConvert.SerializeObject(product,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = "http://127.0.0.1:5000/product/save";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            var response =  client.PostAsync(url, data).Result;

            if (response.IsSuccessStatusCode)
                return true;

            return false;
        }

        public bool SaveProducts(IEnumerable<Product> products)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient("http://127.0.0.1:5000/product/savemany");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", token);
            foreach (var p in products)
            {
                MapProduct.MapProductToSend(p);
            }
            var jsonData = JsonConvert.SerializeObject(products,
                                        Newtonsoft.Json.Formatting.None,
                                        new JsonSerializerSettings
                                        {
                                            NullValueHandling = NullValueHandling.Ignore
                                        });
            Console.WriteLine(jsonData);
            request.AddParameter("application/json", jsonData, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Product GetProduct(long id)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            Product product=null;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://127.0.0.1:5000/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", token);
            HttpResponseMessage response = client.GetAsync($"/product/get/{id}").Result;
            var result = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(result);
            if (response.IsSuccessStatusCode)
            {
                product =  response.Content.ReadAsAsync<Product>().Result;
            }
            return product;

        }

        public bool DeleteProduct(long productId)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient($"http://127.0.0.1:5000/product/delete/{productId}");
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", token);
            //request.AddParameter("application/json", "{\n\"id\":1\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK )
            {
                return true;
            }
            return false;
        }

        public void PostTest(Product product)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient("http://127.0.0.1:5000/product/sss");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", token);
            product = MapProduct.MapProductToSend(product);
            var jsonData = JsonConvert.SerializeObject(product,
                                        Newtonsoft.Json.Formatting.None,
                                        new JsonSerializerSettings
                                        {
                                            NullValueHandling = NullValueHandling.Ignore
                                        });
            Console.WriteLine(jsonData);
            request.AddParameter("application/json", jsonData, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }
    }

    [Export(typeof(IProductService))]
    public class AdditiveService : IAdditiveService
    {
        public bool DeleteAdditive(long idProduct)
        {
            throw new NotImplementedException();
        }

        public Additive GetAdditive(long id)
        {
           return new Additive { Id=id};
        }

        public IEnumerable<Additive> GetManyAdditives(IEnumerable<long> ids)
        {
            throw new NotImplementedException();
        }

        public bool SaveAdditive(Additive additive)
        {
            throw new NotImplementedException();
        }

        public bool SaveAdditives(IEnumerable<Additive> additives)
        {
            throw new NotImplementedException();
        }
    }
}
