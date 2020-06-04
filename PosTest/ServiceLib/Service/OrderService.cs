using RestSharp;
using ServiceInterface.Authorisation;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Newtonsoft.Json.Linq;

namespace ServiceLib.Service
{
    [Export(typeof(IOrderService))]
    public class OrderService : IOrderService
    {
        private readonly RestOrderService _restOrderService = RestOrderService.Instance;
        public int DeleteOrder(long orderId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Order> GetAllOrders(ref int statusCode)
        {
            throw new NotImplementedException();
        }

        public long? GetIdmax(ref int statusCode)
        {

            var idmax = _restOrderService.GetIdmax(ref statusCode);
            /*if (idmax==null)
            {
                throw new ArgumentNullException("idmax is null");
            }*/
            return idmax;
        }

        public IEnumerable<Order> GetManyOrders(IEnumerable<long> orderIds, ref int statusCode)
        {
            throw new NotImplementedException();
        }

        public int SaveOrder(Order order)
        {
            order.MappingBeforeSending();
            return _restOrderService.SaveOrder(order);
        }

        public int UpdateOrder(Order order)
        {
            throw new NotImplementedException();
        }
    }

    internal class RestOrderService : IOrderService
    {
        private static RestOrderService _instance;

        internal static RestOrderService Instance => _instance ?? (_instance = new RestOrderService());
        public int DeleteOrder(long orderId)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(UrlConfig.OrderUrl.DeleteOrder + $"{orderId}");
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", token);
            //request.AddParameter("application/json", "{\n\"id\":1\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    return true;
            //}
            return (int)response.StatusCode;
        }

        public Order GetOrder(long id, ref int statusCode)
        {
            string token = AuthProvider.Instance?.AuthorizationToken;
            var client = new RestClient(UrlConfig.OrderUrl.GetOrder+id.ToString());
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", token);
            request.AddHeader("accept", "application/json");
            IRestResponse response = client.Execute(request);
            Order order = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                order = JsonConvert.DeserializeObject<Order>(response.Content);
            }
            statusCode = (int)response.StatusCode;
            return order;// ;products

            //return new Additive { Id=id};
        }

        public IEnumerable<Order> GetManyOrders(IEnumerable<long> ids, ref int statusCode)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(UrlConfig.OrderUrl.GetManyOrders);
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", token);
            request.AddHeader("content-type", "application/json");
            string json = JsonConvert.SerializeObject(ids,
                           Newtonsoft.Json.Formatting.None,
                           new JsonSerializerSettings
                           {
                               NullValueHandling = NullValueHandling.Ignore
                           });
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            IEnumerable<Order> orders = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                orders = JsonConvert.DeserializeObject<IEnumerable<Order>>(response.Content);
            }
            statusCode = (int)response.StatusCode;
            return orders;
        }

        public int SaveOrder(Order Order)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            //product = MapProduct.MapProductToSend(product);
            string json = JsonConvert.SerializeObject(Order,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            var url = UrlConfig.OrderUrl.SaveOrder;
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", token);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            //if (response.StatusCode == HttpStatusCode.OK)
            //    return true;
            return (int)response.StatusCode;
        }

        public int UpdateOrder(Order order)
        {
            string token = AuthProvider.Instance.AuthorizationToken;

            string json = JsonConvert.SerializeObject(order,
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            var url = UrlConfig.OrderUrl.UpdateOrder;
            var client = new RestClient(url);
            var request = new RestRequest(Method.PUT);
            request.AddHeader("authorization", token);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //    return true;

            return (int)response.StatusCode;
        }

        public int SaveOrders(IEnumerable<Order> orders)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(UrlConfig.OrderUrl.SaveOrders);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", token);
            //foreach (var p in products)
            //{
            //    MapProduct.MapProductToSend(p);
            //}
            var jsonData = JsonConvert.SerializeObject(orders,
                                        Newtonsoft.Json.Formatting.None,
                                        new JsonSerializerSettings
                                        {
                                            NullValueHandling = NullValueHandling.Ignore
                                        });
            //Console.WriteLine(jsonData);
            request.AddParameter("application/json", jsonData, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);

            //if (response.StatusCode == HttpStatusCode.OK)
            //    return true;

            return (int)response.StatusCode;
        }

        public long? GetIdmax(ref int statusCode)
        {
            string token = AuthProvider.Instance?.AuthorizationToken;
            var client = new RestClient(UrlConfig.OrderUrl.GetIdMax);
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", token);
            request.AddHeader("accept", "application/json");
            IRestResponse response = client.Execute(request);
            statusCode = (int)response.StatusCode;
            long? idmax = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                //var jsonContentDict = JObject.Parse(response.Content);
                try
                {
                    idmax = Convert.ToInt64(response.Content);
                }
                catch (Exception)
                {
                    return null;
                }
                
                //idmax = JsonConvert.DeserializeObject<long>(response.Content);
            }
            
            return idmax;
        }

        public IEnumerable<Order> GetAllOrders(ref int statusCode)
        {
            throw new NotImplementedException();
        }
    }
}
