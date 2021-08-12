using System;

using System.Collections.Generic;
using System.Linq;
using Utilities.Extensions.Collections;
using Utilities.Extensions.Types;
using Utilities.Attributes;
using Utilities.Mutation.Observers.Extentions;
using System.Reflection;

namespace Utilities.Mutation.Observers
{
    public class CollectionMutationObserver<T> : ICollectionMutationObserver<T>
    {
        private ICollection<T> _mutatedCollection;
        private readonly List<IMutationObserver<T>> _itemsMutationObservers;
        private readonly string [] _objectObservedProperties;
        private readonly PropertyInfo [] _objectGraphObservedProperties;
        private readonly bool _isSourceCollectionReadonly;
        private readonly bool _isGraphConstructionEnabled;


        public bool IsInitialized { get; private set; }
        public CollectionMutationObserver(ICollection<T> source, bool isObservingItems = false, bool isGraphConstructionEnabled = true, params string[] properties)
        {
            _itemsMutationObservers = new List<IMutationObserver<T>>();
            _isSourceCollectionReadonly = true;
            IsObservingItems = isObservingItems;
            _isGraphConstructionEnabled = isGraphConstructionEnabled;
            properties = GetDecoratedPropertiesIfEmpty(properties);

            if (_isGraphConstructionEnabled && isObservingItems)
            {
                _objectGraphObservedProperties = typeof(T).GetPropertiesDecoratedBy<ObserveMutationsAttribute>().ToArray();
            }

            _objectObservedProperties = properties;
            if (source != null)
            {
                Init(source);
            }
        }

        private void Init(ICollection<T> source)
        {
            Source = source.ToList();
            _mutatedCollection = source;
            SetItemsMutationObservers();
            IsInitialized = true;
        }

        private static string[] GetDecoratedPropertiesIfEmpty(string[] properties)
        {
            if (properties.Length == 0)
            {
                properties = typeof(T).GetPropertyNamesDecoratedBy<ObservePropertyMutationAttribute>().ToArray();

            }

            return properties;
        }

        private void SetItemsMutationObservers()
        {
            
            if (IsObservingItems)
            {

                var canCreate = ObjectGraphMutationObserver<T>.CanCreateMutationObserverGraph(typeof(T));
                if (!(_isGraphConstructionEnabled && canCreate))
                {
                    Source.ToList().ForEach(AddObjectMutationObserver);
                }
                else
                {
                    Source.ToList().ForEach(AddObjectGraphMutationObserver);
                }
            }
        }

        private void AddObjectGraphMutationObserver(T item)
        {
            _itemsMutationObservers.Add(new ObjectGraphMutationObserver<T>(item, _objectGraphObservedProperties));
        }

        private void AddObjectMutationObserver(T item)
        {
            _itemsMutationObservers.Add(new ObjectMutationObserver<T>(item, _objectObservedProperties));
        }


        public ICollection<T> Source { get; private set; }

        public bool IsObservingItems { get; }

        public bool HasCommitedChanges { get; private set; }

        public IMutationObserver this[T obj]
        {
            get=> _itemsMutationObservers.FirstOrDefault(i => i.Source.Equals(obj));

        }


        public ICollection<T> GetAddedItems(ICollection<T> mutatedCollection = null)
        {
            if (!HasCommitedChanges) return null;
            return mutatedCollection.Except(Source).ToList();
        }

        public ICollection<T> GetRemovedItems(ICollection<T> mutatedCollection = null)
        {
            if (!IsInitialized) return null;
            if (!HasCommitedChanges) return null;
            return Source?.Except(mutatedCollection).ToList();
        }

        public ICollection<T> GetMutatedItems(Func<IMutationObserver<T>, T> transform = null,ICollection<T> mutatedCollection = null)
        {
            if (!IsInitialized) return null;
            if (!HasCommitedChanges) return null;
            return _itemsMutationObservers.Where(o=> o.IsMutated()).Select(o=> {

                if (transform == null) return o.Source;
                return transform.Invoke(o);
            }).ToList();
        }

        public bool IsMutated()
        {
            if (!IsInitialized) return false;
            return GetMutationTypes().Any();
        }

        public CollectionMutationType[] GetMutationTypes()
        {
            if (!IsInitialized) return null;
            if (!HasCommitedChanges) return Array.Empty<CollectionMutationType>();

            var hasRemoveItems = GetRemovedItems(_mutatedCollection)?.Any() ?? false;
            var hasAddedItems = GetAddedItems(_mutatedCollection)?.Any() ?? false;

            List<CollectionMutationType> mutations = new List<CollectionMutationType>();
            mutations.Add(CollectionMutationType.ItemsAdded, hasAddedItems);
            mutations.Add(CollectionMutationType.ItemsRemoved, hasRemoveItems);

            if (!IsObservingItems)
            {
                return mutations.ToArray();
            }

            var hasMutatedItems = _itemsMutationObservers.Any(observer => observer.IsMutated());
            mutations.Add(CollectionMutationType.ItemsMutated, hasMutatedItems);
            return mutations.ToArray();
        }

        public void Commit(ICollection<T> mutatedCollection)
        {
            this.MethodGuard();
            _mutatedCollection = mutatedCollection;
            Commit();
        }

        public void Push()
        {
            this.MethodGuard();
            if (!HasCommitedChanges) throw new InvalidOperationException("Can't Call push if no changes are commited");
            if (IsObservingItems)
            {

                UnobserveAllRemovedItemsAfterCommit();

                PushAllCollectionItems();


                ObserveAddedCollectionItemsAfterCommit();
            }

            Source = _mutatedCollection;
            if (!_isSourceCollectionReadonly)
            {
                _mutatedCollection = null;
            }
            HasCommitedChanges = false;
        }

        public bool CommitAndPushAddedItems()
        {
            HasCommitedChanges = true;
            ObserveAddedCollectionItemsAfterCommit();
            HasCommitedChanges = false;
            return true;
        }

        public void ObserveItem(T item)
        {
            if (!IsObservingItems)
            {
                throw new InvalidOperationException("Tyring to observe an item of a collection that is not observing individual item mutations.");
            }
            var canCreate = ObjectGraphMutationObserver<T>.CanCreateMutationObserverGraph(typeof(T));
            if (_isGraphConstructionEnabled&& canCreate)
            {
                AddObjectGraphMutationObserver(item);
            }
            else
            {
                AddObjectMutationObserver(item);
            }
        }

        private void UnobserveAllRemovedItemsAfterCommit()
        {
            var removedItems = GetRemovedItems(_mutatedCollection);
            if (removedItems == null) return;
            _itemsMutationObservers.RemoveAll(o => removedItems.Contains(o.Source));
        }


        /*
                 * NOTE: Must be called after pushing all the previous items 
                 * because newly addeditems are have no mutations to push
                 */
        private void ObserveAddedCollectionItemsAfterCommit()
        {
            var addedItems = GetAddedItems(_mutatedCollection);
            if (addedItems == null) return;
            var addedItemsObservers = addedItems.Select(item => new ObjectMutationObserver<T>(item, _objectObservedProperties.ToArray()));
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

        public void Commit()
        {
            this.MethodGuard();
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

        public void LateInit(ICollection<T> source)
        {
            if (IsInitialized) throw new InvalidOperationException($"{this} was already initialized");
            Init(source);
        }

    }
}