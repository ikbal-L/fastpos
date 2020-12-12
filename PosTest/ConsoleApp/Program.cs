using Newtonsoft.Json;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using ServiceLib;
using ServiceLib.Service;
using System;
using System.Linq;
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

        [Import(typeof(IOrderService))]
        private IOrderService orderService = null;

       [Import(typeof(IDelivereyService))]
        private IDelivereyService delivereyService = null;


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
            var date = "2020-06-06 15:16:12.343000";
            DateTime oDate = Convert.ToDateTime(date);
            Console.WriteLine(oDate);
            Program p = new Program();
            p.Run();
        }

        private async void CallRestAsync()
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
                CategoryId = 11,
                Price = 300.00m,
                BackgroundString = "Green",
                AvailableStock = 222,
                IsNotifying = true
            };

            //var status = productService.SaveProduct(p);
            // var token = authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id=1});
            var token = authService.Authenticate("admin", "admin", new Annex { Id = 1 }, new Terminal { Id=1});
            //var resp1 = productService.SaveProducts(FakeServices.Products);
            //var resp2 = categorieService.SaveCategories(FakeServices.Categories);
            //var resp3 = additiveService.SaveAdditives(FakeServices.Additives);
            int getProductsStatusCode = 0;
            //    getCategoriesStatusCode = 0, code = 0;
            // var prods =  productService.GetAllProducts(ref getProductsStatusCode);
            var t1 = DateTime.Now;
            var savedCode = GenericRest.SaveThing(ref p, UrlConfig.ProductUrl.SaveProduct, out long id,out IEnumerable<string> errors);
            var t2 = DateTime.Now;
            Console.WriteLine(t2-t1);

            
            // //var cats = categorieService.GetAllCategories(ref getCategoriesStatusCode);
            // //var adds = additiveService.GetAllAdditives(ref code);
            //
            // // Order o = new Order
            // // {
            // //     State = OrderState.Ready
            // // };
            // //
            // // int? a;
            //
            //
            // //orderService.SaveOrder(o);
            // var status = 0;
            // //var prods = GenericRest.GetManyThings<Platter>(new List<long> { 1, 2, 3, 4 }, UrlConfig.ProductUrl.GetManyProducts, ref status);
            // //var men = delivereyService.GetAllActiveDeliverymen(ref status);
            // //Console.WriteLine(men.Count());
            //
            // (IEnumerable<long> ids, var status1) = productService.UpdateManyProducts(prods.Where(pr => pr.Id == 41 || pr.Id == 42));
            // Console.WriteLine(ids.Count());
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
