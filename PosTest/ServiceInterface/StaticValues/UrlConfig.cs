using System;

namespace ServiceInterface.StaticValues
{
    public static class UrlConfig
    {
        //static string hostname1 = "http://192.168.1.114:5000/";
        static string hostname1 = "http://127.0.0.1:5001/";
        public static class ProductUrl 
        {
            public static string GetAllProducts = hostname1+"product/getall";
            public static string SaveProduct = hostname1+"product/save";
            public static string SaveProducts = hostname1 + "product/savemany";
            public static string GetProduct;
            public static string DeleteProduct = hostname1 + "product/delete/";
            public static string UpdateProduct = hostname1 + "product/update";
            public static string UpdateManyProducts = hostname1+ "/product/updatemany";
            public static readonly string GetManyProducts = hostname1 + "product/getmany";
        }

        public static class AdditiveUrl
        {
             static string hostname = "http://192.168.1.7:5000/";
            public static string GetManyAdditives = hostname1 + "additive/getmany";
            public static string SaveAdditive = hostname1 + "additive/save";
            public static string SaveAdditives = hostname1 + "additive/savemany";

            public static string GetAdditive = hostname1 + "additive/get/";

            public static string DeleteAdditive = hostname1 + "additive/delete/";
            public static string UpdateAdditive = hostname1 + "additive/update";
            public static string GetAllAdditives = hostname1 + "additive/getall";
        }

        public static class CategoryUrl
        {
            static string hostname = "http://192.168.1.7:5000/";

            public static string GetAllCategories = hostname1 + "category/getall";
            public static string GetManyCategories = hostname1 + "category/getmany";
            public static string SaveCategory = hostname1 + "category/save";
            public static string SaveCategories = hostname1 + "category/savemany";

            public static string GetCategory = hostname1 + "category/get/";

            public static string DeleteCategory = hostname1 + "category/delete/";
            public static string UpdateCategory = hostname1 + "category/update";
        }        
        
        public static class OrderUrl
        {
            static string hostname = "http://192.168.1.100:5000/";

            public static string GetAllOrders = hostname1 + "order/getall";
            public static string GetManyOrders = hostname1 + "order/getmany";
            public static string SaveOrders = hostname1 + "order/savemany";
            public static string GetOrder = hostname1 + "order/get/";
            public static string DeleteOrder = hostname1 + "order/delete/";
            public static string UpdateOrder = hostname1 + "order/update";
            public static string SaveOrder = hostname1 + "order/save";
            public static string GetIdMax = hostname1 + "order/idmax";

            public static string GetTableByNumber = hostname1 + "table/getbynumber/";
            public static string GetTable = hostname1 + "table/get/";
            public static string SaveTable = hostname1 + "table/save";
            public static string GetAllTables = hostname1 + "table/getall";
        } 
        
        public static class DelivereyUrl
        {
            static string hostname = "http://192.168.1.7:5000/";
            
            public static string GetAllDeliverymen = hostname1 + "deliveryman/getall";
            public static string GetAllActiveDeliverymen = hostname1 + "deliveryman/getallactive";
            public static string GetDeliveryman = hostname1 + "deliveryman/get/";
            public static string DeleteDeliveryman = hostname1 + "deliveryman/delete/";
            public static string UpdateDeliveryman = hostname1 + "deliveryman/update";
            public static string SaveDeliveryman = hostname1 + "deliveryman/save";
            public static string GetDeliverymen = hostname1 + "deliveryman/get/";
        }

        public static class WaiterUrl
        {
            static string hostname = "http://192.168.1.7:5000/";

            public static string GetAllWaiters = hostname1 + "waiter/getall";
            public static string GetAllActiveWaiters = hostname1 + "waiter/getallactive";
            public static string GetWaiter = hostname1 + "waiter/get/";
            public static string DeleteWaiter = hostname1 + "waiter/delete/";
            public static string UpdateWaiter = hostname1 + "waiter/update";
            public static string SaveWaiter = hostname1 + "waiter/save";
            public static string GetWaiters = hostname1 + "waiter/get/";
        }
        
        public static class CustomerUrl
        {
            public static string GetAllCustomers = hostname1 + "customer/getall";
            public static string GetManyCustomers = hostname1 + "customer/getmany";
            public static string GetAllActiveCustomers = hostname1 + "customer/getallactive";
            public static string GetCustomer = hostname1 + "customer/get/";
            public static string DeleteCustomer = hostname1 + "customer/delete/";
            public static string UpdateCustomer = hostname1 + "customer/update";
            public static string SaveCustomer = hostname1 + "customer/save";
            public static string SaveCustomers = hostname1 + "customer/savemany";
            public static string GetCustomers = hostname1 + "customer/get/";
        }

        public static class AuthUrl
        {
            static string hostname = "http://192.168.1.7:5000/";
            public static string Authenticate = hostname1 + "auth/login";
        }
    }
}







