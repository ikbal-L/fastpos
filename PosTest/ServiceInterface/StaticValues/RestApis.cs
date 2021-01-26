using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ServiceInterface.StaticValues
{
    public static class RestApis
    {
        private static readonly string _scheme = "http";
        private static readonly string _host = "localhost";
        private static readonly int _port = 8080;
        private static readonly UriBuilder Builder;
        private static string _prefix = "api";
        private static string _resource;
        private static string _endpoint;

        static RestApis()
        {
            Builder = new UriBuilder
            {
                Scheme = _scheme,
                Host = _host,
                Port = _port
            };


        }

        

        public static class Resource<T>
        {
            static Resource()
            {
                
            }

            private static void SetPath([CallerMemberName] string endpoint = "",string pathVariable = "")
            {
                _resource = typeof(T).Name.ToLowerInvariant();
                _endpoint = endpoint;
                
                var path = $"{_prefix}/{_resource}/{_endpoint.ToLowerInvariant()}/{pathVariable}";
                if (string.IsNullOrEmpty(pathVariable))
                {
                   path=  path.TrimEnd('/');
                }
                Builder.Path = path;
            }
            public static string GetAll()
            {
                
                SetPath();
                return Builder.Uri.ToString();

            }

            public static string GetMany()
            {

                SetPath();
                return Builder.Uri.ToString();

            }

            public static string Save()
            {

                SetPath();
                return Builder.Uri.ToString();

            }

            public static string SaveMany()
            {

                SetPath();
                return Builder.Uri.ToString();

            }

            public static string Get<TIdentifier>(TIdentifier id)
            {

                SetPath(pathVariable: id.ToString());
                return Builder.Uri.ToString();

            }
            public static string Put<TIdentifier>(TIdentifier id)
            {

                SetPath(pathVariable:id.ToString());
                return Builder.Uri.ToString();

            }

            public static string PutMany()
            {

                SetPath();
                return Builder.Uri.ToString();

            }


            public static string Delete<TIdentifier>(TIdentifier id)
            {

                SetPath(pathVariable: id.ToString());
                return Builder.Uri.ToString();

            }



        }
    }

   
}