using ServiceInterface.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
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
        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
        }

        void Run()
        {
            Compose();
            Console.WriteLine(message+ service.Products.Count);
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
