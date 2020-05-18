using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Interface
{
    public interface IProductService : IDisposable
    {
        ICollection<Product> GetAllProducts();

        List<Product> createProducts();
        Task<List<Product>> getProductsREST();

<<<<<<< HEAD
        bool SaveProduct(Product product);
        bool SaveProducts(IEnumerable<Product> products);

        Product GetProduct(long id);
=======
        void DeleteProduct(long idProduct);
        
        void SaveProduct(Product product);


>>>>>>> products
    }
}
