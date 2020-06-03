using ServiceInterface.Model;
using System.Collections.Generic;

namespace ServiceInterface.Interface
{
    public interface IAdditiveService
    {
        IEnumerable<Additive> GetAllAdditives(ref int statusCode);
        IEnumerable<Additive> GetManyAdditives(IEnumerable<long> ids, ref int statusCode);
        int SaveAdditive(Additive additive);
        int UpdateAdditive(Additive additive);
        int SaveAdditives(IEnumerable<Additive> additives);

        Additive GetAdditive(long id, ref int statusCode);

        int DeleteAdditive(long idProduct);
    }
}
