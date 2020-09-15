using RestSharp;
using ServiceInterface.Authorisation;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using ServiceInterface.Model;
using System;

namespace ServiceLib.Service
{
    public class GenericRest
    {
        static public T GetThing<T>(string path, ref int statusCode)
        {
            var resp = RestGet(path);
            statusCode = (int)resp.StatusCode;
            T t = default;
            if (statusCode == (int)HttpStatusCode.OK)
            {
                //string s = t.ToString();
//                if (t.ToString() is Waiter)
  //                  Console.WriteLine(s);
                t = JsonConvert.DeserializeObject<T>(resp.Content);
            }
            return t;// ;products

        }

        public static IRestResponse RestGet(string path)
        {
            string token = AuthProvider.Instance?.AuthorizationToken;
            var client = new RestClient(path);
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", token);
            request.AddHeader("accept", "application/json");
            IRestResponse response = client.Execute(request);
            return response;
        }

        public static int SaveThing<T>(T thing, string url)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            //product = MapProduct.MapProductToSend(product);
            string json = JsonConvert.SerializeObject(thing,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", token);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            //if (response.StatusCode == HttpStatusCode.OK)
            //    return true;
            return (int)response.StatusCode;
        }

        public static IEnumerable<T> GetManyThings<T>(IEnumerable<long> ids, string url, ref int statusCode)
        {
            
            IRestResponse response = RestPost(ids, url);
            statusCode = (int)response.StatusCode;

            IEnumerable<T> things = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                things = JsonConvert.DeserializeObject<IEnumerable<T>>(response.Content);
            }
            statusCode = (int)response.StatusCode;
            return things;
        }

        public static IRestResponse RestPost(object objecToPost, string url)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", token);
            request.AddHeader("content-type", "application/json");
            string json = JsonConvert.SerializeObject(objecToPost,
                           Newtonsoft.Json.Formatting.None,
                           new JsonSerializerSettings
                           {
                               NullValueHandling = NullValueHandling.Ignore
                           });
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }

        public static int UpdateThing<T>(T t, string url)
        {
            string token = AuthProvider.Instance.AuthorizationToken;

            string json = JsonConvert.SerializeObject(t,
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            var client = new RestClient(url);
            var request = new RestRequest(Method.PUT);
            request.AddHeader("authorization", token);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //    return true;

            return (int)response.StatusCode;
        }

        public IRestResponse RestDelete(string path)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(path);
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", token);
            IRestResponse response = client.Execute(request);
            return response;
        }
    }
}
