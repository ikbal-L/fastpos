using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.Exceptions;
using ServiceLib.helpers;
using Action = System.Action;

namespace ServiceLib.Service.StateManager
{
    public partial class StateManager
    {
        private static readonly IDictionary<Type, object> State;
        private static readonly IDictionary<Type, object> Service;
        private static readonly IDictionary<Type, Action> Association;
        private static readonly IList<Type> FetchwithAssociatedTypes;
        private static  Action<string,bool> _onFetchRequested;
        private static bool _refreshRequested = false;
        private static StateManager _instance;
        private static AssociationManager associationManager;
        private static readonly HashSet<Type> FetchLock;
        private static IResponseHandler _responseHandler;

        static StateManager()
        {
            Service = new Dictionary<Type, object>();
            State = new Dictionary<Type, object>();
            Association = new Dictionary<Type, Action>();
            _instance = new StateManager();
            associationManager = AssociationManager.Instance;
            FetchwithAssociatedTypes = new List<Type>();
            FetchLock = new HashSet<Type>();
        }

        public  void HandleErrorsUsing(IResponseHandler responseHandler)
        {
            _responseHandler = responseHandler;
        }
        public StateManager Manage<TState, TIdentifier>(IRepository<TState, TIdentifier> repository, bool fetch = false, string predicate = "", bool withAssociatedTypes = false) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            if (withAssociatedTypes)
            {
                FetchwithAssociatedTypes.Add(key);
            }
            if (!State.ContainsKey(key))
            {
                State.Add(key, null);
            }

            if (!Service.ContainsKey(key))
            {
                Service.Add(key, repository);
            }

            if (fetch)
            {
                _onFetchRequested += Fetch<TState, TIdentifier>;
                if (string.IsNullOrEmpty(predicate))
                {
                    Fetch<TState, TIdentifier>(null,withAssociatedTypes);
                }

            }
            return _instance;
        }


        public static StateManager Instance => _instance ??= new StateManager();

        public static ICollection<TState> Get<TState, TIdentifier>(string predicate = "") where TState : IState<TIdentifier> where TIdentifier : struct 
        {
            var key = typeof(TState);
            IsStateManaged<TState, TIdentifier>(key);

            if (State[key] == null) Fetch<TState, TIdentifier>(predicate);


            return State[key] as ICollection<TState>;
        }

