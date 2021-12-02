using Newtonsoft.Json;
using ServiceInterface.Authorisation;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Converters;
using System.Security.Principal;
using System.Threading;

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

        public HttpResponseMessage Authenticate(string user, string password, Terminal terminal, Annex annex)
        {
            
            var response = _restAuthentification.Authenticate(user, password, terminal, annex );
            return response;
        }

        public int Authenticate(User user)
        {
            throw new NotImplementedException();
        }
    }

    public class RestAuthentification : IAuthentification
    {
        private static RestAuthentification _instance;

        internal static RestAuthentification Instance => _instance ?? (_instance = new RestAuthentification());
        public int Authenticate(string user, string password, Annex annex, Terminal terminal)  
        {
            string json = JsonConvert.SerializeObject(
                            new AuthUser {Username=user, Password=password, TerminalId=terminal.Id },
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
           
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = UrlConfig.AuthUrl.Authenticate;

            var client = new HttpClient();
            HttpResponseMessage response;
            response = client.PostAsync(url, data).Result;

            if (response.IsSuccessStatusCode)
            {
                response.Headers.TryGetValues("Authorization", out IEnumerable<string> values);
                var token = values.First();
                AuthProvider.Initialize<DefaultAuthProvider>(new object[] { new User { }, token, annex.Id});
                var jsonContent = response.Content.ReadAsStringAsync().Result;
                try
                {
                    if (!string.IsNullOrEmpty(jsonContent))
                    {
                        var permissions = JsonConvert.DeserializeObject<List<String>>(jsonContent);
                        var principal = new GenericPrincipal(new GenericIdentity("UserTest", ""), permissions.ToArray());
                        Thread.CurrentPrincipal = principal; 
                    }
                }
                catch (Exception)
                {
                    return -400;
                }

            }

            return (int)response.StatusCode;
        }


        public HttpResponseMessage Authenticate(string user, string password,  Terminal terminal, Annex annex)
        {
            string json = JsonConvert.SerializeObject(
                new AuthUser { Username = user, Password = password, TerminalId = terminal.Id },
                Newtonsoft.Json.Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var api = new RestApis();
            
            var url = api.Action("login");

            var client = new HttpClient();
            HttpResponseMessage response;
            response = client.PostAsync(url, data).Result;

            if (response.IsSuccessStatusCode)
            {
                response.Headers.TryGetValues("Authorization", out IEnumerable<string> values);
                var token = values.First();
                AuthProvider.Initialize<DefaultAuthProvider>(new object[] { new User { }, token, annex.Id });
                var jsonContent = response.Content.ReadAsStringAsync().Result;
                var hasSessionId = response.Headers.TryGetValues("user-meta-session-id", out var sessionId);
                try
                {
                    if (!string.IsNullOrEmpty(jsonContent))
                    {
                        var permissions = JsonConvert.DeserializeObject<List<string>>(jsonContent);
                        var name = "";
                        if (hasSessionId)
                        {
                            name = sessionId.First();
                        }
                        var principal = new GenericPrincipal(new GenericIdentity(name), permissions.ToArray());
                        
                        Thread.CurrentPrincipal = principal;
                    }
                }
                catch (Exception)
                {
                    return response;
                }

            }

            return response;
        }
        public int Authenticate(User user)
        {
            string json = JsonConvert.SerializeObject(
                new AuthUser { Username = user.Username, Password = user.Password, TerminalId = 1,Agent = user.Agent},
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = UrlConfig.AuthUrl.Authenticate;
            var client = new HttpClient();
            HttpResponseMessage response;
            response = client.PostAsync(url, data).Result;

            if (response.IsSuccessStatusCode)
            {
                response.Headers.TryGetValues("Authorization", out IEnumerable<string> values);
                var token = values.First();
             
                AuthProvider.Initialize<DefaultAuthProvider>(new object[] { new User { }, token, 1 });

            }

            return (int)response.StatusCode;
        }
    }

    class AuthUser
    {
        
        public string Username { get; set; }
        
        public string Password { get; set; }

        [JsonProperty]
        public long TerminalId { get; set; }
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public Agent Agent { get; set; }
    }
}
