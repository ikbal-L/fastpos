using System;

namespace ServiceInterface.StaticValues
{
    public static class UrlConfig
    {
        static string hostname1 = "http://192.168.1.102:5000/";
        //static string hostname1 = "http://127.0.0.1:5000/";
        public static class ProductUrl 
        {
            static string hostname = "http://192.168.1.102:5000/";

            public static string GetAllProducts = hostname1+"product/getall";
            public static string SaveProduct = hostname1+"product/save";
            public static string SaveProducts = hostname1 + "product/savemany";
            public static string GetProduct;
            public static string DeleteProduct = hostname1 + "product/delete/";
            public static string UpdateProduct = hostname1 + "product/update";
            public static readonly string GetManyProducts = hostname1 + "product/getmany";

        }

        public static class AdditiveUrl
        {
            static string hostname = "http://192.168.1.102:5000/";

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
            static string hostname = "http://192.168.1.102:5000/";
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
            static string hostname = "http://192.168.1.102:5000/";

            public static string GetAllOrders = hostname + "order/getall";
            public static string GetManyOrders = hostname + "order/getmany";
            public static string SaveOrders = hostname + "order/savemany";
            public static string GetOrder = hostname + "order/get/";
            public static string GetTable = hostname + "order/gettable/";
            public static string DeleteOrder = hostname + "order/delete/";
            public static string UpdateOrder = hostname + "order/update";
            public static string SaveOrder = hostname + "order/save";
            public static string GetIdMax = hostname + "order/idmax";
        }

        public static class AuthUrl
        {
            static string hostname = "http://192.168.1.102:5000/";
            public static string Authenticate = hostname1 + "auth/login";

        }
    }
}







