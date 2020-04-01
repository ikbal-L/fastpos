using ServiceInterface.Interface;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLib.Service
{
    [Export(typeof(IProductService))]
    class ProductService : IProductService
    {
        private List<Product> _products;

        public List<Product> Products { get => createProducts(); set => _products = value; }

        List<Product> createProducts()
        {
            Product p1 = new Product { Id =1, Name = "AAAA", Type = "A", Color = "blue" };
            Product p2 = new Product { Id =2, Name = "BBBB", Type = "A", Color = "white" };
            Product p3 = new Product { Id =3, Name = "CCCC", Type = "B", Color = "gray" };
            var products = new List<Product>();
            products.Add(p1);
            products.Add(p2);
            products.Add(p3);
            return products;
        }
    }
}
