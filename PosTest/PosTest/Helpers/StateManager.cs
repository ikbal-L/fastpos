using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ServiceInterface.Model;
using ServiceLib.Service;

namespace PosTest.Helpers
{
    public class StateManager
    {
        private static readonly IDictionary<Type, object> State;
        private static readonly IDictionary<Type, object> Service;
        private static StateManager _instance;

        static StateManager()
        {
            Service = new Dictionary<Type, object>();
            State = new Dictionary<Type, object>();
            _instance = new StateManager();
        }

        private StateManager()
        {

        }
        public StateManager Manage<TState ,TIdentifier>(IRepository<TState, TIdentifier> repository) where TState:IState<TIdentifier> where TIdentifier : struct
        {
            if (!State.ContainsKey(typeof(TState)))
            {
                State.Add(typeof(TState),null);
            }

            if (!Service.ContainsKey(typeof(TState)))
            {
                Service.Add(typeof(TState), repository);
            }

            return _instance;
        }

        public static StateManager Instance => _instance??=new StateManager();

        public static ICollection<TState> Get<TState>()
        {
            return State[typeof(TState)] as ICollection<TState>;
        }

        public bool Save<TState>()
        {
            return false;
        }

        private void Fetch<TState, TIdentifier>() where TState : IState<TIdentifier> where TIdentifier : struct
        {
            if (State[typeof(TState)] != null) return;
            if (Service[typeof(TState)] is IRepository<TState, TIdentifier> service)
            {
                var (status, data) = service.Get();
                //ServiceHelper.HandleStatusCodeErrors();
                if (status != 200) return;
                
            }
        }   
    }
}