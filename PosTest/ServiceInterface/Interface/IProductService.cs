using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Interface
{
    public interface IProductService
    {
        ICollection<Product> GetAllProducts(ref int statusCode);
        int SaveProduct(Product product,ref long Id);
        int UpdateProduct(Product product);
        int SaveProducts(IEnumerable<Product> products);
        Product GetProduct(long id, ref int statusCode);
        int DeleteProduct(long idProduct);

        (IEnumerable<long>, int) UpdateManyProducts(IEnumerable<Product> products);
    }
}