        public static TState Get<TState, TIdentifier>(TIdentifier identifier) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            return Get<TState, TIdentifier>().FirstOrDefault(state=> state.Id.Equals(identifier));
        }

        public static bool Save<TState, TIdentifier>(TState state) where TState : class, IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            IsStateManaged<TState, TIdentifier>(key);

            if (Service[key] is IRepository<TState, TIdentifier> service)
            {
                var status = -1;
                var action = state.Id == null ? StateManagementQuery.Save : StateManagementQuery.Update;
                status = state.Id == null ? service.Save(state, out var errors) : service.Update(state, out errors);
                
                _responseHandler.Handle(status,errors,action,obj:state);
                if ((HttpStatusCode)status == HttpStatusCode.OK || (HttpStatusCode)status == HttpStatusCode.Created)
                {
                    if (State[key] is ICollection<TState> tState)
                    {
                        if (!tState.Contains(state)) tState.Add(state);
                    }

                    //TODO TRACK NULL POINTER EXCEPTION WHEN CREATING A NEW USER
                    if (Association.ContainsKey(key))
                    {
                        Association[key]();
                    }
                    return true;
                }

                if (status == 422)
                {
                    
                }


            }

            return false;
        }
        public static (bool, TReturn) Save<TReturn,TState, TIdentifier>(TState state) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            IsStateManaged<TState, TIdentifier>(key);

            IEnumerable<string> errors = null;

            if (Service[key] is IRepository<TState, TIdentifier> service)
            {
                return state.Id == null? service.Save<TReturn>(state): service.Update<TReturn>(state); ;

            }

            return(false, default(TReturn));
        }

        
        public static (bool,TReturn) SaveAndReturn<TState,TReturn>(TState state) where TState : IState<long>
        {
            return Save<TReturn,TState, long>(state);
        }

        public static bool Save<TState, TIdentifier>(IEnumerable<TState> state) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            IEnumerable<string> errors = null;

            //if (state.Any(tState => tState.Id!= null ))
            //{
            //    throw new ArgumentException("State must be unmanaged");
            //}

            if (Service[key] is IRepository<TState, TIdentifier> service)
            {
                var tState = state.ToList();
                if (tState.All(ts=>ts.Id!= null))
                {
                    var resultOfUpdate = service.Update(tState);
                    if (resultOfUpdate.status!= 200 )
                    {
                        //TODO;
                        
                        return false;
                    }
                    return true;

                }
                var result = service.Save(tState);
                var status = result.status;

                if ((HttpStatusCode) status == HttpStatusCode.OK || (HttpStatusCode) status == HttpStatusCode.Created)
                {
                    State[key] = result.state;
                    return true;
                }
            }
            
            return false;
        }

        public static bool Delete<TState, TIdentifier>(TState state) where TState : class, IState<TIdentifier> where TIdentifier : struct
        {
            
            var key = typeof(TState);
            

            if (!(Service[key] is IRepository<TState, TIdentifier> service)) return false;
            
            if (state.Id == null)
            {
                throw new InvalidOperationException("State must have an Id");
            }

            var (status,errors) = service.Delete((TIdentifier)state.Id);
            _responseHandler.Handle(status,errors,StateManagementQuery.Delete,obj:state);

            if ((HttpStatusCode) status != HttpStatusCode.OK) return false;
            if ((State[key] is ICollection<TState> tState)) 
                   if (tState.Contains(state))
                            tState.Remove(state);
            return true;

        }
        public static (bool,TReturn) DeleteAndRetrun<TState, TReturn>(TState state) where TState : IState<long>
        {
            var key = typeof(TState);
            if (Service[key] is IRepository<TState, long> service)
            {
                var status = -1;
                if (state.Id == null)
                {
                    throw new InvalidOperationException("State must have an Id");
                }

                return service.Delete<TReturn>((long)state.Id);

            }

            return (false, default(TReturn));
        }

        public static bool Delete<TState, TIdentifier>(IEnumerable<TIdentifier> ids) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            IEnumerable<string> errors = null;

            if (!(Service[key] is IRepository<TState, TIdentifier> service)) return false;
            var status = -1;
            if (!ids.Any())
            {
                throw new InvalidOperationException("Set ids to delete");
            }

            status = service.Delete(ids);
            if (status!= 200) return false;

            if ((State[key] is ICollection<TState> tState))
            {
                IEnumerable<TState> stateToRemove;
                foreach (var state in tState.ToList())
                {
                    if (state.Id != null && ids.Contains(state.Id.Value))
                    {
                        tState.Remove(state);
                    }
                }
            }

            return true;
        }

        


       
        private static void Fetch<TState, TIdentifier>(string predicate = null,bool withAssociatedTypes = false) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            //TODO Decide  whether to withAssociatedTypes or FetchWithAssociatedTypes
            if (FetchwithAssociatedTypes.Contains(key))
            {
                var associations = associationManager.GetAssociationsOf<TState>();
                foreach (var association in associations)
                {
                    CallMethodUsingReflection(nameof(Fetch), new object[] {null, false}, association, typeof(long));
                }
                
            }
            if (State[key] != null && !_refreshRequested) return;
            if (Service[key] is IRepository<TState, TIdentifier> service)
            {
                var (status, data) = string.IsNullOrEmpty(predicate) ? service.Get() : service.Get(predicate);
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

        [Conditional("DEBUG")]
        private static void IsStateManaged<TState, TIdentifier>(Type key) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            if (!State.ContainsKey(key))
            {
                throw new StateNotManagedException<TState>();
            }
        }

        public static void Fetch()
        {
            _onFetchRequested?.Invoke(null,false);
        }

        public static void Associate<TMany, TOne>()
        {
            var keyOfTMany = typeof(TMany);
            var keyOfTOne = typeof(TOne);
            if (!State.ContainsKey(keyOfTMany) && !State.ContainsKey(keyOfTOne)) return;
            Action act = GetAssociation<TMany, TOne>();
            if (!Association.ContainsKey(keyOfTOne))
            {
                Association.Add(keyOfTOne, act);
            }
            else
            {
                if (!Association[keyOfTOne].GetInvocationList().Contains(act))
                {
                    Association[keyOfTOne] += act; 
                }
            }

            if (!Association.ContainsKey(keyOfTMany))
            {
                Association.Add(keyOfTMany, act);
            }
            else
            {
                if (!Association[keyOfTMany].GetInvocationList().Contains(act))
                {
                    Association[keyOfTMany] += act; 
                }
            }

            Association[keyOfTOne]();
        }

        private static Action GetAssociation<TMany, TOne>()
        {
            return () =>
            {
                var keyOfTMany = typeof(TMany);
                var keyOfTOne = typeof(TOne);

                var collectionOfTMany = State[keyOfTMany] as ICollection<TMany>;
                var collectionOfTOne = State[keyOfTOne] as ICollection<TOne>;

                if (collectionOfTMany == null || collectionOfTOne == null) return;
                var association = AssociationHelpers.GetAssociation<TMany, TOne>();
                association(collectionOfTMany, collectionOfTOne);
            };
        }

        public static void Flush()
        {
            State.Clear();
            FetchLock.Clear();

        }

        public static void Flush<TState>() where TState:IState<long>
        {
            var key = typeof(TState);
            IsStateManaged<TState>(key);
            if (State.ContainsKey(key))
            {
                (State[key] as ICollection<TState>)?.Clear();
            }
        }

        public static void Refresh()
        {
            _refreshRequested = true;
            Fetch();
            _refreshRequested = false;
        }

        public static void Refresh<TState, TIdentifier>() where TState : IState<TIdentifier> where TIdentifier : struct
        {
            _refreshRequested = true;
            Fetch<TState,TIdentifier>();
            _refreshRequested = false;

            var key = typeof(TState);
            Association[key]();
        }

        
        public static TService GetService<TState, TService>() {
            return (TService) Service[typeof(TState)];
        }

        public static async Task CallMethodUsingReflection(string methodName, object[] methodArgs,params Type [] methodTypeArgs)
        {
            var stateManagerType =typeof(StateManager);
            var targetMethodInfo =stateManagerType.GetMethod(methodName);
            var targetMethod = targetMethodInfo?.MakeGenericMethod(methodTypeArgs);
            var task =(Task) targetMethod?.Invoke(null, methodArgs);
            if (task != null) await task.ConfigureAwait(false);
        }
    }


    
}