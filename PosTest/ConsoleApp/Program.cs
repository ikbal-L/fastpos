using Newtonsoft.Json;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;


namespace ConsoleApp
{
    class Program
    {
        //[Import]
        //string message;

        [Import(typeof(IProductService))]

        private IProductService productService = null;

        [Import(typeof(ICategoryService))]
        private ICategoryService categorieService = null;

        [Import(typeof(IAuthentification))]
        private IAuthentification authService = null;

        [Import(typeof(IAdditiveService))]
        private IAdditiveService additiveService = null;


        static HttpClient client = new HttpClient();

        static async Task<List<Product>> GetProductAsync(string path)
        {
            List<Product> products = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                //products = await response.Content.ReadAsAsync<List<Product>> ();
            }
            return products;
        }


        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
        }

        private async void CalRestAsync()
        {
            using (var client = new HttpClient())
            {
                var content = await client.GetStringAsync("http://127.0.0.1:5000/products");
                Console.WriteLine(content);
                var result = JsonConvert.DeserializeObject<List<Product>>(content);
            }

        }

        void Run()
        {
            Compose();
             Product p = new Product
            {
                Id = 4,
                Name = "prod1",
                CategorieId = 11,
                Price=300.00m,
                BackgroundString = "Green",
                AvailableStock = 222,
                IsNotifying = true
            };
            //var status = productService.SaveProduct(p);
            var token = authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id=1});
 
            int getProductsStatusCode = 0,
                getCategoriesStatusCode = 0, code = 0;
            var prods =  productService.GetAllProducts(ref getProductsStatusCode);
            var cats = categorieService.GetAllCategories(ref getCategoriesStatusCode);
            var adds = additiveService.GetAllAdditives(ref code);

            Console.ReadKey();

        }

        private void Compose()
        {
            //AssemblyCatalog catalog1 = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
           /* AssemblyCatalog catalog = new AssemblyCatalog("ServiceLib.dll");
            CompositionContainer container = new CompositionContainer(catalog);
            container.SatisfyImportsOnce(this);*/

            AssemblyCatalog catalog2 = new AssemblyCatalog("ServiceLib.dll");
            CompositionContainer container = new CompositionContainer(catalog2);
            container.SatisfyImportsOnce(this);
        }
    }


    public class MessageBox
    {
        [Export()]
        public string MyMessage
        {
            get { return "This is my example message."; }
        }
    }

}
