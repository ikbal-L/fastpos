using System;
using System.Collections.Generic;
using ServiceInterface.Interface;
using ServiceInterface.Model;

namespace ServiceLib.Service.StateManager
{
    public partial class StateManager
    {
        public StateManager Manage<T>(IRepository<T, long> repository) where T : IState<long>
        {
            return Manage<T, long>(repository);
        }

        public static ICollection<T> GetAll<T>() where T : IState<long>
        {
            return GetAll<T, long>();
        }

        public static T GetById<T>(long id) where T : IState<long>
        {
            return GetById<T, long>(id);
        }

        public static bool SaveAll<T>(IEnumerable<T> state) where T : IState<long>
        {
            return SaveAll<T, long>(state);
        }

        public static bool Save<T>(T state) where T : class, IState<long>
        {
            return Save<T, long>(state);
        }

        private static void IsStateManaged<T>(Type key) where T : IState<long>
        {
            IsStateManaged<T, long>(key);
        }

        public static void Refresh<T>() where T : IState<long>
        {
            Refresh<T, long>();
        }

        public static bool Delete<T>(T state) where T : class, IState<long>
        {
            return Delete<T, long>(state);
        }

        public IRepository<T,long> GetRepository<T>() where T : IState<long>
        {
            return GetRepository<T, long>();
        }


    }
 
}
