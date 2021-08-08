using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceInterface.Model;

namespace ServiceLib.Service.StateManager
{
    public  partial class StateManager
    {
        public static async Task<ICollection<TState>> GetAsync<TState>(string predicate = "") where TState : IState<long>
        {
            
            return await GetAsync<TState, long>(predicate).ConfigureAwait(false);
        }
    }
}