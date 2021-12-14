using Newtonsoft.Json;
using RestSharp;
using ServiceInterface.Authorisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLib.Service
{
    public class RestService
    {
        public static readonly Dictionary<string, string> SHARED_HEADERS =new() 
        { 
            { "authorization", AuthProvider.Instance?.AuthorizationToken },
            { "accept", "application/json" },
            { "Annex-Id", $"{AuthProvider.Instance?.AnnexId}" },
        };

        public static readonly JsonSerializerSettings Json_Serializer_Settings = new JsonSerializerSettings() 
        {
            NullValueHandling = NullValueHandling.Include,
            DateTimeZoneHandling = DateTimeZoneHandling.Unspecified,
        };

        public static readonly RestService Instance = new RestService();

        private RestService()
        {

        }

        public IRestClient Client { get; set; } = new RestClient();

        public virtual async Task<IRestResponse> GetAsync(string path)
        {
            var request = new RestRequest(new Uri(path),Method.GET);
            request.AddHeaders(SHARED_HEADERS);
            var response = await Client.ExecuteAsync(request);
            return response;
        }

        public IRestResponse RestPost(object objecToPost, string url)
        {
            var request = new RestRequest(new Uri(url),Method.POST);
            request.AddHeaders(SHARED_HEADERS);
            string json = JsonConvert.SerializeObject(objecToPost,Json_Serializer_Settings);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = Client.Execute(request);
            return response;
        }
        public  IRestResponse Put(object objecToPost, string url)
        {
            var request = new RestRequest(new Uri(url),Method.PUT);
            request.AddHeaders(SHARED_HEADERS);

            string json = JsonConvert.SerializeObject(objecToPost,Json_Serializer_Settings);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = Client.Execute(request);
            return response;
        }


        private  IRestResponse Delete(string path, object payload = null)
        {
            var request = new RestRequest(new Uri(path),Method.DELETE);
            request.AddHeaders(SHARED_HEADERS);
            if (payload != null)
            {
                var json = JsonConvert.SerializeObject(payload,Json_Serializer_Settings);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
            }

            IRestResponse response = Client.Execute(request);
            return response;
        }



        public async Task<(int status, R resource)> GetResourceAsync<R>(string uri)
        {
            var response = await GetAsync(uri);
            R t = response.IsSuccessful? JsonConvert.DeserializeObject<R>(response.Content):default;
            return ((int)response.StatusCode, t);
        }


    }
}
