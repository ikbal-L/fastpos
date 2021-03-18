using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using Caliburn.Micro;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.Exceptions;
using ServiceLib.helpers;
using Action = System.Action;

namespace ServiceLib.Service
{
    public partial class StateManager
    {
        private static readonly IDictionary<Type, object> State;
        private static readonly IDictionary<Type, object> Service;
        private static readonly IDictionary<Type, Action> Association;
        private static  Action _onFetchRequested;
        private static bool _refreshRequested = false;
        private static Action<ICollection<object>, ICollection<object>> OnAssociationRequested;
        private static StateManager _instance;

        static StateManager()
        {
            Service = new Dictionary<Type, object>();
            State = new Dictionary<Type, object>();
            Association = new Dictionary<Type, Action>();
            _instance = new StateManager();
        }

        public StateManager Manage<TState, TIdentifier>(IRepository<TState, TIdentifier> repository, bool fetch = true) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
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
                Fetch<TState, TIdentifier>();

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

        public static bool Save<TState, TIdentifier>(TState state) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            IsStateManaged<TState, TIdentifier>(key);

            IEnumerable<string> errors = null;

            if (Service[key] is IRepository<TState, TIdentifier> service)
            {
                var status = -1;
                status = state.Id == null ? service.Save(state, out errors) : service.Update(state, out errors);

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

        public static bool Save<TState>(TState state) where TState : IState<long>
        {
            return Save<TState, long>(state);
        }
        public static (bool,TReturn) SaveAndReturn<TState,TReturn>(TState state) where TState : IState<long>
        {
            return Save<TReturn,TState, long>(state);
        }

        public static bool Save<TState, TIdentifier>(IEnumerable<TState> state) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            IEnumerable<string> errors = null;

            if (Service[key] is IRepository<TState, TIdentifier> service)
            {
                var status = service.Update(state);

                if ((HttpStatusCode)status == HttpStatusCode.OK || (HttpStatusCode)status == HttpStatusCode.Created) return true;
            }

            return false;
        }

        public static bool Delete<TState, TIdentifier>(TState state) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            IEnumerable<string> errors = null;

            if (!(Service[key] is IRepository<TState, TIdentifier> service)) return false;
            var status = -1;
            if (state.Id == null)
            {
                throw new InvalidOperationException("State must have an Id");
            }

            status = service.Delete((TIdentifier)state.Id);

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
                else
                {
                    return service.Delete<TReturn>((long)state.Id);
                }

            }

            return (false, default(TReturn));
        }
        public static bool Delete<TState>(TState state) where TState : IState<long>
        {
            return Delete<TState, long>(state);
        }
        private static void Fetch<TState, TIdentifier>() where TState : IState<TIdentifier> where TIdentifier : struct
        {
            Fetch<TState,TIdentifier>(null);
        }
        private static void Fetch<TState, TIdentifier>(string predicate) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            if (State[key] != null && !_refreshRequested) return;
            if (Service[key] is IRepository<TState, TIdentifier> service)
            {
                var (status, data) = string.IsNullOrEmpty(predicate)?service.Get(): service.Get(predicate);
                //ServiceHelper.HandleStatusCodeErrors();
                if ((HttpStatusCode)status != HttpStatusCode.OK && (HttpStatusCode)status != HttpStatusCode.NoContent) return;
                State[key] = data;

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
            _onFetchRequested();
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
    }


    
}