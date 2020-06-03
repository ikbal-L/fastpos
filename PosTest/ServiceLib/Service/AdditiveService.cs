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
//    [Export(typeof(IAdditiveService))]
    public class AdditiveService : IAdditiveService
    {
        public bool DeleteAdditive(long idProduct)
        {
            throw new NotImplementedException();
        }

        public Additive GetAdditive(long id)
        {
            throw new NotImplementedException();
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

        public bool UpdateAdditive(Additive additive)
        {
            throw new NotImplementedException();
        }
    }

    [Export(typeof(IAdditiveService))]
    public class RestAdditiveService: IAdditiveService
    {
        private static RestAdditiveService _instance;

        public static RestAdditiveService Instance => _instance ?? (_instance = new RestAdditiveService());
        public bool DeleteAdditive(long additivdId)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(UrlConfig.AdditiveUrl.DeleteAdditive + $"{additivdId}");
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

        public Additive GetAdditive(long id)
        {
            string token = AuthProvider.Instance?.AuthorizationToken;
            var client = new RestClient(UrlConfig.AdditiveUrl.GetAdditive+id.ToString());
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", token);
            request.AddHeader("accept", "application/json");
            IRestResponse response = client.Execute(request);
            Additive additive = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                additive = JsonConvert.DeserializeObject<Additive>(response.Content);
            }

            return additive;// ;products

            //return new Additive { Id=id};
        }

        public IEnumerable<Additive> GetManyAdditives(IEnumerable<long> ids)
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

            IEnumerable<Additive> additives = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                additives = JsonConvert.DeserializeObject<IEnumerable<Additive>>(response.Content);
            }

            return additives;
        }

        public bool SaveAdditive(Additive additive)
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

            if (response.StatusCode == HttpStatusCode.OK)
                return true;

            return false;
        }

       public bool UpdateAdditive(Additive additive)
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

            if (response.StatusCode == HttpStatusCode.OK)
                return true;

            return false;
        }

        public bool SaveAdditives(IEnumerable<Additive> additives)
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

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }
    }


}
