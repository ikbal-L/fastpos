using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Interface
{
    public interface IOrderService : IDisposable
    {
        ICollection<Product> GetAll();

        bool Create(Product product);

        bool Update(Product product);

        bool Delete(int personId);


        List<Product> createProducts();
        Task<List<Product>> getProductsREST();
    }
}
