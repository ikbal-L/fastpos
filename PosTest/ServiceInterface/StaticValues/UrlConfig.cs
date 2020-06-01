namespace ServiceInterface.StaticValues
{
    public static class UrlConfig
    {
       
        public static class ProductUrl 
        {
            static string hostname = "http://192.168.1.3:5000/";

            public static string GetAllProducts = hostname+"product/getall";
            public static string SaveProduct = hostname+"product/save";
            public static string SaveProducts = hostname + "product/savemany";
            public static string GetProduct;
            public static string DeleteProduct = hostname + "product/delete/";
        }

        public static class AdditiveUrl
        {
            static string hostname = "http://192.168.1.3:5000/";

            public static string GetManyAdditives = hostname + "additive/getmany";
            public static string SaveAdditive = hostname + "additive/save";
            public static string SaveAdditives = hostname + "additive/savemany";

            public static string GetAdditive = hostname + "additive/get/";

            public static string DeleteAdditive = hostname + "additive/delete/";
            public static string UpdateAdditive = hostname + "additive/update";
        }

        public static class CategoryUrl
        {
            static string hostname = "http://192.168.1.3:5000/";

            public static string GetAllCategories = hostname + "category/getall";
            public static string GetManyCategories = hostname + "category/getmany";
            public static string SaveCategory = hostname + "category/save";
            public static string SaveCategories = hostname + "category/savemany";

            public static string GetCategory = hostname + "category/get/";

            public static string DeleteCategory = hostname + "category/delete/";
            public static string UpdateCategory = hostname + "category/update";
        }        
        
        public static class AuthUrl
        {
            static string hostname = "http://192.168.1.3:5000/";

            public static string Authenticate = hostname + "auth/login";

        }
    }
}







