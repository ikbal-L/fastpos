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
    public class CustomerService : ICustomerService
    {
        private readonly RestCustomerService _restCustomerService = RestCustomerService.Instance;
        public int DeleteCustomer(long id)
        {
            return _restCustomerService.DeleteCustomer(id);
        }

        Customer ICustomerService.GetCustomer(long id)
        {
            throw new NotImplementedException();
        }

        ICollection<Customer> ICustomerService.GetAllCustomers()
        {
            throw new NotImplementedException();
        }

        public int SaveCustomer(Customer Customer, out long id)
        {
            return GenericRest.SaveThing(Customer, UrlConfig.CustomerUrl.SaveCustomer, out id);
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

    [Export(typeof(ICustomerService))]
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

        public ICollection<Customer> GetAllCustomers()
        {
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

            Customers = new List<Customer>() 
            { 
                new Customer { Id = 1, Mobile = "0560202020", Name = "C.ustomer1" },
                new Customer { Id = 1, Mobile = "0560202020", Name = "C.ustomer1" },
                new Customer { Id = 2, Mobile = "0560202020", Name = "Customer2" },
                new Customer { Id = 3, Mobile = "0560202020", Name = "Customer3" },
                new Customer { Id = 4, Mobile = "0560202020", Name = "Customer4" },
                new Customer { Id = 5, Mobile = "0560202020", Name = "Customer5" },
                new Customer { Id = 6, Mobile = "0560202020", Name = "Customer6" },
                new Customer { Id = 7, Mobile = "0560202020", Name = "Customer7" },
                new Customer { Id = 8, Mobile = "0560202020", Name = "Customer8" },
                new Customer { Id = 9, Mobile = "0560202020", Name = "Customer9" },
                new Customer { Id = 10, Mobile = "022020202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "072080202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "072980202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "072980202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "072980202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "072980202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "072980202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "072980202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "072980202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "062980202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "062980202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "062980202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "062980202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "062920202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "062020202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "062020202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "062020202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "062020202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "062020202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "062020202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "062020202", Name = "Customer10" },
                new Customer { Id = 10, Mobile = "062020202", Name = "Customer10" }

            };
            

            return Customers.ToList();
        }

        public int SaveCustomer(Customer Customer,out long id)
        {
            id = -1;
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

        public IEnumerable<Customer> GetAllCustomers(ref int statusCode)
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
