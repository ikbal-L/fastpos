using RestSharp;
using ServiceInterface.Authorisation;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ServiceLib.Service
{
    public class GenericRest
    {
        public static (int status,T) GetThing<T>(string path)
        {
            var resp = RestGet(path);
            T t = default;
            if (resp.StatusCode == HttpStatusCode.OK)
            {

                t = JsonConvert.DeserializeObject<T>(resp.Content);
            }
            return ((int)resp.StatusCode,t);// ;products

        }

        public static async Task<(int status, T)> GetThingAsync<T>(string path)
        {
            var resp = await RestGetAsync(path);
            T t = default;
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                //string s = t.ToString();
                //                if (t.ToString() is Waiter)
                //                  Console.WriteLine(s);
                t = JsonConvert.DeserializeObject<T>(resp.Content);
            }
            return ((int)resp.StatusCode, t);// ;products

        }

        public static IRestResponse RestGet(string path,string? payload = null)
        {
            string token = AuthProvider.Instance?.AuthorizationToken;
            var client = new RestClient(path);
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", token);
            request.AddHeader("accept", "application/json");
            request.AddHeader("Annex-Id", $"{AuthProvider.Instance?.AnnexId}");
            if (!string.IsNullOrEmpty(payload))
            {
                request.AddParameter("application/json", payload, ParameterType.RequestBody);
            }
            IRestResponse response = client.Execute(request);
            return response;
        }

        public static async Task<IRestResponse> RestGetAsync(string path)
        {
            string token = AuthProvider.Instance?.AuthorizationToken;
            var client = new RestClient(path);
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", token);
            request.AddHeader("accept", "application/json");
            request.AddHeader("Annex-Id", $"{AuthProvider.Instance?.AnnexId}");
            var response = await client.ExecuteAsync(request);
            return response;
        }

        public static IRestResponse SaveThing<T>(T thing, string url)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            string json = JsonConvert.SerializeObject(thing,
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                
                            });

            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", token);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            //request.AddHeader("Annex-Id", AuthProvider.Instance.AnnexId.ToString());
            
            IRestResponse response = client.Execute(request);
            return response;
        }

        public static (int status, T result) SaveThing<T>(T thing, string url, out IEnumerable<string> errors)
        {
            errors = new List<string>();
            var response = SaveThing<T>(thing, url);
            if (response.StatusCode == HttpStatusCode.Created)
            {

                long.TryParse(response.Content, out long id);
                thing.GetType().GetProperty("Id")?.SetValue(thing, id);

            }

            if ((int)response.StatusCode == 422)
            {
                errors = JsonConvert.DeserializeObject<IEnumerable<string>>(response.Content);
               
            }

            return ((int)response.StatusCode, thing);
        }
        public static (bool, TReturn) SaveThing<T,TReturn>(T thing, string url)
        {
            var response = SaveThing<T>(thing, url);
            if (response.StatusCode == HttpStatusCode.OK||response.StatusCode==HttpStatusCode.Created)
            {
                return(true, JsonConvert.DeserializeObject<TReturn>(response.Content));
                
            }
            return (false,default(TReturn));
        }


        public static (int Status ,IEnumerable<T> result) GetManyThings<T>(IEnumerable<long> ids, string url)
        {
            
            IRestResponse response = RestPost(ids, url);
            

            IEnumerable<T> things = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                things = JsonConvert.DeserializeObject<IEnumerable<T>>(response.Content);
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                things =  Array.Empty<T>();
            }
            
            return ((int)response.StatusCode, things);
        }

        public static IRestResponse RestPost(object objecToPost, string url)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", token);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Annex-Id", AuthProvider.Instance.AnnexId.ToString());
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
        public static IRestResponse RestPut(object objecToPost, string url)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(url);
            var request = new RestRequest(Method.PUT);
            request.AddHeader("authorization", token);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Annex-Id", AuthProvider.Instance.AnnexId.ToString());
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

        public static (int status, string) UpdateThing<T>(T t, string url, out IEnumerable<string> errors)
        {
            errors = null;
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
            request.AddHeader("Annex-Id", AuthProvider.Instance.AnnexId.ToString());
            IRestResponse response = client.Execute(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //    return true;
            return ((int)response.StatusCode, response.Content);
        }
        public static (bool, TReturn) UpdateThing<T,TReturn>(T t, string url) {

            IRestResponse response = RestPut(t, url);
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                return (true, JsonConvert.DeserializeObject<TReturn>(response.Content));
            }
            return (false, default(TReturn));
        }
        public static IRestResponse UpdateThing<T>(T t, string url)
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

            return response;
        }

        private static IRestResponse RestDelete(string path,object payload = null)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(path);
            var request = new RestRequest(Method.DELETE);
            if (payload!= null)
            {
                var json = JsonConvert.SerializeObject(payload,
                       Newtonsoft.Json.Formatting.None,
                       new JsonSerializerSettings
                       {
                           NullValueHandling = NullValueHandling.Ignore
                       });
                request.AddParameter("application/json", json, ParameterType.RequestBody); 
            }
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", token);
            IRestResponse response = client.Execute(request);
            return response;
        }

        public static (int,IEnumerable<T>) GetAll<T>(string url)
        {
            IRestResponse response = RestGet( url);

            IEnumerable<T> collection = null;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                collection = JsonConvert.DeserializeObject<IEnumerable<T>>(response.Content);
            }

            if (response.StatusCode== HttpStatusCode.NoContent)
            {
                collection = new List<T>();
            }
            
            return ((int)response.StatusCode,collection);
        }

        public static (int status,IEnumerable<string> errors) Delete<T>(string url)
        {
            var resp = RestDelete(url);

            if (resp.StatusCode!= HttpStatusCode.OK)
            {
                var errors = JsonConvert.DeserializeObject<IEnumerable<string>>(resp.Content);
                return ((int) resp.StatusCode, errors);
            }
            return ((int)resp.StatusCode,default);
        }
        public static (int status, T) PostThing<T>(string path,object postObject)
        {
            var resp = RestPost(postObject,path);
            T t = default;
            if (resp.StatusCode == HttpStatusCode.OK|| resp.StatusCode == HttpStatusCode.Created)
            {
       
                t = JsonConvert.DeserializeObject<T>(resp.Content);
            }
            return ((int)resp.StatusCode, t);// ;products

        }

        public static (int status, T) UpdateThing<T>(string path, object postObject)
        {
            var resp = RestPut(postObject, path);
            T t = default;
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                //string s = t.ToString();
                //                if (t.ToString() is Waiter)
                //                  Console.WriteLine(s);
                t = JsonConvert.DeserializeObject<T>(resp.Content);
            }
            return ((int)resp.StatusCode, t);// ;products

        }

        public static int DeleteThing(string url, object payload = null)
        {

            IRestResponse response = RestDelete(url, payload);
           
            return (int)response.StatusCode;
        }
        public static (bool, TReturn) DeleteThing<TReturn>(string url,object payload = null)
        {

            IRestResponse response =RestDelete(url,payload);
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                return (true, JsonConvert.DeserializeObject<TReturn>(response.Content));
            }
            return (false, default(TReturn));
        }
    }
}
