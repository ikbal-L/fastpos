using ServiceInterface.Interface;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        //[Import]
        string message;

        [Import(typeof(IProductService))]
        IProductService service;

        static HttpClient client = new HttpClient();

        static async Task<List<Product>> GetProductAsync(string path)
        {
            List<Product> products = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                products = await response.Content.ReadAsAsync<List<Product>> ();
            }
            return products;
        }


        static void Main(string[] args)
        {/*
            client.BaseAddress = new Uri("http://127.0.0.1:8080/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Task<List<Product>> taskPproducts =  GetProductAsync("/Products");
            taskPproducts.Wait();
            List<Product> products = taskPproducts.Result;

            products.ForEach(p => Console.WriteLine(p.Id + p.Name));

            Console.ReadKey();*/
            Program p = new Program();
            p.Run();
        }

        void Run()
        {
            Compose();
            Console.WriteLine(message+ service.getProductsREST().Count);
            service.getProductsREST().ForEach(p => Console.WriteLine(p.Id + p.Name));
            Console.ReadKey();
            
        }

        private void Compose()
        {
            //AssemblyCatalog catalog1 = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
            AssemblyCatalog catalog = new AssemblyCatalog("ServiceLib.dll");
            CompositionContainer container = new CompositionContainer(catalog);
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
