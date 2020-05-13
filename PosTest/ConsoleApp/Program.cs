using Newtonsoft.Json;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        //[Import]
        //string message;

        [Import(typeof(IProductService))]
        IProductService service;

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
        {
            //Category1 cat = new Category1();
            //cat.Id = 7;
            //cat.Name = "test";
            //string t = JsonConvert.SerializeObject(cat);
            //Category1 c = JsonConvert.DeserializeObject<Category1>(t);

            // Console.WriteLine(t);
            //Console.WriteLine(c.BackGroundColor.ToString());
            //Console.ReadKey();


            //client.BaseAddress = new Uri("http://192.168.1.2:5000/");
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //Task<List<Product>> taskPproducts =  GetProductAsync("/products");
            //taskPproducts.Wait();
            //List<Product> products = taskPproducts.Result;

            //products.ForEach(p => Console.WriteLine(p.Id + p.Name));



            HttpClient client1 = new HttpClient();
            client1.BaseAddress = new Uri("http://192.168.1.2:5000/");
            client1.DefaultRequestHeaders.Accept.Clear();
            client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client1.GetAsync("/products");

            if (response.IsSuccessStatusCode)
            {
                var product = await response.Content.ReadAsAsync<List<Product>>();
                product.ForEach(p => Console.WriteLine(p.Id + p.Name));
                //Console.WriteLine(category1.Id);
                Console.ReadKey();
            }





            //return products;
            //List<Product> taskPproducts = await GetProductAsync(client, "/Products");
            //Console.WriteLine(products.Count);


            //Program p = new Program();
            //await p.RunAsync();
        }

        async Task RunAsync()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://127.0.0.1:5000/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Console.WriteLine("4");
            List<Product> products = null;
            Console.WriteLine("5");
            Console.ReadKey();
            HttpResponseMessage response = await client.GetAsync("/Products");
            Console.ReadKey();
            Console.WriteLine("6");
            if (response.IsSuccessStatusCode)
            {
                products = await response.Content.ReadAsAsync<List<Product>>();
            }
            var prod = products;
            Console.WriteLine("1");
            Console.WriteLine( prod.Count);
            Console.WriteLine("2");
            //prod.ForEach(p => Console.WriteLine(p.Id + p.Name));
            Console.ReadKey();
        }

        public async Task<List<Product>> getProductsREST()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://127.0.0.1:5000/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            List<Product> products = null;
            HttpResponseMessage response = await client.GetAsync("/Products");
            if (response.IsSuccessStatusCode)
            {
                products = await response.Content.ReadAsAsync<List<Product>>();
            }
            List<Product> taskPproducts =  products;
            return taskPproducts;
        }

        private void Compose()
        {
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
