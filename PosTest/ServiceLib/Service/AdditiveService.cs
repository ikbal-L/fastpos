using Newtonsoft.Json;
using RestSharp;
using ServiceInterface.Authorisation;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ServiceLib.Service
{
    public class AdditiveService : IAdditiveService
    {
        private readonly RestAdditiveService _restAdditiveService = RestAdditiveService.Instance;
        public int DeleteAdditive(long id)
        {
            return _restAdditiveService.DeleteAdditive(id);
        }

        public Additive GetAdditive(long id, ref int statusCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Additive> GetManyAdditives(IEnumerable<long> ids, ref int statusCode)
        {
            throw new NotImplementedException();
        }

        public int SaveAdditive(Additive additive)
        {
            throw new NotImplementedException();
        }

        public int SaveAdditives(IEnumerable<Additive> additives)
        {
            throw new NotImplementedException();
        }

        public int UpdateAdditive(Additive additive)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Additive> GetAllAdditives(ref int statusCode)
        {
            throw new NotImplementedException();
        }
    }

    [Export(typeof(IAdditiveService))]
    public class RestAdditiveService : IAdditiveService
    {
        private static RestAdditiveService _instance;

        public static RestAdditiveService Instance => _instance ?? (_instance = new RestAdditiveService());
        public int DeleteAdditive(long additivdId)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(UrlConfig.AdditiveUrl.DeleteAdditive + $"{additivdId}");
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

        public Additive GetAdditive(long id, ref int statusCode)
        {
            string token = AuthProvider.Instance?.AuthorizationToken;
            var client = new RestClient(UrlConfig.AdditiveUrl.GetAdditive+id.ToString());
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", token);
            request.AddHeader("accept", "application/json");
            IRestResponse response = client.Execute(request);
            Additive additive = null;
            statusCode = (int)response.StatusCode;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                additive = JsonConvert.DeserializeObject<Additive>(response.Content);
            }

            return additive;// ;products

            //return new Additive { Id=id};
        }

        public IEnumerable<Additive> GetManyAdditives(IEnumerable<long> ids, ref int statusCode)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(UrlConfig.AdditiveUrl.GetManyAdditives);
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
            statusCode = (int)response.StatusCode;

            IEnumerable<Additive> additives = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                additives = JsonConvert.DeserializeObject<IEnumerable<Additive>>(response.Content);
            }

            return additives;
        }

        public int SaveAdditive(Additive additive)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            //product = MapProduct.MapProductToSend(product);
            string json = JsonConvert.SerializeObject(additive,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            var url = UrlConfig.AdditiveUrl.SaveAdditive;
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

       public int UpdateAdditive(Additive additive)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            //product = MapProduct.MapProductToSend(product);
            string json = JsonConvert.SerializeObject(additive,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            var url = UrlConfig.AdditiveUrl.UpdateAdditive;
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

        public int SaveAdditives(IEnumerable<Additive> additives)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(UrlConfig.AdditiveUrl.SaveAdditives);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", token);
            //foreach (var p in products)
            //{
            //    MapProduct.MapProductToSend(p);
            //}
            var jsonData = JsonConvert.SerializeObject(additives,
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
            //    return true;

            return (int)response.StatusCode;
        }

        public IEnumerable<Additive> GetAllAdditives(ref int statusCode)
        {
            string token = AuthProvider.Instance?.AuthorizationToken;
            var client = new RestClient(UrlConfig.AdditiveUrl.GetAllAdditives);
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", token);
            request.AddHeader("accept", "application/json");
            IRestResponse response = client.Execute(request);
            IEnumerable<Additive> additives = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                additives = JsonConvert.DeserializeObject<IEnumerable<Additive>>(response.Content);
            }
            statusCode = (int)response.StatusCode;
            return additives;
        }
    }


}
