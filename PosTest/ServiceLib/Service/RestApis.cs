using System;
using ServiceInterface;
using ServiceInterface.Interface;

namespace ServiceLib.Service
{
    public  class RestApis
    {
        private static readonly string _scheme = "http";
        private static readonly string Host = "localhost";
        private static readonly int Port = 8080;
        private static UriBuilder Builder;
      
        private static string _resource;
        private static string _endpoint;
        private string _prefix = "api";

        static RestApis()
        {
            
            ;
            if (AppConfigurationManager.ContainsKey(nameof(Host)))
            {
                var values = (AppConfigurationManager.Configuration(nameof(Host)) as string)?.Split(':');
                Host = values?[0];
                Port = int.Parse(values?[1]!);
            }
            Builder = new UriBuilder
            {
                Scheme = _scheme,
                Host = Host,
                Port = Port
            };


        }

        public RestApis()
        {
            
            Builder = new UriBuilder
            {
                Scheme = _scheme,
                Host = Host,
                Port = Port
            };
        }
        public RestApis(string host,int port,string prefix)
        {
            this._prefix = prefix;
            Builder = new UriBuilder(){Scheme = _scheme,Host = host,Port = port,};
        }

        public string Prefix
        {
            get => _prefix;
            set => _prefix = value;
        }

        public  string Action<T>(EndPoint endPoint, object arg = null, string subPath = "",string resource = null)
        {
            _resource = resource?.ToLowerInvariant() ?? typeof(T).Name.ToLowerInvariant();
            return Action(_resource, endPoint, arg, subPath);
        }

        public string Action(string resource, EndPoint endPoint, object arg = null, string subPath = "")
        {
            _resource = resource;
            _endpoint = endPoint.ToString();

            //var path = $"{_prefix}/{_resource}/{_endpoint.ToLowerInvariant()}/{pathVariable}";

            IPathBuilder builder = Path.Create(_prefix).SubPath(_resource).SubPath(_endpoint);
            if (arg != null)
            {
                builder.Arg(arg);
            }

            if (!string.IsNullOrEmpty(subPath))
            {
                builder.SubPath(subPath);
            }

            IPath path = builder.Build();
            Builder.Path = path.ToString();
            return Builder.Uri.ToString();
        }

        public string Action(string endPoint, object arg = null, string subPath = "")
        {

            _endpoint = endPoint;



            IPathBuilder builder = Path.Create(_endpoint);
            if (arg != null)
            {
                builder.Arg(arg);
            }

            if (!string.IsNullOrEmpty(subPath))
            {
                builder.SubPath(subPath);
            }

            IPath path = builder.Build();
            Builder.Path = path.ToString();
            return Builder.Uri.ToString();
        }
    }

    public enum EndPoint
    {
        GetAll,Get,GetMany,Save,SaveMany,Put,PutMany,Delete,DeleteMany,Login
    }
}