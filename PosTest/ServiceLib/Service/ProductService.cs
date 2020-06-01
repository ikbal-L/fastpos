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
using System.Linq;
using System.Net;
using System.Net.Http;

using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLib.Service
{
    [Export(typeof(IProductService))]
    public class ProductService : IProductService
    {
        private RestProductService _restProductService = RestProductService.Instance;
        private RestAdditiveService _restAdditiveService = RestAdditiveService.Instance;
        private RestCategoryService _restCategoryService = RestCategoryService.Instance;

        public bool UpdateProduct(Product product)
        {
            product.MappingBeforeSending();
            return _restProductService.UpdateProduct(product);

        }

        bool IProductService.DeleteProduct(long idProduct)
        {
            return _restProductService.DeleteProduct(idProduct);
        }

        ICollection<Product> IProductService.GetAllProducts()
        {
            IEnumerable<Additive> GetAdditivesFromCollection(IEnumerable<Additive> additives, IEnumerable<long> idAdditves)
            {
                foreach (var id in idAdditves)
                {
                    var additive = additives.Where(a => a.Id == id).FirstOrDefault();
                    if (additive != null)
                    {
                        yield return additive;
                    }
                }
            }

            var t1 = DateTime.Now;
            var products = _restProductService.GetAllProducts();
            if (products == null)
            {
                return null;
            }
            var idProducts = new List<long>();
            var idCategories = new HashSet<long>();
            var idAdditivesOfAllProducts = new HashSet<long>();
            products.ToList().ForEach(p => idProducts.Add(p.Id));
            products.ToList().ForEach(p => idCategories.Add(p.CategorieId));
            foreach (var p in products)
            {
                if (p is Platter plat && plat.IdAdditives != null)
                {
                    plat.IdAdditives.ForEach(id => idAdditivesOfAllProducts.Add(id));
                }
            }

            var additivesOfAllProducts = _restAdditiveService.GetManyAdditives(idAdditivesOfAllProducts);
            var categories = _restCategoryService.GetManyCategories(idCategories);
            var t2 = DateTime.Now;
            Console.WriteLine($"{t2 - t1}");
            foreach (var p in products)
            {
                Category category;
                if (categories.Any(c => c.Id == p.CategorieId))
                {
                    category = categories.Where(c => c.Id == p.CategorieId).First();
                }
                else
                {
                    category = _restCategoryService.GetCategory(p.CategorieId);
                }
                IEnumerable<Additive> additives = null;
                if (p is Platter plat && plat.IdAdditives != null)
                {
                    additives = GetAdditivesFromCollection(additivesOfAllProducts, plat.IdAdditives);
                }
 
                p.MappingAfterReceiving(category, additives?.ToList());
            }

            return products;
        }

        Product IProductService.GetProduct(long id)
        {
            var product =_restProductService.GetProduct(id);
            if(product == null)
            {
                return null;
            }
            var category = _restCategoryService.GetCategory(product.CategorieId);
            IEnumerable<Additive> additives = null;
            if (product is Platter plat && plat.IdAdditives!=null && plat.IdAdditives.Count>0)
            {
                additives = _restAdditiveService.GetManyAdditives(plat.IdAdditives);
            }
            product.MappingAfterReceiving(category, additives.ToList());

            return product;
        }

        bool IProductService.SaveProduct(Product product)
        {
            product.MappingBeforeSending();
            return _restProductService.SaveProduct(product);
        }

        bool IProductService.SaveProducts(IEnumerable<Product> products)
        {
            if (products == null)
            {
                return false;
            }
            products.ToList().ForEach(p => p.MappingBeforeSending());
            return _restProductService.SaveProducts(products);
            throw new NotImplementedException();
        }
    }


    internal class RestProductService{
        private static RestProductService _instance;

        public static RestProductService Instance => _instance ?? (_instance = new RestProductService());
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
                var plattersProducts = JsonConvert.DeserializeObject<List<Platter>>(response.Content);
                if (plattersProducts != null && plattersProducts.Count>0)
                {
                    products = new Collection<Product>();
                    foreach (var platProduct in plattersProducts)
                    {
                        if (!platProduct.IsPlatter)
                        {
                            var p = new Product
                            {
                                AvailableStock = platProduct.AvailableStock,
                                BackgroundString = platProduct.BackgroundString,
                                CategorieId = platProduct.CategorieId,
                                Description = platProduct.Description,
                                Id = platProduct.Id,
                                IsMuchInDemand = platProduct.IsMuchInDemand,
                                Name = platProduct.Name,
                                PictureFileName = platProduct.PictureFileName,
                                PictureUri = platProduct.PictureUri,
                                Price = platProduct.Price,
                                Type = platProduct.Type,
                                Unit = platProduct.Unit
                            };
                            products.Add(p);
                        }
                        else
                        {
                            products.Add(platProduct);
                        }
                    }

                } 
            }
            
            return products;// FakeServices.Products;
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

        public bool UpdateProduct(Product product)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            //product = MapProduct.MapProductToSend(product);
            string json = JsonConvert.SerializeObject(product,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            var url = UrlConfig.ProductUrl.UpdateProduct;
            var client = new RestClient(url);
            var request = new RestRequest(Method.PUT);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", token);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
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

    }


}
