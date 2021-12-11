using FastPosFrontend.Configurations;
using RestSharp;
using ServiceLib.Service;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FastPosFrontend.Helpers
{
    public class ConnectionHelper
    {
        public static readonly IRestClient Client;

        public static readonly IRestRequest Ping;

        static ConnectionHelper()
        {
            var baseUrl = ConfigurationManager.Get<PosConfig>().Url;
            Client = new RestClient(baseUrl);
            Ping = new RestRequest("/", Method.GET);
        }
        public  static  bool PingHost(string hostUri="localhost", int portNumber=8080)
        {
            var res = Client.Execute(Ping);
            return res.StatusCode != 0;
        }
    }
}