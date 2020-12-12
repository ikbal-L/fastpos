using ServiceInterface.Model;
using System.Collections.Generic;

namespace ServiceInterface.Interface
{
    public interface IAdditiveService
    {
        (int,IEnumerable<Additive>) GetAllAdditives();
        IEnumerable<Additive> GetManyAdditives(IEnumerable<long> ids, ref int statusCode);
        int SaveAdditive(Additive additive, out IEnumerable<string> errors);
        int UpdateAdditive(Additive additive);
        int SaveAdditives(IEnumerable<Additive> additives);

        Additive GetAdditive(long id, ref int statusCode);

        int DeleteAdditive(long idProduct);
    }
}
