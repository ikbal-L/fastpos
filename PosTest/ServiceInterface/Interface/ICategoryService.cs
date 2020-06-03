using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Interface
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetAllCategories(ref int statusCode);
        IEnumerable<Category> GetManyCategories(IEnumerable<long> ids, ref int statusCode);
        int SaveCategory(Category category);
        int UpdateCategory(Category category);
        int SaveCategories(IEnumerable<Category> categories);
        Category GetCategory(long id, ref int statusCode);
        int DeleteCategory(long id);
    }
}
