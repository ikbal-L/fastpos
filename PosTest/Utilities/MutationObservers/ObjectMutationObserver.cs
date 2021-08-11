using System;

using System.Collections.Generic;
using System.Linq;
using Utilities.Extensions;
using Utilities.Extensions.Collections;
using Utilities.Extensions.Types;
using Utilities.Attributes;
using Utilities.Mutation;
using Utilities.Mutation.Observers.Extentions;

namespace Utilities.Mutation.Observers
{
    public class ObjectMutationObserver<T> : IObjectMutationObserver<T>
    {
        private readonly Dictionary<string, PropertyMutation> _mutations;
        
        public T Source { get; private set; }

        public bool IsInitialized { get; private set; }

        public ICollection<string> ObservedProperties => _mutations.Keys;

        public bool HasCommitedChanges { get; private set; }

        public PropertyMutation this[string property] => _mutations[property];


        public ObjectMutationObserver(T source, params string[] properties)
        {
            _mutations = new Dictionary<string, PropertyMutation>();
            if (source!= null)
            {
                Init(source, properties);
            }
        }

        private void Init(T source, params string[] properties)
        {
            Source = source;
            if (properties.Length == 0)
            {
                properties = typeof(T).GetPropertyNamesDecoratedBy<ObservePropertyMutationAttribute>().ToArray();

            }
            properties.ToList()
                      .ForEach(propertyName => { _mutations.Add(propertyName, new PropertyMutation(propertyName, source)); });
            IsInitialized = true;
            
        }

        public void Commit()
        {
            this.MethodGuard();
            if (IsInitialized)
            {
                foreach (var pair in _mutations)
                {
                    var (property, mutation) = pair.ToTuple();
                    mutation.Committed = Source.GetPropertyValue(property);
                    mutation.IsCommitted = true;
                } 
            }
            HasCommitedChanges = true;
        }

        public void Push()
        {
            this.MethodGuard();
            foreach (var mutation in _mutations.Values)
            {
                mutation.Initial = mutation.Committed;
                mutation.Committed = null;
                mutation.IsCommitted = false;
            }
            HasCommitedChanges = false;
        }

        public bool IsMutated()
        {
            return _mutations.Values.Any(mutation =>{

                if (mutation.IsCommitted)
                {
                    if (mutation.Initial == null && mutation.Committed == null) return false;
                    if (mutation.Initial == null && mutation.Committed!= null) return true;
                    if (mutation.Initial != null && mutation.Committed== null) return true;
                   return !mutation.Committed.Equals(mutation.Initial);
                }
                return mutation.IsCommitted;
            
            });
        }

        public void LateInit(T source)
        {
            if (IsInitialized) throw new InvalidOperationException($"{this} was already initialized");
            Init(source);
        }
    }

    

   

    public class DiffGenerator<T> where T : new()
    {
        public static T Generate(ObjectMutationObserver<T> mutationObserver, params (string property, IPropertyDiff propertyDiff)[] generator)
        {
            var t = new T();
            generator.ToList().ForEach(p => {

                var mutation = mutationObserver[p.property];
                p.propertyDiff.Invoke(t, mutation);
            });
            return t;
        }


    }

}