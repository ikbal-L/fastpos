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
        public bool DeleteCategory(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _restCategoryService.GetAllCategories();
        }

        public Category GetCategory(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Category> GetManyCategories(IEnumerable<long> ids)
        {
            throw new NotImplementedException();
        }

        public bool SaveCategories(IEnumerable<Category> categories)
        {
            throw new NotImplementedException();
        }

        public bool SaveCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public bool UpdateCategory(Category category)
        {
            throw new NotImplementedException();
        }
    }

    internal class RestCategoryService
    {
        private static RestCategoryService _instance;
        public static RestCategoryService Instance => _instance ?? (_instance = new RestCategoryService());

        public bool DeleteCategory(long id)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(UrlConfig.CategoryUrl.DeleteCategory + $"{id}");
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", token);
            //request.AddParameter("application/json", "{\n\"id\":1\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public IEnumerable<Category> GetAllCategories()
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

            return categories;
        }

        //public ICollection<Category> GetAllCategories()
        //{
        //    return FakeServices.Categories;
        //}

        public Category GetCategory(long id)
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

            return category;
        }

        public IEnumerable<Category> GetManyCategories(IEnumerable<long> ids)
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

            return categories;
        }

        public bool SaveCategories(IEnumerable<Category> categories)
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

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public bool SaveCategory(Category category)
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

            if (response.StatusCode == HttpStatusCode.OK)
                return true;
            return false;

        }

        public bool UpdateCategory(Category category)
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

            if (response.StatusCode == HttpStatusCode.OK)
                return true;

            return false;
        }
    }
}
