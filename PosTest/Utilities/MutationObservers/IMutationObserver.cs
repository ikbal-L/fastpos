using System;
using System.Collections.Generic;

namespace Utilities.Mutation.Observers
{
    public interface IMutationObserver
    {
        bool IsInitialized { get; }
        bool HasCommitedChanges { get; }
        void Commit();
        void Push();
        bool IsMutated();
        //TSource GetSource<TSource>();
        TSource GetDifference<TSource>(Func<IMutationObserver,TSource> generator)where TSource:class;
    }
    public interface IMutationObserver<T> : IMutationObserver
    {
        T Source { get; }
    }

    public interface IObjectMutationObserver<T> : IMutationObserver<T>
    {
        
        
    }

    public interface ICollectionMutationObserver : IMutationObserver
    {
        bool IsObservingItems { get; }
        CollectionMutationType[] GetMutationTypes();
    }

    public interface ICollectionMutationObserver<T> : ICollectionMutationObserver, IMutationObserver<ICollection<T>>
    {

        void Commit(ICollection<T> mutatedCollection);
        ICollection<T> GetAddedItems(ICollection<T> mutatedCollection);

        ICollection<T> GetRemovedItems(ICollection<T> mutatedCollection);

    }

    public enum CollectionMutationType
    {
        ItemsAdded, ItemsRemoved, ItemsMutated
    }

}