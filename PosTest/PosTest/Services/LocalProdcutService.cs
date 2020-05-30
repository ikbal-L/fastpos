using ServiceInterface.Interface;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.Services
{
    public class LocalProdcutService
    {
        [Import(typeof(IProductService))]
        private IProductService _productService;

        [Import(typeof(IAdditiveService))]
        private IAdditiveService _additiveService;

        [Import(typeof(ICategoryService))]
        private ICategoryService _categoryService;

        private static LocalProdcutService _instance;
        public static LocalProdcutService Instance => _instance ?? (_instance = new LocalProdcutService());

        public LocalProdcutService()
        {
            Compose();
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

        public IEnumerable<Product> GetAllProducts()
        {
            var products = _productService.GetAllProducts();
            //var idProducts = new List<long>();
            //products.ToList().ForEach(p => idProducts.Add(p.Id));

            foreach (var p in products)
            {
                var category = _categoryService.GetCategory(p.CategorieId);
                IEnumerable<Additive> additives = null;
                if (p is Platter plat && plat.Additives != null)
                {
                    additives = _additiveService.GetManyAdditives(plat.IdAdditives);
                }
                p.MappingAfterReceiving(category, additives?.ToList());
            }

            return products;
        }

       

        bool SaveProduct(Product product)
        {
            throw new NotImplementedException();
        }
        public bool SaveProducts(IEnumerable<Product> products)
        {
            
            //var idProducts = new List<long>();
            products.ToList().ForEach(p => p.MappingBeforeSending());

            //foreach (var p in products)
            //{
            //    if (p is Platter plat)
            //    {

            //        p.MappingBeforeSending();

            //    }
            //}

            return _productService.SaveProducts(products);
            
        }

        Product GetProduct(long id)
        {
            throw new NotImplementedException();
        }

        bool DeleteProduct(long idProduct)
        {
            throw new NotImplementedException();
        }

        public bool SaveCategories(IEnumerable<Category> categories)
        {
            categories.ToList().ForEach(c => c.MappingBeforeSending());
            return _categoryService.SaveCategories(categories);
        }
    }
}
