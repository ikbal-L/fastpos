using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceInterface.Interface
{
    public interface IOrdreItemService : IDisposable
    {
        ICollection<Product> GetAll();

        bool Create(Product product);

        bool Update(Product product);

        bool Delete(int personId);


        List<Product> createProducts();
        Task<List<Product>> getProductsREST();
    }
}
