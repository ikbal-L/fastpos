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

        void DeleteProduct(long idProduct);
    }
}
