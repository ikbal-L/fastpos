using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ServiceInterface.Interface;
using ServiceInterface.Model;

namespace ServiceLib.Service.StateManager
{
    public partial class StateManager
    {
        public static async Task FetchAsync<T, Id>(string predicate) where T : IState<Id> where Id : struct
        {
            var key = typeof(T);
            if (FetchLock.Contains(key)) return;
            FetchLock.Add(key);

            if (FetchwithAssociatedTypes.Contains(key))
            {
                var associations = associationManager.GetAssociationsOf<T>();
                if (associations!= null)
                {
                    foreach (var association in associations)
                    {
                        await CallMethodUsingReflection(nameof(FetchAsync), new object[] { null }, association, typeof(long));
                    }
                }

            }
            
            if (State[key] != null && !_refreshRequested) return;
            if (Service[key] is IRepository<T, Id> service)
            {
                var (status, data) = string.IsNullOrEmpty(predicate) ?await service.GetAllAsync() :await service.GetAllAsync(EndPoint.GET_ALL,predicate);
                //var (status, data) = await service.GetAsync();
                //ServiceHelper.HandleStatusCodeErrors();
                FetchLock.Remove(key);
                if ((HttpStatusCode)status != HttpStatusCode.OK && (HttpStatusCode)status != HttpStatusCode.NoContent) return;
                
                if ((HttpStatusCode)status != HttpStatusCode.OK)
                {
                    State[key] = new List<T>();
                }
                else
                {
                    State[key] = data;
                }
                associationManager.Map<T>(State);

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