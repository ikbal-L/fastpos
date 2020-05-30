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

        //public IEnumerable<Product> GetAllProducts()
        //{
        //    var products = _productService.GetAllProducts();
        //    //var idProducts = new List<long>();
        //    //products.ToList().ForEach(p => idProducts.Add(p.Id));            
        //    var categories = new List<Category>();
        //    foreach (var p in products)
        //    {
        //        Category category;
        //        if (categories.Any(c => c.Id == p.CategorieId))
        //        {
        //            category = categories.Where(c => c.Id == p.CategorieId).First();
        //        }
        //        else
        //        {
        //            category = _categoryService.GetCategory(p.CategorieId);
        //            categories.Add(category);
        //        }
        //        IEnumerable<Additive> additives = null;
        //        if (p is Platter plat && plat.IdAdditives != null)
        //        {
        //            additives = _additiveService.GetManyAdditives(plat.IdAdditives);
        //        }
        //        p.MappingAfterReceiving(category, additives?.ToList());
        //    }

        //    return products;
        //}

        //public IEnumerable<Product> GetAllProducts2()
        //{
        //    var products = _productService.GetAllProducts();
        //    var idProducts = new List<long>();
        //    var idCategories = new HashSet<long>();
        //    products.ToList().ForEach(p => idProducts.Add(p.Id));
        //    products.ToList().ForEach(p => idCategories.Add(p.CategorieId));
            
        //    var categories = _categoryService.GetManyCategories(idCategories);
        //    foreach (var p in products)
        //    {
        //        Category category;
        //        if (categories.Any(c => c.Id == p.CategorieId))
        //        {
        //            category = categories.Where(c => c.Id == p.CategorieId).First();
        //        }
        //        else
        //        {
        //            category = _categoryService.GetCategory(p.CategorieId);
        //        }
        //        IEnumerable<Additive> additives = null;
        //        if (p is Platter plat && plat.IdAdditives != null)
        //        {
        //            additives = _additiveService.GetManyAdditives(plat.IdAdditives);
        //        }
        //        p.MappingAfterReceiving(category, additives?.ToList());
        //    }

        //    return products;
        //}

        public IEnumerable<Product> GetAllProducts()
        {
            IEnumerable<Additive> GetAdditivesFromCollection(IEnumerable<Additive> additives, IEnumerable<long> idAdditves)
            {
                foreach (var id in idAdditves)
                {
                    var additive = additives.Where(a => a.Id == id).FirstOrDefault();
                    if (additive != null)
                    {
                         yield return additive;
                    }
                }
            }
            var t1 = DateTime.Now;
            var products = _productService.GetAllProducts();
            var idProducts = new List<long>();
            var idCategories = new HashSet<long>();
            var idAdditivesOfAllProducts = new HashSet<long>();
            products.ToList().ForEach(p => idProducts.Add(p.Id));
            products.ToList().ForEach(p => idCategories.Add(p.CategorieId));
            foreach (var p in products)
            {
                if(p is Platter plat && plat.IdAdditives != null)
                {
                    plat.IdAdditives.ForEach(id => idAdditivesOfAllProducts.Add(id));
                }
            }

            var additivesOfAllProducts = _additiveService.GetManyAdditives(idAdditivesOfAllProducts);
            var categories = _categoryService.GetManyCategories(idCategories);
            var t2 = DateTime.Now;
            Console.WriteLine($"{t2 - t1}");
            foreach (var p in products)
            {
                Category category;
                if (categories.Any(c => c.Id == p.CategorieId))
                {
                    category = categories.Where(c => c.Id == p.CategorieId).First();
                }
                else
                {
                    category = _categoryService.GetCategory(p.CategorieId);
                }
                IEnumerable<Additive> additives = null;
                if (p is Platter plat && plat.IdAdditives != null)
                {
                    additives = GetAdditivesFromCollection(additivesOfAllProducts, plat.IdAdditives);
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
