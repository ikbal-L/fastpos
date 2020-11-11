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

        public int SaveOrder(Order order, out IEnumerable<string> errors)
        {
            order.MappingBeforeSending();
            return _restOrderService.SaveOrder(order,out errors);
        }

        public int UpdateOrder(Order order)
        {
            order.MappingBeforeSending();
            return _restOrderService.UpdateOrder(order);
        }

        public Table GetTable(int id, ref int statusCode)
        {
            return GenericRest.GetThing<Table>(UrlConfig.OrderUrl.GetTable + id.ToString(), ref statusCode);
        }

        public Table GetTableByNumber(int tableNumber, ref int statusCode)
        {
            return GenericRest.GetThing<Table>(UrlConfig.OrderUrl.GetTableByNumber + tableNumber.ToString(), ref statusCode);
        }

        public int SaveTable(Table table)
        {
            return GenericRest.SaveThing<Table>(table, UrlConfig.OrderUrl.SaveTable);
        }

        public IEnumerable<Table> GeAlltTables(ref int statusCode)
        {
            return GenericRest.GetThing<IEnumerable<Table>>(UrlConfig.OrderUrl.GetAllTables, ref statusCode);
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
            var client = new RestClient(UrlConfig.OrderUrl.GetOrder + id.ToString());
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

        public int SaveOrder(Order order, out IEnumerable<string> errors)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            //product = MapProduct.MapProductToSend(product);
            string json = JsonConvert.SerializeObject(order,
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
            errors = new List<string>();
            try
            {
                errors = JsonConvert.DeserializeObject<IEnumerable<string>>(response.Content);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                ;
            }
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
            var response = GenericRest.RestGet(UrlConfig.OrderUrl.GetIdMax);
            statusCode = (int)response.StatusCode;
            long? idmax = null;
            if (statusCode == (int)HttpStatusCode.OK)
            {
                try
                {
                    idmax = Convert.ToInt64(response.Content);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return idmax;
        }

        public IEnumerable<Order> GetAllOrders(ref int statusCode)
        {
            throw new NotImplementedException();
        }

        public Table GetTable(int id, ref int statusCode)
        {
            return GenericRest.GetThing<Table>(UrlConfig.OrderUrl.GetTable + id.ToString(), ref statusCode);
        }

        public Table GetTableByNumber(int tableNumber, ref int statusCode)
        {
            return GenericRest.GetThing<Table>(UrlConfig.OrderUrl.GetTableByNumber + tableNumber.ToString(), ref statusCode);
        }

        public int SaveTable(Table table)
        {
            return GenericRest.SaveThing<Table>(table, UrlConfig.OrderUrl.SaveTable);
        }

        public IEnumerable<Table> GeAlltTables(ref int statusCode)
        {
            throw new NotImplementedException();
        }

    }
    [Export(typeof(IDelivereyService))]
    public class DelivereyService : IDelivereyService
    {
        public int DeleteDelivereyman(long orderId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Delivereyman> GetAllActiveDelivereymen(ref int statusCode)
        {
            return GenericRest.GetThing<IEnumerable<Delivereyman>>(UrlConfig.DelivereyUrl.GetAllActiveDeliverymen, ref statusCode);
        }

        public IEnumerable<Delivereyman> GetAllDelivereymen(ref int statusCode)
        {
            return GenericRest.GetThing<IEnumerable<Delivereyman>>(UrlConfig.DelivereyUrl.GetAllDeliverymen, ref statusCode);
        }

        public Delivereyman GetDelivereyman(int id, ref int statusCode)
        {
            return GenericRest.GetThing<Delivereyman>(UrlConfig.DelivereyUrl.GetDeliverymen+id.ToString(), ref statusCode);
        }

        public int SaveDelivereyman(Delivereyman delivereyman)
        {
            return GenericRest.SaveThing(delivereyman, UrlConfig.DelivereyUrl.SaveDeliveryman);
        }

        public int UpdateDelivereyman(Delivereyman delivereyman)
        {
            throw new NotImplementedException();
        }
    }

    [Export(typeof(IWaiterService))]
    public class WaiterService : IWaiterService
    {
        public int DeleteWaiter(long orderId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Waiter> GetAllActiveWaiters(ref int statusCode)
        {
            return GenericRest.GetThing<IEnumerable<Waiter>>(UrlConfig.WaiterUrl.GetAllActiveWaiters, ref statusCode);
        }

        public IEnumerable<Waiter> GetAllWaiters(ref int statusCode)
        {
            return GenericRest.GetThing<IEnumerable<Waiter>>(UrlConfig.WaiterUrl.GetAllWaiters, ref statusCode);
        }

        public Waiter GetWaiters(int id, ref int statusCode)
        {
            throw new NotImplementedException();
        }

        public int SaveWaiter(Waiter Waiter)
        {
            throw new NotImplementedException();
        }

        public int UpdateWaiter(Waiter Waiter)
        {
            throw new NotImplementedException();
        }
    }
}
