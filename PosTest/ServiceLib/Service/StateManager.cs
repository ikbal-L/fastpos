using System;
using System.Collections.Generic;
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

        public StateManager Manage<TState ,TIdentifier>(IRepository<TState, TIdentifier> repository,bool fetch = true) where TState:IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            if (!State.ContainsKey(key))
            {
                State.Add(key,null);
            }

            if (!Service.ContainsKey(key))
            {
                Service.Add(key, repository);
            }

            if (fetch)
            {
                OnfetchRequested +=Fetch<TState,TIdentifier>;
            }
            return _instance;
        }

        public StateManager Manage<TState>(IRepository<TState, long> repository) where TState : IState<long>
        {
           return Manage<TState,long>(repository);
        }


        public static StateManager Instance => _instance??=new StateManager();

        public static ICollection<TState> Get<TState, TIdentifier>() where TState : IState<TIdentifier> where TIdentifier : struct
        {
            var key = typeof(TState);
            if (!State.ContainsKey(key))
            {
                throw new KeyNotFoundException($"Must manage type {typeof(TState)} using a repository of type IRepository<{typeof(TState)},{typeof(TIdentifier)}>");
            }

            if (State[key] == null) Fetch<TState, TIdentifier>();


            return State[typeof(TState)] as ICollection<TState>;
        }

        public static ICollection<TState> Get<TState>() where TState : IState<long>
        {
            return Get<TState, long>();
        }

        public bool Save<TState>()
        {
            return false;
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

        public static void Associate<TMany,TOne>()
        {
            var keyOfTMany = typeof(TMany);
            var keyOfTOne = typeof(TOne);
            if (!State.ContainsKey(keyOfTMany)&& !State.ContainsKey(keyOfTOne)) return;
            var collectionOfTMany = State[keyOfTMany] as ICollection<TMany>;
            var collectionOfTOne = State[keyOfTOne] as ICollection<TOne>;
            Action<ICollection<object>, ICollection<object>> association =  AssociationHelpers.GetAssociation<TMany, TOne>();
            association((ICollection<object>)collectionOfTMany, (ICollection<object>)collectionOfTOne);
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