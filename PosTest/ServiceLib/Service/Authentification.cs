using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceInterface.Authorisation;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using System;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Text;

namespace ServiceLib.Service
{
    [Export(typeof(IAuthentification))]
    public class Authentification : IAuthentification
    {
        private readonly RestAuthentification _restAuthentification = RestAuthentification.Instance;
        public int Authenticate(string user, string password, Annex annex, Terminal terminal)
        {
            int response;
            response = _restAuthentification.Authenticate(user, password, annex, terminal);
            return (int)response;

        }

    }

    public class RestAuthentification : IAuthentification
    {
        private static RestAuthentification _instance;

        internal static RestAuthentification Instance => _instance ?? (_instance = new RestAuthentification());
        public int Authenticate(string user, string password, Annex annex, Terminal terminal)  
        {
            string json = JsonConvert.SerializeObject(
                            new AuthUser {Username=user, Password=password, AnnexId = annex.Id, TerminalId=terminal.Id },
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
           
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = UrlConfig.AuthUrl.Authenticate;
            //var url = "http://127.0.0.1:5000/auth/login";
            var client = new HttpClient();
            HttpResponseMessage response;
            response = client.PostAsync(url, data).Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = response.Content.ReadAsStringAsync().Result;
                var jsonContentDict = JObject.Parse(jsonContent);
                string token = jsonContentDict["auth_token"].ToString();
                long sessionId = jsonContentDict.Value<long>("SessionId");
                AuthProvider.Initialize<DefaultAuthProvider>(new object[] { new User { }, token, sessionId});
                //return true;
            }

            return (int)response.StatusCode;
        }

    }

    class AuthUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public long AnnexId { get; set; }
        public long TerminalId { get; set; }
    }
}
