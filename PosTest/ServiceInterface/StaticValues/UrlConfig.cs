namespace ServiceInterface.StaticValues
{
    public static class UrlConfig
    {
        static string hostname1 = "http://192.168.1.102:5000/";
        //static string hostname1 = "http://127.0.0.1:5000/";
        public static class ProductUrl 
        {
            static string hostname = "http://192.168.1.3:5000/";

            public static string GetAllProducts = hostname1+"product/getall";
            public static string SaveProduct = hostname1+"product/save";
            public static string SaveProducts = hostname1 + "product/savemany";
            public static string GetProduct;
            public static string DeleteProduct = hostname1 + "product/delete/";
            public static string UpdateProduct = hostname1 + "product/update/";
        }

        public static class AdditiveUrl
        {
            static string hostname = "http://192.168.1.3:5000/";

            public static string GetManyAdditives = hostname1 + "additive/getmany";
            public static string SaveAdditive = hostname1 + "additive/save";
            public static string SaveAdditives = hostname1 + "additive/savemany";

            public static string GetAdditive = hostname1 + "additive/get/";

            public static string DeleteAdditive = hostname1 + "additive/delete/";
            public static string UpdateAdditive = hostname1 + "additive/update";
        }

        public static class CategoryUrl
        {
            static string hostname = "http://192.168.1.3:5000/";

            public static string GetAllCategories = hostname1 + "category/getall";
            public static string GetManyCategories = hostname1 + "category/getmany";
            public static string SaveCategory = hostname1 + "category/save";
            public static string SaveCategories = hostname1 + "category/savemany";

            public static string GetCategory = hostname1 + "category/get/";

            public static string DeleteCategory = hostname1 + "category/delete/";
            public static string UpdateCategory = hostname1 + "category/update";
        }        
        
        public static class AuthUrl
        {
            static string hostname = "http://192.168.1.3:5000/";

            public static string Authenticate = hostname1 + "auth/login";

        }
    }
}







