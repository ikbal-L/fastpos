﻿using ServiceInterface.Model;
using System.Collections.Generic;

namespace ServiceInterface.Interface
{
    public interface IProductService
    {
        ICollection<Product> GetAllProducts(ref int statusCode);
        int SaveProduct(Product product, out IEnumerable<string> errors);
        int UpdateProduct(Product product, out IEnumerable<string> errors);
        int SaveProducts(IEnumerable<Product> products);
        Product GetProduct(long id, ref int statusCode);
        int DeleteProduct(long idProduct);

        (IEnumerable<long>, int) UpdateManyProducts(IEnumerable<Product> products);
    }
}
