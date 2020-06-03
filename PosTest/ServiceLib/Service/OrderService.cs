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
        public int DeleteOrder(int orderId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Order> GetAllOrders()
        {
            throw new NotImplementedException();
        }

        public long GetIdmax()
        {

            var idmax = _restOrderService.GetIdmax();
            if (idmax==null)
            {
                throw new Exception("idmax is null");
            }
            return (long)idmax;
        }

        public IEnumerable<Order> GetManyOrders(IEnumerable<long> orderIds)
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

    internal class RestOrderService
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

        public Order GetOrder(long id)
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

            return order;// ;products

            //return new Additive { Id=id};
        }

        public IEnumerable<Order> GetManyOrders(IEnumerable<long> ids)
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

        public bool UpdateOrder(Order order)
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

            if (response.StatusCode == HttpStatusCode.OK)
                return true;

            return false;
        }

        public bool SaveOrders(IEnumerable<Order> orders)
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

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        internal long? GetIdmax()
        {
            string token = AuthProvider.Instance?.AuthorizationToken;
            var client = new RestClient(UrlConfig.OrderUrl.GetIdMax);
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", token);
            request.AddHeader("accept", "application/json");
            IRestResponse response = client.Execute(request);
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
    }
}
