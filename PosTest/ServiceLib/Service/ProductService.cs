using Newtonsoft.Json;
using RestSharp;
using ServiceInterface.Authorisation;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ICollection<Product> GetAllProducts()
        {
            string token = AuthProvider.Instance?.AuthorizationToken;
            var client = new RestClient(UrlConfig.ProductUrl.GetAllProducts);
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", token);
            request.AddHeader("accept", "application/json");
            request.AddHeader("content-type", "application/json");
            IRestResponse response = client.Execute(request);
            ICollection<Product> products = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var all = JsonConvert.DeserializeObject<List<Platter>>(response.Content);
                if (all != null && all.Count>0)
                {
                    products = new Collection<Product>();
                    foreach (var item in all)
                    {
                        if (!item.IsPlatter)
                        {
                            var p = new Product
                            {
                                AvailableStock = item.AvailableStock,
                                BackgroundString = item.BackgroundString,
                                CategorieId = item.CategorieId,
                                Description = item.Description,
                                Id = item.Id,
                                IsMuchInDemand = item.IsMuchInDemand,
                                Name = item.Name,
                                PictureFileName = item.PictureFileName,
                                PictureUri = item.PictureUri,
                                Price = item.Price,
                                Type = item.Type,
                                Unit = item.Unit
                            };
                            products.Add(p);
                        }
                        else
                        {
                            products.Add(item);
                        }
                    }

                } 
            }
            
            return products;// FakeServices.Products;
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
            string token = AuthProvider.Instance.AuthorizationToken;
            string json = JsonConvert.SerializeObject(product,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = UrlConfig.ProductUrl.SaveProduct;
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
            var client = new RestClient(UrlConfig.ProductUrl.SaveProducts);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", token);
           
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
            var client = new RestClient(UrlConfig.ProductUrl.DeleteProduct + $"{productId}");
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


}
