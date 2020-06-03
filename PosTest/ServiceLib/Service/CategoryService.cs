using RestSharp;
using ServiceInterface.Authorisation;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Net;

using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ServiceLib.Service
{
    [Export(typeof(ICategoryService))]
    class CategoryService : ICategoryService
    {
        private RestCategoryService _restCategoryService = RestCategoryService.Instance;
        private RestProductService _restProductService = RestProductService.Instance;
        public int DeleteCategory(long id)
        {
            return _restCategoryService.DeleteCategory(id);
        }

        public IEnumerable<Category> GetAllCategories(ref int statusCode)
        {
            var code = 0;
            var categories = _restCategoryService.GetAllCategories(ref statusCode);
            if (statusCode != 200)
            {
                return null;
            }
            var products = _restProductService.GetAllProducts(ref code);
            if (code != 200)
            {
                throw new MappingException("Error in GetAllProducts to map with products, StatusCode: " + code.ToString());
            }
            categories.ToList().ForEach(c => c.MappingAfterReceiving(products));
            return categories;
        }

        public Category GetCategory(long id, ref int statusCode)
        {
            Category category;
            category = _restCategoryService.GetCategory(id, ref statusCode);
            if (statusCode != 200)
            {
                return null;
            }
            int getProductStatusCode=0;
            var products = _restProductService.GetManyProducts(category.ProductIds, ref getProductStatusCode);

            if (getProductStatusCode != 200)
            {
                throw new MappingException("Error in GetManyProducts to map with products, StatusCode: " + getProductStatusCode.ToString());
            }
            category.MappingAfterReceiving(products);
            return category;

        }

        public IEnumerable<Category> GetManyCategories(IEnumerable<long> ids, ref int statusCode)
        {
            throw new NotImplementedException();
        }

        public int SaveCategories(IEnumerable<Category> categories)
        {
            throw new NotImplementedException();
        }

        public int SaveCategory(Category category)
        {
            category.MappingBeforeSending();
            return _restCategoryService.SaveCategory(category);
        }

        public int UpdateCategory(Category category)
        {
            category.MappingBeforeSending();
            return _restCategoryService.SaveCategory(category);
        }
    }

    internal class RestCategoryService : ICategoryService
    {
        private static RestCategoryService _instance;
        public static RestCategoryService Instance => _instance ?? (_instance = new RestCategoryService());

        public int DeleteCategory(long id)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(UrlConfig.CategoryUrl.DeleteCategory + $"{id}");
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", token);
            //request.AddParameter("application/json", "{\n\"id\":1\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    return true;
            //}
            return (int)response.StatusCode;
        }

        public IEnumerable<Category> GetAllCategories(ref int statusCode)
        {
            string token = AuthProvider.Instance?.AuthorizationToken;
            var client = new RestClient(UrlConfig.CategoryUrl.GetAllCategories);
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", token);
            request.AddHeader("accept", "application/json");
            IRestResponse response = client.Execute(request);
            IEnumerable<Category> categories = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                categories = JsonConvert.DeserializeObject<IEnumerable<Category>>(response.Content);
            }
            statusCode = (int)response.StatusCode;
            return categories;
        }

        //public ICollection<Category> GetAllCategories()
        //{
        //    return FakeServices.Categories;
        //}

        public Category GetCategory(long id, ref int StatusCode)
        {
            string token = AuthProvider.Instance?.AuthorizationToken;
            var client = new RestClient(UrlConfig.CategoryUrl.GetCategory + id.ToString());
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", token);
            request.AddHeader("accept", "application/json");
            IRestResponse response = client.Execute(request);
            Category category = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                category = JsonConvert.DeserializeObject<Category>(response.Content);
            }
            StatusCode = (int)response.StatusCode;
            return category;
        }

        public IEnumerable<Category> GetManyCategories(IEnumerable<long> ids, ref int StatusCode)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(UrlConfig.CategoryUrl.GetManyCategories);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", token);
            request.AddHeader("content-type", "application/json");
            string json = JsonConvert.SerializeObject(ids,
                           Newtonsoft.Json.Formatting.None,
                           new JsonSerializerSettings
                           {
                               NullValueHandling = NullValueHandling.Ignore
                           });
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            IEnumerable<Category> categories = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                categories = JsonConvert.DeserializeObject<IEnumerable<Category>>(response.Content);
            }
            StatusCode = (int)response.StatusCode;
            return categories;
        }

        public int SaveCategories(IEnumerable<Category> categories)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(UrlConfig.CategoryUrl.SaveCategories);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", token);
            //foreach (var p in products)
            //{
            //    MapProduct.MapProductToSend(p);
            //}
            var jsonData = JsonConvert.SerializeObject(categories,
                                        Newtonsoft.Json.Formatting.None,
                                        new JsonSerializerSettings
                                        {
                                            NullValueHandling = NullValueHandling.Ignore
                                        });
            //Console.WriteLine(jsonData);
            request.AddParameter("application/json", jsonData, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    return true;
            //}
            return (int)response.StatusCode; ;
        }

        public int SaveCategory(Category category)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            //product = MapProduct.MapProductToSend(product);
            string json = JsonConvert.SerializeObject(category,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            var url = UrlConfig.CategoryUrl.SaveCategory;
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", token);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //    return true;
            return (int)response.StatusCode;

        }

        public int UpdateCategory(Category category)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            //product = MapProduct.MapProductToSend(product);
            string json = JsonConvert.SerializeObject(category,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            var url = UrlConfig.CategoryUrl.UpdateCategory;
            var client = new RestClient(url);
            var request = new RestRequest(Method.PUT);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", token);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //    return true;
            return (int)response.StatusCode;
        }

        int ICategoryService.SaveCategories(IEnumerable<Category> categories)
        {
            return ((ICategoryService)Instance).SaveCategories(categories);
        }
    }
}
