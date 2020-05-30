using ServiceInterface.Model;
using System.Collections.Generic;

namespace ServiceInterface.Interface
{
    public interface IAdditiveService
    {
        IEnumerable<Additive> GetManyAdditives(IEnumerable<long> ids);
        bool SaveAdditive(Additive additive);
        bool UpdateAdditive(Additive additive);
        bool SaveAdditives(IEnumerable<Additive> additives);

        Additive GetAdditive(long id);

        bool DeleteAdditive(long idProduct);
    }
}
