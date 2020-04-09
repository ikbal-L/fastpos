using ServiceInterface.Interface;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;

using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLib.Service
{
    [Export(typeof(ICategorieService))]
    class CategorieService : ICategorieService
    {
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public ICollection<Categorie> GetAllCategory()
        {
            return FakeServices.Category;
        }
    }
}
