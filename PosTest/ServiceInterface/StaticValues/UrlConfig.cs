namespace ServiceInterface.StaticValues
{
    public static class UrlConfig
    {
        public static class ProductUrl
        {
            public static string GetAllProducts = "http://127.0.0.1:5000/product/getall";
            public static string SaveProduct = "http://127.0.0.1:5000/product/save";
            public static string SaveProducts = "http://127.0.0.1:5000/product/savemany";
            public static string GetProduct;
            public static string DeleteProduct = "http://127.0.0.1:5000/product/delete/";
        }

        public static class AdditiveUrl
        {
            public static string GetManyAdditives = "http://127.0.0.1:5000/additive/getmany";
            public static string SaveAdditive = "http://127.0.0.1:5000/additive/save";
            public static string SaveAdditives = "http://127.0.0.1:5000/additive/savemany";

            public static string GetAdditive = "http://127.0.0.1:5000/additive/get/";

            public static string DeleteAdditive = "http://127.0.0.1:5000/additive/delete/";
            public static string UpdateAdditive = "http://127.0.0.1:5000/additive/update";
        }

        public static class CategoryUrl
        {
            public static string GetAllCategories = "http://127.0.0.1:5000/category/getall";
            public static string GetManyCategories = "http://127.0.0.1:5000/category/getmany";
            public static string SaveCategory = "http://127.0.0.1:5000/category/save";
            public static string SaveCategories = "http://127.0.0.1:5000/category/savemany";

            public static string GetCategory = "http://127.0.0.1:5000/category/get/";

            public static string DeleteCategory = "http://127.0.0.1:5000/category/delete/";
            public static string UpdateCategory = "http://127.0.0.1:5000/category/update";
        }
    }
}







