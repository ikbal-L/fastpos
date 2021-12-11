using System;
using ServiceInterface;
using ServiceInterface.Interface;

namespace ServiceLib.Service
{
    public class RestApi : IRestApi
    {
        private static string _resource;
        private static string _endpoint;
        public RestApi(string basUrl = "")
        {
            if (!string.IsNullOrWhiteSpace(basUrl)) BaseUrl = basUrl;

        }

        public string BaseUrl { get; set; } = "http://localhost:8080";

        public string Prefix { get; set; } = "api";

        public string Resource<T>(string endPoint, object arg = null, string subPath = "", string resource = null)
        {

            _resource = resource?.ToLowerInvariant() ?? typeof(T).Name.ToLowerInvariant();
            return Resource(_resource, endPoint, arg, subPath);
        }

        public string Resource(string resource, string endPoint, object arg = null, string subPath = "")
        {
            _resource = resource;
            _endpoint = endPoint.ToString();


            IPathBuilder builder = Path.Create(Prefix).SubPath(_resource).SubPath(_endpoint);
            if (arg != null)
            {
                builder.Arg(arg);
            }

            if (!string.IsNullOrEmpty(subPath))
            {
                builder.SubPath(subPath);
            }

            IPath path = builder.Build();

            return BaseUrl + path.ToString();
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
            return BaseUrl + path.ToString();
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