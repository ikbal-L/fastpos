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
        IEnumerable<Category> GetAllCategories();
        IEnumerable<Category> GetManyCategories(IEnumerable<long> ids);
        bool SaveCategory(Category category);
        bool UpdateCategory(Category category);
        bool SaveCategories(IEnumerable<Category> categories);
        Category GetCategory(long id);
        bool DeleteCategory(long id);
    }
}
