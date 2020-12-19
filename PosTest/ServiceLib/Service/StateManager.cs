using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Caliburn.Micro;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.helpers;
using Action = System.Action;

namespace ServiceLib.Service
{
    public class StateManager
    {
        private static readonly IDictionary<Type, object> State;
        private static readonly IDictionary<Type, object> Service;
        private Action OnfetchRequested;
        private static Action<ICollection<object>, ICollection<object>> OnAssociationRequested;
        private static StateManager _instance;

        static StateManager()
        {
            Service = new Dictionary<Type, object>();
            State = new Dictionary<Type, object>();
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
                OnfetchRequested += Fetch<TState, TIdentifier>;
            }
            return _instance;
        }

        public StateManager Manage<TState>(IRepository<TState, long> repository) where TState : IState<long>
        {
            return Manage<TState, long>(repository);
        }


        public static StateManager Instance => _instance ??= new StateManager();

        public static ICollection<TState> Get<TState, TIdentifier>() where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            if (!State.ContainsKey(key))
            {
                throw new KeyNotFoundException($"Must manage type {typeof(TState)} using a repository of type IRepository<{typeof(TState)},{typeof(TIdentifier)}>");
            }

            if (State[key] == null) Fetch<TState, TIdentifier>();


            return State[key] as ICollection<TState>;
        }

        public static ICollection<TState> Get<TState>() where TState : IState<long>
        {
            return Get<TState, long>();
        }

        public static TState Get<TState, TIdentifier>(TIdentifier identifier) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            return Get<TState, TIdentifier>().FirstOrDefault(state=> state.Id.Equals(identifier));
        }

        public static TState Get<TState>(long identifier) where TState : IState<long> 
        {
            return Get<TState, long>(identifier);
        }

        public static bool Save<TState, TIdentifier>(TState state) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            var status = -1;
            IEnumerable<string> errors = null;

            if (Service[key] is IRepository<TState, TIdentifier> service)
            {
                if (state.Id == null)
                {
                    status = service.Save(state, out errors);
                }
                else
                {
                    status = service.Update(state, out errors);
                }

                if ((HttpStatusCode)status == HttpStatusCode.OK || (HttpStatusCode)status == HttpStatusCode.Created)
                {
                    if (State[key] is ICollection<TState> tState)
                    {
                        if (!tState.Contains(state)) tState.Add(state);
                    }
                    return true;
                }


            }

            return false;
        }

        public static bool Save<TState>(TState state) where TState : IState<long>
        {
            return Save<TState, long>(state);
        }

        public static bool Save<TState, TIdentifier>(IEnumerable<TState> state) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            var status = -1;
            IEnumerable<string> errors = null;

            if (Service[key] is IRepository<TState, TIdentifier> service)
            {

                status = service.Update(state);

                if ((HttpStatusCode)status == HttpStatusCode.OK || (HttpStatusCode)status == HttpStatusCode.Created) return true;
            }

            return false;
        }

        public static bool Save<TState>(IEnumerable<TState> state) where TState : IState<long>
        {
            return Save<TState, long>(state);
        }

        public static bool Delete<TState, TIdentifier>(TState state) where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            var status = -1;
            IEnumerable<string> errors = null;

            if (Service[key] is IRepository<TState, TIdentifier> service)
            {
                if (state.Id == null)
                {
                    throw new InvalidOperationException("State must have an Id");
                }
                else
                {
                    status = service.Delete((TIdentifier)state.Id);
                }

                if ((HttpStatusCode)status == HttpStatusCode.OK || (HttpStatusCode)status == HttpStatusCode.Created)
                {
                    if (State[key] is ICollection<TState> tState)
                    {
                        if (tState.Contains(state)) tState.Remove(state);
                    }
                    return true;
                }


            }

            return false;
        }

        public static bool Delete<TState>(TState state) where TState : IState<long>
        {
            return Delete<TState, long>(state);
        }
        private static void Fetch<TState, TIdentifier>() where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            if (State[key] != null) return;
            if (Service[key] is IRepository<TState, TIdentifier> service)
            {
                var (status, data) = service.Get();
                //ServiceHelper.HandleStatusCodeErrors();
                if (status != 200) return;
                State[key] = data;

            }
        }

        public void Fetch()
        {
            OnfetchRequested();
        }

        public static void Associate<TMany, TOne>()
        {
            var keyOfTMany = typeof(TMany);
            var keyOfTOne = typeof(TOne);
            if (!State.ContainsKey(keyOfTMany) && !State.ContainsKey(keyOfTOne)) return;
            var collectionOfTMany = State[keyOfTMany] as ICollection<TMany>;
            var collectionOfTOne = State[keyOfTOne] as ICollection<TOne>;
            var association = AssociationHelpers.GetAssociation<TMany, TOne>();
            association(collectionOfTMany, collectionOfTOne);
        }

        private void Flush()
        {
            State.Clear();
        }

        private void Flush<TState>()
        {
            var key = typeof(TState);
            if (State.ContainsKey(key))
            {
                (State[key] as ICollection<TState>)?.Clear();
            }
        }
    }
}