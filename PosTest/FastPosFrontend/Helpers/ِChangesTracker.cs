using FastPosFrontend.Extensions;
using System;

using System.Collections.Generic;
using System.Linq;



namespace FastPosFrontend.Helpers
{
    public class Mutation
    {
        public Mutation(string propertyName, object mutatedObject)
        {
            PropertyName = propertyName;
            Initial = mutatedObject.GetPropertyValue(propertyName);
        }

        public string PropertyName { get; }
        public object Initial { get; set; }

        public object Committed { get; set; }

        public bool IsCommitted { get; set; }
    }

    public class CollectionMutationObserver<T>
    {
        private ICollection<T> _mutatedCollection;
        private readonly List<ObjectMutationObserver<T>> _itemsMutationObservers;
        private readonly List<string> _collectionItemObservedProperties;


        public CollectionMutationObserver(ICollection<T> source, bool isObservingItems = false, params string[] properties)
        {
            _itemsMutationObservers = new List<ObjectMutationObserver<T>>();
            _collectionItemObservedProperties = new List<string>();
            _collectionItemObservedProperties.AddRange(properties);
            Source = source.ToList();
            IsObservingItems = isObservingItems;
            if (isObservingItems)
            {
                Source.ToList().ForEach(item =>
                {
                    _itemsMutationObservers.Add(new ObjectMutationObserver<T>(item,properties));
                });
            }
        }

        public ICollection<T> Source { get; private set; }

        public bool IsObservingItems { get; }

        public bool HasCommitedChanges { get; private set; }


        public ICollection<T> GetAddedItems(ICollection<T> mutatedCollection)
        {
            if (!HasCommitedChanges) return null;
            return mutatedCollection.Except(Source).ToList();
        }

        public ICollection<T> GetRemovedItems(ICollection<T> mutatedCollection)
        {
            if (!HasCommitedChanges) return null;
            return Source.Except(mutatedCollection).ToList();
        }

        public bool IsMutated()
        {
            return GetMutationTypes().Any();
        }

        public CollectionMutationType[] GetMutationTypes()
        {
            if (!HasCommitedChanges) return Array.Empty<CollectionMutationType>();

            var hasRemoveItems = GetRemovedItems(_mutatedCollection)?.Any()?? false;
            var hasAddedItems = GetAddedItems(_mutatedCollection)?.Any()??false;

            List< CollectionMutationType > mutations = new List<CollectionMutationType>();
            mutations.Add(CollectionMutationType.ItemsAdded,hasAddedItems);
            mutations.Add(CollectionMutationType.ItemsRemoved,hasRemoveItems);

            if (!IsObservingItems)
            {
                return mutations.ToArray();
            }

            var hasMutatedItems = _itemsMutationObservers.Any(observer => observer.IsMutated());
            mutations.Add(CollectionMutationType.ItemsMutated,hasMutatedItems);
            return mutations.ToArray();
        }

        public void Commit(ICollection<T> mutatedCollection)
        {
            _mutatedCollection = mutatedCollection;
            if (IsObservingItems)
            {
                _itemsMutationObservers.ForEach(observer =>
                {
                    if (_mutatedCollection.Contains(observer.Source))
                    {
                        observer.Commit();
                    }
                });
            }
            HasCommitedChanges = true;
        }

        public void Push()
        {
           
            if (IsObservingItems)
            {

                var removedItems = GetRemovedItems(_mutatedCollection);
                var addedItems = GetAddedItems(_mutatedCollection);

                
                PushAllCollectionItems();

                /*
                 * NOTE: Must be called after pushing all the previous items 
                 * because newly addeditems are have no mutations to push
                 */
                ObserveAddedCollectionItemsAfterCommit(addedItems);
            }

            Source = _mutatedCollection;
            _mutatedCollection = null;
            HasCommitedChanges = false;
        }

        private void UnobserveAllRemovedItemsAfterCommit(ICollection<T> removedItems)
        {
            if (removedItems == null) return;
            _itemsMutationObservers.RemoveAll(o => removedItems.Contains(o.Source));
        }

        private void ObserveAddedCollectionItemsAfterCommit(ICollection<T> addedItems)
        {
            if (addedItems == null) return;
            var addedItemsObservers = addedItems.Select(item => new ObjectMutationObserver<T>(item, _collectionItemObservedProperties.ToArray()));
            _itemsMutationObservers.AddRange(addedItemsObservers);
        }

        private void PushAllCollectionItems()
        {
            _itemsMutationObservers?.ForEach(observer =>
            {
                if (_mutatedCollection.Contains(observer.Source))
                {
                    observer.Push();
                }
            });
        }
    }

    public enum CollectionMutationType
    {
        ItemsAdded,ItemsRemoved,ItemsMutated
    }


    public interface IObjectMutationObserver
    {
        void Commit();
        void Push();
        bool IsMutated();
    }

    public class ObjectMutationObserver<T> : IObjectMutationObserver
    {
        private readonly Dictionary<string, Mutation> _mutations;
        public T Source { get; }

        public ICollection<string> ObservedProperties => _mutations.Keys;
        public ICollection<Mutation> Mutations => _mutations.Values;

        public ObjectMutationObserver(T source)
        {
            Source = source;
            _mutations = new Dictionary<string, Mutation>();
        }

        public ObjectMutationObserver(T source, params string[] properties)
        {
            Source = source;
            _mutations = new Dictionary<string, Mutation>();
            properties.ToList()
                .ForEach(propertyName => { _mutations.Add(propertyName, new Mutation(propertyName, source)); });
        }

        public void Commit()
        {
            foreach (var pair in _mutations)
            {
                var (property, mutation) = pair.ToTuple();
                mutation.Committed = Source.GetPropertyValue(property);
                mutation.IsCommitted = true;
            }
        }

        public void Push()
        {
            foreach (var mutation in _mutations.Values)
            {
                mutation.Initial = mutation.Committed;
                mutation.Committed = null;
                mutation.IsCommitted = false;
            }
        }

        public bool IsMutated()
        {
            return _mutations.Values.Any(mutation =>{

                if (mutation.IsCommitted)
                {
                    if (mutation.Committed == null && mutation.Initial == null) return false;
                    if (mutation.Initial == null && mutation.Committed!= null) return true;
                    if (mutation.Initial != null && mutation.Committed== null) return true;
                   return !mutation.Committed.Equals(mutation.Initial);
                }
                return mutation.IsCommitted;
            
            });
        }
    }

    class DiffGenerator<T>
    {
        public T Generate(ObjectMutationObserver<T> mutationObserver,params (string property,IPropertyDiff propertyDiff)[] generator)
        {
            //mutationObserver.ObservedProperties
            //mutationObserver.Mutations
            generator.ToList().ForEach(p => {
            
                p.property
            });
        }

        
    }

    public class PropertyDiff<TProperty> : IPropertyDiff
    {
        private readonly Func<TProperty, TProperty, TProperty> _propertyMutationHanlder;

        public PropertyDiff(Func<TProperty, TProperty, TProperty> propertyMutationHanlder)
        {
            _propertyMutationHanlder = propertyMutationHanlder;
        }

        public void Invoke(object obj, Mutation mutation)
        {
            var value = _propertyMutationHanlder.Invoke((TProperty)mutation.Initial, (TProperty)mutation.Committed);
            obj.GetType().GetProperty(mutation.PropertyName).SetValue(obj, value);
        }
    }




}