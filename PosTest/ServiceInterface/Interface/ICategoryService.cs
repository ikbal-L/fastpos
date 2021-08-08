﻿using ServiceInterface.Model;
using System.Collections.Generic;

namespace ServiceInterface.Interface
{
    public interface ICategoryService
    {
        (int Status,IEnumerable<Category> Categories)GetAllCategories();
        ((int CategoryStatus,int ProductStatus),(IEnumerable<Category>, IEnumerable<Product>)) GetAllCategoriesAndProducts();
        (int,IEnumerable<Category>) GetManyCategories(IEnumerable<long?> ids);
        int SaveCategory(Category category,out IEnumerable<string> errors);
        int UpdateCategory(Category category , out IEnumerable<string> errors);
        int SaveCategories(IEnumerable<Category> categories);
        (int,Category) GetCategory(long id);
        int DeleteCategory(long id);
    }
}
