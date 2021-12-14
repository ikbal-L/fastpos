using System;
using System.Collections;
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

        private string _baseUrl;

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

        public StateManager BaseUrl(string url)
        {
            _baseUrl = url;
            return this;
        }
        public StateManager Manage<T, Id>(IRepository<T, Id> repository, bool fetch = false, string predicate = "", bool withAssociatedTypes = false) where T : IState<Id> where Id : struct
        {
            var key = typeof(T);
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
                repository.BaseUrl = _baseUrl;
                Service.Add(key, repository);
            }

            if (fetch)
            {
                _onFetchRequested += Fetch<T, Id>;
                if (string.IsNullOrEmpty(predicate))
                {
                    Fetch<T, Id>(null,withAssociatedTypes);
                }

            }
            return _instance;
        }


        public static StateManager Instance => _instance ??= new StateManager();

        public static ICollection<T> Get<T, Id>() where T : IState<Id> where Id : struct 
        {
            var key = typeof(T);
            IsStateManaged<T, Id>(key);

            if (State[key] == null) Fetch<T, Id>("");


            return State[key] as ICollection<T>;
        }

        public static TState GetById<TState, Id>(Id id) where TState : IState<Id> where Id : struct
        {
            return Get<TState, Id>().FirstOrDefault(state=> state.Id.Equals(id));
        }

        public static bool Save<T, Id>(T state) where T : class, IState<Id> where Id : struct
        {
            var key = typeof(T);
            IsStateManaged<T, Id>(key);

            if (Service[key] is IRepository<T, Id> service)
            {

                var action = state.Id == null ? StateManagementQuery.Save : StateManagementQuery.Update;
                var (status,errors) = state.Id == null ? service.Save(state) : service.Update(state);
                
                _responseHandler.Handle(status,errors,action,obj:state);
                if ((HttpStatusCode)status == HttpStatusCode.OK || (HttpStatusCode)status == HttpStatusCode.Created)
                {
                    if (State[key] is ICollection<T> tState)
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


        public static bool Save<T, Id>(IEnumerable<T> state) where T : IState<Id> where Id : struct
        {
            var key = typeof(T);

            if (Service[key] is IRepository<T, Id> service)
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
                var result = service.SaveAll(tState);
                var status = result.status;

                if ((HttpStatusCode) status == HttpStatusCode.OK || (HttpStatusCode) status == HttpStatusCode.Created)
                {
                    State[key] = result.state;
                    return true;
                }
            }
            
            return false;
        }

        public static bool Delete<T, Id>(T state) where T : class, IState<Id> where Id : struct
        {
            
            var key = typeof(T);
            

            if (!(Service[key] is IRepository<T, Id> service)) return false;
            
            if (state.Id == null)
            {
                throw new InvalidOperationException("State must have an Id");
            }

            var (status,errors) = service.DeleteById((Id)state.Id);
            _responseHandler.Handle(status,errors,StateManagementQuery.Delete,obj:state);

            if ((HttpStatusCode) status != HttpStatusCode.OK) return false;
            if ((State[key] is ICollection<T> tState)) 
                   if (tState.Contains(state))
                            tState.Remove(state);
            return true;

        }

        public static bool Delete<T, Id>(IEnumerable<Id> ids) where T : IState<Id> where Id : struct
        {
            var key = typeof(T);
            IEnumerable<string> errors = null;

            if (!(Service[key] is IRepository<T, Id> service)) return false;
            var status = -1;
            if (!ids.Any())
            {
                throw new InvalidOperationException("Set ids to delete");
            }

            status = service.Delete(ids);
            if (status!= 200) return false;

            if ((State[key] is ICollection<T> tState))
            {
                IEnumerable<T> stateToRemove;
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

        private static void Fetch<T, Id>(string predicate = null,bool withAssociatedTypes = false) where T : IState<Id> where Id : struct
        {
            var key = typeof(T);
            //TODO Decide  whether to withAssociatedTypes or FetchWithAssociatedTypes
            if (FetchwithAssociatedTypes.Contains(key))
            {
                var associations = associationManager.GetAssociationsOf<T>();
                foreach (var association in associations)
                {
                    CallMethodUsingReflection(nameof(Fetch), new object[] {null, false}, association, typeof(long));
                }
                
            }
            if (State[key] != null && !_refreshRequested) return;
            if (Service[key] is IRepository<T, Id> service)
            {
                var (status, data) = string.IsNullOrEmpty(predicate) ? service.GetAll() : service.Get(predicate);
                //ServiceHelper.HandleStatusCodeErrors();
                if ((HttpStatusCode)status != HttpStatusCode.OK && (HttpStatusCode)status != HttpStatusCode.NoContent) return;
                if ((HttpStatusCode)status == HttpStatusCode.NoContent)
                {
                    State[key] = new List<T>();
                }
                else
                {
                    State[key] = data;
                }

            }


        }

        [Conditional("DEBUG")]
        private static void IsStateManaged<T, Id>(Type key) where T : IState<Id> where Id : struct
        {
            if (!State.ContainsKey(key))
            {
                throw new StateNotManagedException<T>();
            }
        }

        public static void Fetch()
        {
            _onFetchRequested?.Invoke(null,false);
        }

        public static void Flush()
        {
            State.Clear();
            FetchLock.Clear();

        }

        public static void Flush<T>() where T:IState<long>
        {
            var key = typeof(T);
            IsStateManaged<T>(key);
            if (State.ContainsKey(key))
            {
                //(State[key] as ICollection<TState>)?.Clear();
                State[key]  = null;
            }
        }

        public static void Refresh()
        {
            _refreshRequested = true;
            Fetch();
            _refreshRequested = false;
        }

        public static void Refresh<T, Id>() where T : IState<Id> where Id : struct
        {
            _refreshRequested = true;
            Fetch<T,Id>();
            _refreshRequested = false;

            var key = typeof(T);
            Association[key]();
        }

        
        public static TService GetService<TState, TService>() {
            return (TService) Service[typeof(TState)];
        }

        public static IRepository<T,Id> GetRepository<T,Id>() where T:IState<Id> where Id:struct
        {
            if (Service[typeof(T)] is IRepository<T, Id> repository) return repository;
            return default(IRepository<T, Id>);
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