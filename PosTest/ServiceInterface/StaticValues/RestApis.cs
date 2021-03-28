using System;
using System.Linq;
using System.Runtime.CompilerServices;
using ServiceInterface.Interface;

namespace ServiceInterface.StaticValues
{
    public  class RestApis
    {
        private static readonly string _scheme = "http";
        private static readonly string _host = "localhost";
        private static readonly int _port = 8080;
        private static UriBuilder Builder;
      
        private static string _resource;
        private static string _endpoint;
        private string _prefix = "api";

        static RestApis()
        {
            Builder = new UriBuilder
            {
                Scheme = _scheme,
                Host = _host,
                Port = _port
            };


        }

        public RestApis()
        {
            Builder = new UriBuilder
            {
                Scheme = _scheme,
                Host = _host,
                Port = _port
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

        public  string Action<T>(EndPoint endPoint, object arg = null, string subPath = "")
        {
            _resource = typeof(T).Name.ToLowerInvariant();
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
    }

    public enum EndPoint
    {
        GetAll,Get,GetMany,Save,SaveMany,Put,PutMany,Delete,DeleteMany
    }
}