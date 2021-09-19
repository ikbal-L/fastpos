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
            _prefix = prefix;
            Builder = new UriBuilder(){Scheme = _scheme,Host = host,Port = port,};
        }

        public string Prefix
        {
            get => _prefix;
            set => _prefix = value;
        }

        public  string Resource<T>(string endPoint, object arg = null, string subPath = "",string resource = null)
        {
            _resource = resource?.ToLowerInvariant() ?? typeof(T).Name.ToLowerInvariant();
            return Resource(_resource, endPoint, arg, subPath);
        }

        public string Resource(string resource, string endPoint, object arg = null, string subPath = "")
        {
            _resource = resource;
            _endpoint = endPoint.ToString();


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

    public class EndPoint
    {
        public static readonly string GET = "Get";
        public static readonly string PUT = "Put";
        public static readonly string DELETE = "Delete";
        public static readonly string GET_ALL = "GetAll";
        public static readonly string GET_ALL_BY_CRITERIAS = "getallbycriteria";
        public static readonly string GET_MANY = "GetMany";
        public static readonly string SAVE = "Save";
        public static readonly string SAVE_MANY = "SaveMany";
        public static readonly string PUT_MANY = "PutMany";
        public static readonly string DELETE_MANY = "DeleteMany";
        public static readonly string LOGIN = "Login";
        public static readonly string LOCK = "Lock";
    }
    
}