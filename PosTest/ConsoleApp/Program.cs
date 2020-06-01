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
            //Console.ReadKey();
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
            //Console.WriteLine(1);
            //Console.WriteLine(productService.GetAllProducts().Count);
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
            // Console.WriteLine(p1.Name);
            //var b1 = productService.DeleteProduct(3);
            //var status = productService.SaveProduct(p);
            //Console.WriteLine(b1);
            //var status = productService.GetProduct(3);
            //var products = productService.GetAllProducts();
            //Console.WriteLine(status);
            //Console.WriteLine(products.Count);
            //foreach(var p2 in products)
            //{
            //    Console.WriteLine($"Id: {p2.Id}  name {p2.Name}");
            //}

            //var pp = FakeServices.Products[0];
            //productService.PostTest(pp);

            //productService.SaveProducts(FakeServices.Products);
            //productService.createProducts();
            //additiveService.SaveAdditives(FakeServices.Additives);
            //categorieService.SaveCategory(FakeServices.Categories[0]);

            
            //prodserv.SaveProducts(FakeServices.Products);
            var prods =  productService.GetAllProducts();
            var cats = categorieService.GetAllCategories();           
            //prodserv.SaveCategories(FakeServices.Categories);
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
