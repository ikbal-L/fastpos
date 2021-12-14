using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceInterface.Model;

namespace ServiceInterface.Interface
{
    public interface IRepositoryAsync<T, in Id> where T : IState<Id> where Id : struct
    {
        Task<(int status, T resource)> GetByIdAsync(Id id);     
    }
}