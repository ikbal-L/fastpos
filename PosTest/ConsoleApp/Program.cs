using Newtonsoft.Json;
using ServiceInterface.Interface;
using ServiceInterface.Model;
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

        [Import(typeof(ICategorieService))]
        private ICategorieService categorieService = null;

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


        static async Task Main(string[] args)
        {/*
            client.BaseAddress = new Uri("http://192.168.1.3:8080/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Task<List<Product>> taskPproducts =  GetProductAsync("/Products");
            taskPproducts.Wait();
            List<Product> products = taskPproducts.Result;

            products.ForEach(p => Console.WriteLine(p.Id + p.Name));*/

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://127.0.0.1:5000/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            List<Product> products = null;
            HttpResponseMessage response = await client.GetAsync("/products");
            Console.WriteLine(response.Content);
            string resp;
            if (response.IsSuccessStatusCode)
            {
                resp = await response.Content.ReadAsStringAsync();
                Console.WriteLine(resp);
                var items = JsonConvert.DeserializeObject<List<Product>>(resp);
                products = await response.Content.ReadAsAsync<List<Product>>();
            }
            //return products;
            //List<Product> taskPproducts = await GetProductAsync(client, "/Products");
            Console.WriteLine(products.Count);
            Console.ReadKey();

            Program p = new Program();
            await p.Run();
            //p.CalRestAsync();


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

        async Task Run()
        {
            Compose();
            Console.WriteLine(1);
            Console.WriteLine(productService.GetAllProducts().Count);
            //Console.WriteLine(message+service.getProductsREST().Count);
            //service.getProductsREST().ForEach(p => Console.WriteLine(p.Id + p.Name));
/*            var products = this.productService.GetAllProducts();
            List<Product> products2;
            var cat1 = new Category { Id = 1, Description="abc", Name = "def", BackgroundString = "green",  };
            var cat2 = new Category { Id = 1, Description="abc", Name = "def", BackgroundString = "green",  };
            var cat3 = new Category { Id = 1, Description="abc", Name = "def", BackgroundString = "green",  };
            var cats = new List<Category>();
            cats.Add(cat1);
            cats.Add(cat2);
            cats.Add(cat3);

            var currentProductId = 1;
             products2 = new List<Product>()
            {
                new Platter { Id = currentProductId++, Name = "Pizza maison", Price = 15, CategorieId = 1,  Category=cat1 },
                new Platter { Id = currentProductId++, Name = "PIZZA VIANDES", Price = 29, CategorieId = 1,  Category=cat2},
                new Platter { Id = currentProductId++, Name = "PIZZA POULET", Price = 26, CategorieId = 1,  Category=cat3},
                new Platter { Id = currentProductId++, Name = "PIZZA BOEUF CHILI", Price = 29, CategorieId = 1,  Category=cat2},
                new Platter { Id = currentProductId++, Name = "PIZZA BUFFALO", Price = 26, CategorieId = 1,  Category=cat2},
            };

            var platter = new Platter { Id = currentProductId++, Name = "PIZZA BUFFALO", Price = 26, CategorieId = 1, Category=cat1 };
            var jsserializer = new JavaScriptSerializer();
            //jsserializer.RecursionLimit = 99999999;
            cat1.Products = products2;
            var json = jsserializer.Serialize(products);
*/            
            Console.WriteLine(2);
            var products3 = await productService.getProductsREST();
            Console.WriteLine(3);
            Console.WriteLine(products3?.Count);
            products3.ForEach(p => Console.WriteLine($"{p.Id} {p.Name}"));
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
