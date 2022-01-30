using Caliburn.Micro;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace FastPosFrontend.Helpers
{
    public abstract class PaginatorBase<T>:PropertyChangedBase, IPaginator<T> where T : class
    {
        protected int _currentPage = 0;



        public int? PageCount { get; set; }
        public int PageSize { get; set; } = 10;

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                Set(ref _currentPage, value);
                RaisePageChanged();
            }
        }

        public CollectionViewSource PaginationCollectionViewSource { get; protected set; }

        public ICollectionView PaginationView => PaginationCollectionViewSource.View;

        public event EventHandler<PageChangedEventArgs> PageChanged;

        public ObservableCollection<T> ObservableSourceCollection { get; protected set; }

        public void RaisePageChanged()
        {
            PageChanged?.Invoke(this, new PageChangedEventArgs(_currentPage));
        }

        public abstract bool CanGoToNextPage();


        public abstract bool CanGoToPreviousPage();


        public abstract void NextPage();

        public abstract void PreviousPage();

        public abstract void Reload();

        protected bool PageContainsItem(int itemIndex) => itemIndex >= PageSize * CurrentPage && itemIndex < PageSize * (CurrentPage + 1);

    }
}
