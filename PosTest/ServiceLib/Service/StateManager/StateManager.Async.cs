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
        public static async Task FetchAsync<TState, TIdentifier>(string predicate) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            if (FetchLock.Contains(key)) return;
            FetchLock.Add(key);
            //Decide on  either withAssociatedTypes or FetchWithAssociatedTypes
            //Bug if multiple tasks of GetAsync are executing data could be fetched multiple times
            if (FetchwithAssociatedTypes.Contains(key))
            {
                var associations = associationManager.GetAssociationsOf<TState>();
                if (associations!= null)
                {
                    foreach (var association in associations)
                    {
                        await CallMethodUsingReflection(nameof(FetchAsync), new object[] { null }, association, typeof(long));
                    }
                }

            }
            
            if (State[key] != null && !_refreshRequested) return;
            if (Service[key] is IRepository<TState, TIdentifier> service)
            {
                var (status, data) = string.IsNullOrEmpty(predicate) ?await service.GetAsync() :await service.GetAsync(predicate);
                //var (status, data) = await service.GetAsync();
                //ServiceHelper.HandleStatusCodeErrors();
                FetchLock.Remove(key);
                if ((HttpStatusCode)status != HttpStatusCode.OK && (HttpStatusCode)status != HttpStatusCode.NoContent) return;
                
                if ((HttpStatusCode)status != HttpStatusCode.OK)
                {
                    State[key] = new List<TState>();
                }
                else
                {
                    State[key] = data;
                }
                associationManager.Map<TState>(State);

            }
        }

        public static async Task<ICollection<TState>> GetAsync<TState, TIdentifier>(string predicate = "") where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            IsStateManaged<TState, TIdentifier>(key);

            if (State[key] == null) await FetchAsync<TState, TIdentifier>(predicate);

            Console.WriteLine($"Thread {Thread.CurrentThread.GetHashCode()} Time {DateTime.Now.TimeOfDay}");
            return State[key] as ICollection<TState>;
        }

        
    }
}