using Newtonsoft.Json;
using RestSharp;
using ServiceInterface.Authorisation;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ServiceLib.Service
{
    [Export(typeof(ICustomerService))]
    public class CustomerService : ICustomerService
    {
        private readonly RestCustomerService _restCustomerService = RestCustomerService.Instance;
        public int DeleteCustomer(long id)
        {
            return _restCustomerService.DeleteCustomer(id);
        }

        public Customer GetCustomer(long id)
        {
            throw new NotImplementedException();
        }

        public ICollection<Customer> GetAllCustomers(out int status)
        {
            return GenericRest.GetAll<Customer>(UrlConfig.CustomerUrl.GetAllCustomers, out status).ToList();
        }

        public int SaveCustomer(Customer Customer, out long id, out IEnumerable<string> errors)
        {
            return GenericRest.SaveThing(ref Customer, UrlConfig.CustomerUrl.SaveCustomer, out id,out errors);
        }

        public int SaveCustomers(IEnumerable<Customer> Customers)
        {
            throw new NotImplementedException();
        }

        public int UpdateCustomer(Customer Customer)
        {
            throw new NotImplementedException();
        }
    }

    
    public class RestCustomerService : ICustomerService
    {
        private static RestCustomerService _instance;

        public static RestCustomerService Instance => _instance ?? (_instance = new RestCustomerService());
        public int DeleteCustomer(long additivdId)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(UrlConfig.CustomerUrl.DeleteCustomer + $"{additivdId}");
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

        public Customer GetCustomer(long id)
        {
            string token = AuthProvider.Instance?.AuthorizationToken;
            var client = new RestClient(UrlConfig.CustomerUrl.GetCustomer+id.ToString());
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", token);
            request.AddHeader("accept", "application/json");
            IRestResponse response = client.Execute(request);
            Customer Customer = null;
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Customer = JsonConvert.DeserializeObject<Customer>(response.Content);
            }

            return Customer;// ;products

            //return new Customer { Id=id};
        }

        public ICollection<Customer> GetAllCustomers(out int status)
        {
            status = -1;
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(UrlConfig.CustomerUrl.GetManyCustomers);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", token);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Annex-Id", $"{AuthProvider.Instance.AnnexId}");
            string json = JsonConvert.SerializeObject(
                           Newtonsoft.Json.Formatting.None,
                           new JsonSerializerSettings
                           {
                               NullValueHandling = NullValueHandling.Ignore
                           });
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            

            IEnumerable<Customer> Customers = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Customers = JsonConvert.DeserializeObject<IEnumerable<Customer>>(response.Content);
            }

       
            

            return Customers.ToList();
        }

        public int SaveCustomer(Customer Customer,out long id, out IEnumerable<string> errors)
        {
            id = -1;
            errors = new List<string>();
            string token = AuthProvider.Instance.AuthorizationToken;
            //product = MapProduct.MapProductToSend(product);
            string json = JsonConvert.SerializeObject(Customer,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            var url = UrlConfig.CustomerUrl.SaveCustomer;
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", token);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //    return true;

            return (int)response.StatusCode;
        }

       public int UpdateCustomer(Customer Customer)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            //product = MapProduct.MapProductToSend(product);
            string json = JsonConvert.SerializeObject(Customer,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            var url = UrlConfig.CustomerUrl.UpdateCustomer;
            var client = new RestClient(url);
            var request = new RestRequest(Method.PUT);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", token);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            //if (response.StatusCode == HttpStatusCode.OK)
            //    return true;

            return (int)response.StatusCode;
        }

        public int SaveCustomers(IEnumerable<Customer> Customers)
        {
            string token = AuthProvider.Instance.AuthorizationToken;
            var client = new RestClient(UrlConfig.CustomerUrl.SaveCustomers);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("Authorization", token);
            //foreach (var p in products)
            //{
            //    MapProduct.MapProductToSend(p);
            //}
            var jsonData = JsonConvert.SerializeObject(Customers,
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

        public IEnumerable<Customer> GetAllCustomers( int statusCode)
        {
            string token = AuthProvider.Instance?.AuthorizationToken;
            var client = new RestClient(UrlConfig.CustomerUrl.GetAllCustomers);
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", token);
            request.AddHeader("accept", "application/json");
            IRestResponse response = client.Execute(request);
            IEnumerable<Customer> Customers = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Customers = JsonConvert.DeserializeObject<IEnumerable<Customer>>(response.Content);
            }
            statusCode = (int)response.StatusCode;
            return Customers;
        }
    }


}
