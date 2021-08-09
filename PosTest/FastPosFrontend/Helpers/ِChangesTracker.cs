using ServiceInterface.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastPosFrontend.Helpers
{
     public class MutationObserver
    {
        Dictionary<object,Mutations> observables;
        Dictionary<object, ICollection> ObservableCollections;
        public MutationObserver()
        {
            observables = new Dictionary<object, Mutations>();
            ObservableCollections = new Dictionary<object, ICollection>();
        }
        public void Observe(object observable, params string [] properties)
        {
            if (!observables.ContainsKey(observable))
            {
                observables.Add(observable, new Mutations(properties));
            }
        }
        public void ObserveCollection<T>(object key,ICollection<T> collection)
        {
            ObservableCollections.Add(key, collection.ToList());
        }

        public ICollection<T> GetAddedItems<T>(object key,ICollection<T> mutatedCollection)
        {
            var collection = ObservableCollections[key].Cast<T>();
            return mutatedCollection.Except<T>(collection).ToList();
        }

        public ICollection<T> GetRemovedItems<T>(object key, ICollection<T> mutatedCollection)
        {
            var collection = ObservableCollections[key].Cast<T>();
            return collection.Except<T>(mutatedCollection).ToList();
        }



        public bool InitCommit(object source,string prop)
        {
            var value = source.GetType().GetProperty(prop).GetValue(source);
            observables[source].InitCommit(prop, value);
            return true;
        }
        public void Commit(object source, object prop, object value)
        {
            observables[source].Commit(prop, value);
        }
        public void Push(object source)
        {
            observables[source].Push();
        }

        public bool IsMutated(object source, object prop)
        {
            return observables[source].IsMutated(prop);
        }



    }

    public class Mutations
    {
        private Dictionary<object, ( object initial,object commited)> _values;
        private bool initCommit = false;
        public Mutations(params object [] props)
        {
            _values = new Dictionary<object, (object initial, object commited)>();
            foreach (var prop in props)
            {
                _values.Add(prop, (null,null));
            }
        }
        public bool InitCommit(object prop,object value)
        {
            _values[prop] = (value, value);
            return true;
        }
        public void Commit(object prop, object value)
        {
            var mutation = _values[prop];
            mutation.commited = value;
            _values[prop] = mutation;
        }
        public void Push()
        {
            foreach (var kv in _values)
            {
                var (_,commited) = _values[kv.Key];
                _values[kv.Key] = (commited, null);
            }
        }

        public bool IsMutated(object prop)
        {
            var(initial,commited) = _values[prop];
            return !initial.Equals(commited);
        }
        
    }
    
}
