using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceLib.helpers
{
    public partial class AssociationManager 
    {

        private readonly Dictionary<Type, Dictionary<Type, Action<IEnumerable<object>, IEnumerable<object>>>> _associations;
        
        private AssociationManager()
        {
            _associations = new Dictionary<Type, Dictionary<Type, Action<IEnumerable<object>, IEnumerable<object>>>>();
        }

        public static readonly AssociationManager Instance = new AssociationManager();

        public Action<IEnumerable<TOne>, IEnumerable<TMany>> GetMapper<TOne,TMany>()
        {
            if (_associations[typeof(TOne)][typeof(TMany)] is Action<IEnumerable<TOne>, IEnumerable<TMany>> action)
            {
                return action;
            }

            return null;
        }

        public void Map<TOne>(IDictionary<Type,object> state)
        {
            var keyOfTOne = typeof(TOne);
            if (!_associations.ContainsKey(keyOfTOne) || _associations[keyOfTOne] == null ||
                !_associations[keyOfTOne].Any()) return;
            var collectionOfTOne = state[keyOfTOne] as ICollection<TOne>;
            foreach (var keyOfTMany in _associations[keyOfTOne].Keys)
            {
                var collectionOfTMany = state[keyOfTMany] ;
                var map = _associations[keyOfTOne][keyOfTMany];
                map((IEnumerable<object>) collectionOfTOne, (IEnumerable<object>) collectionOfTMany);

            }
        }

        public void Map<TOne>(ICollection<TOne> collectionOfTOne, IDictionary<Type, object> state)
        {
            var keyOfTOne = typeof(TOne);
            if (!_associations.ContainsKey(keyOfTOne) || _associations[keyOfTOne] == null ||
                !_associations[keyOfTOne].Any()) return;

            foreach (var keyOfTMany in _associations[keyOfTOne].Keys)
            {
                var collectionOfTMany = state[keyOfTMany];
                var map = _associations[keyOfTOne][keyOfTMany];
                map((IEnumerable<object>)collectionOfTOne, (IEnumerable<object>)collectionOfTMany);

            }
        }

        public List<Type> GetAssociationsOf<T>()
        {
            var key = typeof(T);
            if (!_associations.ContainsKey(key)) return null;
            var associationsOfT = _associations[key].Keys.ToList();
            return associationsOfT;

        }

        public void Associate<TOne, TMany>(Action<IEnumerable<TOne>, IEnumerable<TMany>> map)
        {
            var keyOfTOne = typeof(TOne);
            var KeyOfTMany = typeof(TMany);

            Action<IEnumerable<object>, IEnumerable<object>> mapWrapper = (one, many) =>
            {
                if (one is IEnumerable<TOne> enumerablOfTOne && many is IEnumerable<TMany> enumerablOfTMany)
                {
                    map(enumerablOfTOne, enumerablOfTMany);
                }
                else
                {
#if DEBUG
                //    throw new InvalidOperationException(
                //$"Expected{typeof(TOne)} got type of {one.GetType()},Expected{typeof(TMany)} got type of {many.GetType()}"); 
#endif
                }
            };
            if (!_associations.ContainsKey(keyOfTOne))
            {
                _associations.Add(keyOfTOne, new Dictionary<Type, Action<IEnumerable<object>, IEnumerable<object>>>());
                
            }
            _associations[keyOfTOne].Add(KeyOfTMany, mapWrapper);

        }


    }



}