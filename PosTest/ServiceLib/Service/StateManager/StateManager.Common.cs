using System;
using System.Collections.Generic;
using ServiceInterface.Interface;
using ServiceInterface.Model;

namespace ServiceLib.Service.StateManager
{
    public partial class StateManager
    {
        public StateManager Manage<TState>(IRepository<TState, long> repository) where TState : IState<long>
        {
            return Manage<TState, long>(repository);
        }

        public static ICollection<TState> Get<TState>(string predicate = "") where TState : IState<long>
        {
            return Get<TState, long>(predicate:predicate);
        }

        public static TState Get<TState>(long identifier) where TState : IState<long>
        {
            return Get<TState, long>(identifier);
        }

     

        public static bool Save<TState>(IEnumerable<TState> state) where TState : IState<long>
        {
            return Save<TState, long>(state);
        }


        public static bool Save<TState>(TState state) where TState : class, IState<long>
        {
            return Save<TState, long>(state);
        }



        private static void IsStateManaged<TState>(Type key) where TState : IState<long>
        {
            IsStateManaged<TState, long>(key);
        }

        public static void Refresh<TState>() where TState : IState<long>
        {
            Refresh<TState, long>();
        }

        public static bool Delete<TState>(TState state) where TState : class, IState<long>
        {
            return Delete<TState, long>(state);
        }
    }

    
}
