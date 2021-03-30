using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ServiceInterface.Interface;
using ServiceInterface.Model;

namespace ServiceLib.Service.StateManager
{
    public partial class StateManager
    {
        private static async Task FetchAsync<TState, TIdentifier>(string predicate) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            if (State[key] != null && !_refreshRequested) return;
            if (Service[key] is IRepository<TState, TIdentifier> service)
            {
                //var (status, data) = string.IsNullOrEmpty(predicate) ? service.GetAsync() : service.Get(predicate);
                var (status, data) = await service.GetAsync();
                //ServiceHelper.HandleStatusCodeErrors();
                if ((HttpStatusCode)status != HttpStatusCode.OK && (HttpStatusCode)status != HttpStatusCode.NoContent) return;
                if ((HttpStatusCode)status == HttpStatusCode.NoContent)
                {
                    State[key] = new List<TState>();
                }
                else
                {
                    State[key] = data;
                }

            }
        }

        public static async Task<ICollection<TState>> GetAsync<TState, TIdentifier>(string predicate = "") where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            IsStateManaged<TState, TIdentifier>(key);

            if (State[key] == null) await FetchAsync<TState, TIdentifier>(predicate);


            return State[key] as ICollection<TState>;
        }

        
    }
}