using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceInterface.Authorisation;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using System;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Text;

namespace ServiceLib.Service
{
    [Export(typeof(IAuthentification))]
    public class Authentification : IAuthentification
    {
        public bool Authenticate(string user, string password, Annex annex, Terminal terminal)
        {
            string json = JsonConvert.SerializeObject(
                            new AuthUser {Username=user, Password=password, AnnexId = annex.Id, TerminalId=terminal.Id },
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            //json = $"{{\"user\" : \"{user}\", \"password\" : \"{password}\"}}";
           
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = "http://127.0.0.1:5000/auth/login";
            var client = new HttpClient();
            
            var response = client.PostAsync(url, data).Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = response.Content.ReadAsStringAsync().Result;
                var jsonContentDict = JObject.Parse(jsonContent);
                string token = jsonContentDict["auth_token"].ToString();
                AuthProvider.Initialize<DefaultAuthProvider>(new User { Token = token});
                return true;
            }
            return false;

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
