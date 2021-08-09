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
        private Dictionary<object, Mutation> _values;
      
        public Mutations(params string [] props)
        {
            _values = new Dictionary<object, Mutation>();
            foreach (var prop in props)
            {
                _values.Add(prop,new Mutation() );
            }
        }
        public bool InitCommit(object prop, object value)
        {
            _values[prop] = new Mutation() { Initial= value};
            return true;
        }
        public void Commit(object prop, object value)
        {
            var mutation = _values[prop];
            mutation.Commited = value; 
            mutation.HasCommitted = true;
            _values[prop] = mutation;
        }
        public void Push()
        {
            foreach (var key in _values.Keys.ToList())
            {
                var mutation = _values[key];
                mutation.Initial = mutation.Commited;
                mutation.Commited = null;
                mutation.HasCommitted = false;
            }
        }

        public bool IsMutated(object prop)
        {
            var mutation = _values[prop];
            return mutation.HasCommitted&&!mutation.Initial.Equals(mutation.Commited);
        }
        
    }


    public class Mutation
    {
    
        public object Initial { get; set; }

        public object Commited { get; set; }

        public bool HasCommitted { get; set; } = false;
    }  
}
