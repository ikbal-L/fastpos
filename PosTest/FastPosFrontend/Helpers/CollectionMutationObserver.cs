using System;

using System.Collections.Generic;
using System.Linq;
using Utilities.Extensions.Collections;
using Utilities.Extensions.Types;
using Utilities.Attributes;
using System.Runtime.CompilerServices;

namespace FastPosFrontend.Helpers
{
    public class CollectionMutationObserver<T> : ICollectionMutationObserver<T>
    {
        private ICollection<T> _mutatedCollection;
        private readonly List<IMutationObserver<T>> _itemsMutationObservers;
        private readonly List<string> _collectionItemObservedProperties;
        private readonly bool _isSourceCollectionReadonly;
        

        public bool IsInitialized { get; private set; }
        public CollectionMutationObserver(ICollection<T> source, bool isObservingItems = false, params string[] properties) 
        {
            _itemsMutationObservers = new List<IMutationObserver<T>>();
            _isSourceCollectionReadonly = true;
            IsObservingItems = isObservingItems;
            properties = GetDecoratedPropertiesIfEmpty(properties);
            _collectionItemObservedProperties = new List<string>(properties);
            if (source != null)
            {
                Init(source);
            }


        }

        private void Init(ICollection<T> source)
        {
            Source = source.ToList();
            _mutatedCollection = source;
            SetItemsMutationObservers(_collectionItemObservedProperties.ToArray());
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

        private void SetItemsMutationObservers(params string[] properties)
        {
            
            if (IsObservingItems)
            {
                var constructGraph = true;
                var canCreate = ObjectGraphMutationObserver<T>.CanCreateMutationObserverGraph(typeof(T));
                if (! constructGraph)
                {
                    Source.ToList().ForEach(item =>
                    {
                        _itemsMutationObservers.Add(new ObjectMutationObserver<T>(item, properties));
                    });
                }
                else
                {
                    var graphProperties = typeof(T).GetPropertiesDecoratedBy<ObserveMutationsAttribute>().ToArray();
                    Source.ToList().ForEach(item =>
                    {
                        _itemsMutationObservers.Add(new ObjectGraphMutationObserver<T>(item, graphProperties));
                    });
                }
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
            if (!IsInitialized) return null;
            if (!HasCommitedChanges) return null;
            return Source?.Except(mutatedCollection).ToList();
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
            MethodGuard();
            _mutatedCollection = mutatedCollection;
            Commit();
        }

        public void Push()
        {
            MethodGuard();
            if (!HasCommitedChanges) throw new InvalidOperationException("Can't Call push if no changes are commited");
            if (IsObservingItems)
            {

                var removedItems = GetRemovedItems(_mutatedCollection);
                var addedItems = GetAddedItems(_mutatedCollection);

                UnobserveAllRemovedItemsAfterCommit(removedItems);

                PushAllCollectionItems();

                /*
                 * NOTE: Must be called after pushing all the previous items 
                 * because newly addeditems are have no mutations to push
                 */
                ObserveAddedCollectionItemsAfterCommit(addedItems);
            }

            Source = _mutatedCollection;
            if (!_isSourceCollectionReadonly)
            {
                _mutatedCollection = null; 
            }
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

        public void Commit()
        {
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

        private void MethodGuard([CallerMemberName] string method ="")
        {
            

            switch (method)
            {

                case nameof(Commit):
                case nameof(Push):
                case nameof(IsMutated):
                case nameof(GetMutationTypes):
                case nameof(GetAddedItems):
                case nameof(GetRemovedItems):
                    if (!IsInitialized) throw new InvalidOperationException($"Can't Call {method} if {nameof(CollectionMutationObserver<T>)} is not initialized");
                    break;
                default:
                    break;
            }
        }

    }






}