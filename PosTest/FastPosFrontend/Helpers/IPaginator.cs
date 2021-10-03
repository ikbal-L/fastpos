using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace FastPosFrontend.Helpers
{
    public interface IPaginator
    {
        int CurrentPage { get; }
        int? PageCount { get;  }
        int PageSize { get;  }
        CollectionViewSource PaginationCollectionViewSource { get; }
        ICollectionView PaginationView { get; }
        public bool CanGoToNextPage();
        public bool CanGoToPreviousPage();
        void NextPage();
        void PreviousPage();
        void Reload();
    }

    public interface IPaginator<T>:IPaginator where T : class
    {

        ObservableCollection<T> ObservableSourceCollection { get; }

    }
}