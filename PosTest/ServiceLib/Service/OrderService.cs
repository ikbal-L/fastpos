using RestSharp;
using ServiceInterface.Authorisation;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using System;
using System.Collections;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Web.ModelBinding;
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

        public (int,IEnumerable<Order>) GetAllOrders( bool unprocessed = false)
        {
            string url = unprocessed ? UrlConfig.OrderUrl.GetAllUnprocessedOrders : UrlConfig.OrderUrl.GetAllOrders;
            return GenericRest.GetAll<Order>(url);
        }

        public (int,IEnumerable<Order>) GetManyOrders(IEnumerable<long> orderIds)
        {
            throw new NotImplementedException();
        }

        public int SaveOrder( Order order,out IEnumerable<string> errors)
        {
            order.MappingBeforeSending();
            errors = ValidationService.Validate(order);
            if (errors.Any())
            {
                // id = -1;
                return 0;
            }
            var response = GenericRest.SaveThing(order,UrlConfig.OrderUrl.SaveOrder);
            if (response.StatusCode == HttpStatusCode.Created|| response.StatusCode==HttpStatusCode.OK)
            {

                var deserializedOrder = JsonConvert.DeserializeObject<Order>(response.Content);
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    order.Id = deserializedOrder.Id; 
                }

                foreach (var newOrderItem in deserializedOrder.OrderItems)
                {
                    var oldOrderItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == newOrderItem.ProductId);
                    newOrderItem.Additives = oldOrderItem.Additives;
                }

                ;
                order.OrderItems = deserializedOrder.OrderItems;
                deserializedOrder = null;


            }
            else
            {
                errors = JsonConvert.DeserializeObject<IEnumerable<string>>(response.Content);
            }

            return (int)response.StatusCode;

        }

        public int UpdateOrder(ref Order order,out IEnumerable<string> errors)
        {
            order.MappingBeforeSending();
            //order.MappingBeforeSending();
            errors = ValidationService.Validate(order);
            if (errors.Count() > 0)
            {
                return 0;
            }
            var ( statusCode,content ) = GenericRest.UpdateThing(order,UrlConfig.OrderUrl.UpdateOrder+order.Id,out _);
            return statusCode;
            // return _restOrderService.UpdateOrder(order);
        }

        public (int,Table) GetTable(int id)
        {
            return GenericRest.GetThing<Table>(UrlConfig.OrderUrl.GetTable + id.ToString());
        }

        public (int,Table) GetTableByNumber(int tableNumber)
        {
            return GenericRest.GetThing<Table>(UrlConfig.OrderUrl.GetTableByNumber + tableNumber.ToString());
        }

        public int SaveTable(Table table,out  IEnumerable<string> errors)
        {
            return GenericRest.SaveThing<Table>(table, UrlConfig.OrderUrl.SaveTable,out errors).Item1;
        }

        public (int,IEnumerable<Table>)  GeAllTables()
        {
            return GenericRest.GetThing<IEnumerable<Table>>(UrlConfig.OrderUrl.GetAllTables);
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

        public (int,Order) GetOrder(long id)
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

            return ((int)response.StatusCode, order);// ;products

            //return new Additive { Id=id};
        }

        public (int,IEnumerable<Order>) GetManyOrders(IEnumerable<long> ids)
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
            
            return ((int)response.StatusCode,orders);
        }

        public int SaveOrder(Order order/*,out long id*/, out IEnumerable<string> errors)
        {
            // id = -1;
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
            request.AddHeader("Annex-Id", $"{AuthProvider.Instance.AnnexId}");
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

        public int UpdateOrder(ref Order order, out IEnumerable<string> errors)
        {
            errors = new List<string>();
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

        
        public (int,IEnumerable<Order>) GetAllOrders(bool unprocessed = false)
        {
            throw new NotImplementedException();
        }

        public (int,Table) GetTable(int id)
        {
            return GenericRest.GetThing<Table>(UrlConfig.OrderUrl.GetTable + id.ToString());
        }

        public (int status, Table table) GetTableByNumber(int tableNumber)
        {
            return GenericRest.GetThing<Table>(UrlConfig.OrderUrl.GetTableByNumber + tableNumber.ToString());
        }

        public int SaveTable(Table table,out IEnumerable<string> errors)
        {
            return GenericRest.SaveThing<Table>( table, UrlConfig.OrderUrl.SaveTable,out errors).Item1;
        }

        public (int,IEnumerable<Table>) GeAllTables()
        {
            throw new NotImplementedException();
        }

    }
    [Export(typeof(IDelivereyService))]
    public class DeliveryService : IDelivereyService
    {
        public int DeleteDeliveryman(long orderId)
        {
            throw new NotImplementedException();
        }

        public (int,IEnumerable<Deliveryman>) GetAllActiveDeliverymen()
        {
            return GenericRest.GetThing<IEnumerable<Deliveryman>>(UrlConfig.DelivereyUrl.GetAllActiveDeliverymen);
        }

        public (int,IEnumerable<Deliveryman>) GetAllDeliverymen()
        {
            return GenericRest.GetAll<Deliveryman>(UrlConfig.DelivereyUrl.GetAllDeliverymen);
        }

        public (int,Deliveryman) GetDeliveryman(int id)
        {
            return GenericRest.GetThing<Deliveryman>(UrlConfig.DelivereyUrl.GetDeliverymen+id.ToString());
        }

        public int SaveDeliveryman(Deliveryman deliveryman,out IEnumerable<string> errors)
        {
            return GenericRest.SaveThing(deliveryman, UrlConfig.DelivereyUrl.SaveDeliveryman,out errors).Item1;
        }

        public int UpdateDeliveryman(Deliveryman deliveryman)
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

        public (int,IEnumerable<Waiter>) GetAllActiveWaiters()
        {
            return GenericRest.GetThing<IEnumerable<Waiter>>(UrlConfig.WaiterUrl.GetAllActiveWaiters);
        }

        public (int,IEnumerable<Waiter>) GetAllWaiters()
        {
            return GenericRest.GetThing<IEnumerable<Waiter>>(UrlConfig.WaiterUrl.GetAllWaiters);
        }

        public (int,Waiter) GetWaiters(int id)
        {
            throw new NotImplementedException();
        }

        public int SaveWaiter(Waiter waiter , out IEnumerable<string> errors)
        {
            return GenericRest.SaveThing(waiter,UrlConfig.WaiterUrl.SaveWaiter, out errors).Item1;
        }

        public int UpdateWaiter(Waiter waiter)
        {
            throw new NotImplementedException();
        }
    }
}
