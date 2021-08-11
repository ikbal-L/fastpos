using System.Collections.Generic;

namespace FastPosFrontend.Helpers
{
    public interface ICollectionMutationObserver<T>: IMutationObserver
    {
        bool HasCommitedChanges { get; }
        bool IsObservingItems { get; }
        ICollection<T> Source { get; }

        void Commit(ICollection<T> mutatedCollection);
        ICollection<T> GetAddedItems(ICollection<T> mutatedCollection);
        CollectionMutationType[] GetMutationTypes();
        ICollection<T> GetRemovedItems(ICollection<T> mutatedCollection);
        
    }
}