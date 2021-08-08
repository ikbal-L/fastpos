namespace ServiceInterface.StaticValues
{
    public static class UrlConfig
    {
        //static string hostname1 = "http://192.168.1.114:5000/";
        // static string hostname1 = "http://127.0.0.1:5001/";
        static string hostname1 = "http://localhost:8080/";
        public static class ProductUrl 
        {
            private static string ProductApiUrl = "api/product";
            public static string GetAllProducts = hostname1+ProductApiUrl+"/getall";
            public static string SaveProduct = hostname1+ProductApiUrl+"/save";
            public static string SaveProducts = hostname1 +ProductApiUrl+ "/savemany";
            public static string GetProduct;
            public static string DeleteProduct = hostname1 +ProductApiUrl+ "/delete/";
            public static string UpdateProduct = hostname1 +ProductApiUrl+ "/put/";
            public static string UpdateManyProducts = hostname1+ProductApiUrl+ "/updatemany";
            public static readonly string GetManyProducts = hostname1 +ProductApiUrl+ "/getmany";
        }

        public static class AdditiveUrl
        {
            static string hostname = "http://192.168.1.7:5000/";
            private static string AdditiveApiUrl = hostname1+ "api/additive";
            public static string GetManyAdditives = AdditiveApiUrl + "/getmany";
            public static string SaveAdditive = AdditiveApiUrl + "/save";
            public static string SaveAdditives = AdditiveApiUrl + "/savemany";

            public static string GetAdditive = AdditiveApiUrl + "/get/";

            public static string DeleteAdditive = AdditiveApiUrl + "/delete/";
            public static string UpdateAdditive = AdditiveApiUrl + "/put/";
            public static string GetAllAdditives = AdditiveApiUrl + "/getall";
        }

        public static class CategoryUrl
        {
            static string hostname = "http://192.168.1.7:5000/";
            private static string CategoryApiUrl = "api/category";
            public static string GetAllCategories = hostname1 +CategoryApiUrl+ "/getall";
            public static string GetManyCategories = hostname1 +CategoryApiUrl+ "/getmany";
            public static string SaveCategory = hostname1 +CategoryApiUrl+ "/save";
            public static string SaveCategories = hostname1 +CategoryApiUrl +"/savemany";

            public static string GetCategory = hostname1 +CategoryApiUrl +"/get/";

            public static string DeleteCategory = hostname1 +CategoryApiUrl+ "/delete/";
            public static string UpdateCategory = hostname1 +CategoryApiUrl +"/put/";
        }        
        
        public static class OrderUrl
        {
            static string hostname = "http://192.168.1.100:5000/";
            private static string OrderApiEndpoint = hostname1 + "api/order";
            public static string GetAllOrders = OrderApiEndpoint + "/getall";
            public static string GetAllUnprocessedOrders = GetAllOrders+ "/unprocessed";
            public static string GetManyOrders = OrderApiEndpoint + "/getmany";
            public static string SaveOrders = OrderApiEndpoint + "/savemany";
            public static string GetOrder = OrderApiEndpoint + "/get/";
            public static string DeleteOrder = OrderApiEndpoint + "/delete/";
            public static string UpdateOrder = OrderApiEndpoint + "/put/";
            public static string SaveOrder = OrderApiEndpoint + "/save";
            public static string GetIdMax = OrderApiEndpoint + "/idmax";

            private static string TableApiEndpoint = hostname1 + "api/table";
            public static string GetTableByNumber = TableApiEndpoint + "/getbynumber/";
            public static string GetTable = TableApiEndpoint + "/get/";
            public static string SaveTable = TableApiEndpoint + "/save";
            public static string GetAllTables = TableApiEndpoint + "/getall";
            public static string GetByStatePage = OrderApiEndpoint + "/getByState";
            public static string GetAllByDeliveryManPage = OrderApiEndpoint + "/getAllbyDeliveryManAndStatePage";
        }

        public static class PaymentUrl
        {
            private static string PaymentApiEndpoint = hostname1 + "api/payment";

            public static string GetAllByDeliveryManPage = PaymentApiEndpoint + "/getAllbydeliverymanPage";
            public static string GetByDeliveryManAndDate = PaymentApiEndpoint + "/getByDeliverymanAndDate";

        }
        public static class DelivereyUrl
        {
            static string hostname = "http://192.168.1.7:5000/";
            private static string DeliveryApiEndpoint = hostname1 + "api/deliveryman";
            public static string GetAllDeliverymen = DeliveryApiEndpoint + "/getall";
            public static string GetAllActiveDeliverymen = DeliveryApiEndpoint + "/getallactive";
            public static string GetDeliveryman = DeliveryApiEndpoint + "/get/";
            public static string DeleteDeliveryman = DeliveryApiEndpoint + "/delete/";
            public static string UpdateDeliveryman = DeliveryApiEndpoint + "/update";
            public static string SaveDeliveryman = DeliveryApiEndpoint + "/save";
            public static string GetDeliverymen = DeliveryApiEndpoint + "/get/";
        }

        public static class WaiterUrl
        {
            static string hostname = "http://192.168.1.7:5000/";
            private static string WaiterApiEndpoint = hostname1 + "api/waiter";
            public static string GetAllWaiters = WaiterApiEndpoint + "/getall";
            public static string GetAllActiveWaiters = WaiterApiEndpoint + "/getallactive";
            public static string GetWaiter = WaiterApiEndpoint + "/get/";
            public static string DeleteWaiter = WaiterApiEndpoint + "/delete/";
            public static string UpdateWaiter = WaiterApiEndpoint + "/update";
            public static string SaveWaiter = WaiterApiEndpoint + "/save";
            public static string GetWaiters = WaiterApiEndpoint + "/get/";
        }
        
        public static class CustomerUrl
        {
            private static string CustomerApiEndpoint = hostname1 + "api/customer";
            public static string GetAllCustomers = CustomerApiEndpoint + "/getall";
            public static string GetManyCustomers = CustomerApiEndpoint + "/getmany";
            public static string GetAllActiveCustomers = CustomerApiEndpoint + "/getallactive";
            public static string GetCustomer = CustomerApiEndpoint + "/get/";
            public static string DeleteCustomer = CustomerApiEndpoint + "/delete/";
            public static string UpdateCustomer = CustomerApiEndpoint + "/update";
            public static string SaveCustomer = CustomerApiEndpoint + "/save";
            public static string SaveCustomers = CustomerApiEndpoint + "/savemany";
            public static string GetCustomers = CustomerApiEndpoint + "/get/";
        }

        public static class AuthUrl
        {
            static string hostname = "http://192.168.1.7:5000/";
            // public static string Authenticate = hostname1 + "auth/login";
            public static string GetUserAnnexesIds = hostname1 + "config/" + "user/getannexes/";
            public static string Authenticate = hostname1 + "login";
        }
    }
}







