﻿using RestSharp;
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
        private ProductService _productService = new ProductService();
        public int DeleteCategory(long id)
        {
            return _restCategoryService.DeleteCategory(id);
        }

        public (int,IEnumerable<Category>) GetAllCategories()
        {
            var code = 0;
            var (statusCode,categories) = _restCategoryService.GetAllCategories();
            if (statusCode != 200)
            {
                return (statusCode,null);
            }
            var products = _restProductService.GetAllProducts(ref code);
            if (code != 200)
            {
                throw new MappingException("Error in GetAllProducts to map with products, StatusCode: " + code.ToString());
            }
            categories.ToList().ForEach(c => c.MappingAfterReceiving(ref products));
            return (Status:statusCode,Categories:categories);
        }
        public ((int,int),(IEnumerable<Category>, IEnumerable<Product>)) GetAllCategoriesAndProducts()
        {
            int prodStatusCode = 0;
            var (categStatusCode,categories) = _restCategoryService.GetAllCategories();
           
            var products = _productService.GetAllProducts(ref prodStatusCode);

            if (categStatusCode != 200 && prodStatusCode!=200)
            {
                return ((categStatusCode,prodStatusCode),(null, null));
            }
            categories.ToList().ForEach(c => c.MappingAfterReceiving(ref products));

            var tuple = (categories, products);
            return ((categStatusCode,prodStatusCode),(categories, products));
        }

        public (int, Category) GetCategory(long id)
        {
            
            var (statusCode,category) = _restCategoryService.GetCategory(id);
            if (statusCode != 200)
            {
                return (statusCode,null);
            }
            int getProductStatusCode=0;
            var products = _restProductService.GetManyProducts(category.ProductIds, ref getProductStatusCode);

            if (getProductStatusCode != 200)
            {
                throw new MappingException("Error in GetManyProducts to map with products, StatusCode: " + getProductStatusCode.ToString());
            }
            category.MappingAfterReceiving(ref products);
            return (statusCode,category);

        }

        public (int, IEnumerable<Category>) GetManyCategories(IEnumerable<long?> ids)
        {
            throw new NotImplementedException();
        }

        public int SaveCategories(IEnumerable<Category> categories)
        {
            categories.ToList().ForEach(c => c.MappingBeforeSending());
            return _restCategoryService.SaveCategories(categories);
        }

        public int SaveCategory(Category category,out IEnumerable<string> errors)
        {
            category.MappingBeforeSending();
            errors = ValidationService.Validate(category);
            if (errors.Any())
            {
                //id = -1;
                return 0;
            }
            return GenericRest.SaveThing(category,UrlConfig.CategoryUrl.SaveCategory, out errors).Item1;
            // return _restCategoryService.SaveCategory(category, ref id);
        }

        public int UpdateCategory(Category category,out IEnumerable<string> errors)
        {
            category.MappingBeforeSending();
            errors = ValidationService.Validate(category);
            if (errors.Any()) return 0;
            return _restCategoryService.UpdateCategory(category,out errors);
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

        public (int Status,IEnumerable<Category> Categories) GetAllCategories()
        {
            string token = AuthProvider.Instance?.AuthorizationToken;
            var client = new RestClient(UrlConfig.CategoryUrl.GetAllCategories);
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Authorization", token);
            request.AddHeader("accept", "application/json");
            request.AddHeader("Annex-Id", AuthProvider.Instance.AnnexId.ToString());
            IRestResponse response = client.Execute(request);
            IEnumerable<Category> categories = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                categories = JsonConvert.DeserializeObject<IEnumerable<Category>>(response.Content);
            }
             
            return ((int)response.StatusCode,categories);
        }

        public ((int, int), (IEnumerable<Category>, IEnumerable<Product>)) GetAllCategoriesAndProducts()
        {
            throw new NotImplementedException();
        }

        //public ICollection<Category> GetAllCategories()
        //{
        //    return FakeServices.Categories;
        //}

        public (int, Category) GetCategory(long id)
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
            
            return ((int)response.StatusCode, category);
        }

        public (int,IEnumerable<Category>) GetManyCategories(IEnumerable<long?> ids)
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
            return ((int)response.StatusCode,categories);
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

        public int SaveCategory(Category category , out IEnumerable<string> errors)
        {
            
            errors = new List<string>();
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

            if (response.StatusCode == HttpStatusCode.OK)
            {
                long.TryParse(response.Content,out long id);
                category.Id = id;
            }
            return (int)response.StatusCode;

        }

        public int UpdateCategory(Category category, out IEnumerable<string> errors)
        {
            errors = new List<string>();
            string token = AuthProvider.Instance.AuthorizationToken;
            //product = MapProduct.MapProductToSend(product);
            string json = JsonConvert.SerializeObject(category,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            var url = UrlConfig.CategoryUrl.UpdateCategory+category.Id;
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
