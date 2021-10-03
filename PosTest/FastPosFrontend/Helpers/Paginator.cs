using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FastPosFrontend.Helpers
{
    public class Paginator<T> : PropertyChangedBase, IPaginator<T> where T : class
    {
        private int _currentPage = 0;
        private readonly PageRetriever<T> _pageRetriever;
        Predicate<T> _filterPredicate;
        public int? PageCount { get; set; }
        public int PageSize { get; set; } = 10;

        public int CurrentPage { get => _currentPage; set => Set(ref _currentPage, value); }

        public ObservableCollection<T> ObservableSourceCollection { get; private set; }

        public CollectionViewSource PaginationCollectionViewSource { get; private set; }

        public ICollectionView PaginationView => PaginationCollectionViewSource.View;

        public Paginator(PageRetriever<T> pageRetriever, IEnumerable<T> data = null, Predicate<T> filterPredicate = null)
        {
            _pageRetriever = pageRetriever;
            _filterPredicate = filterPredicate;

            ObservableSourceCollection = ObservableCollectionEx.FromNullable(data);

            PaginationCollectionViewSource = new CollectionViewSource() { Source = ObservableSourceCollection };

            if (_filterPredicate != null)
            {
                PaginationCollectionViewSource.Filter += PaginationCollectionViewSource_FilterWithPredicate;
            }
            else
            {
                PaginationCollectionViewSource.Filter += PaginationCollectionViewSource_Filter;
            }
        }

        private void PaginationCollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            var itemIndex = ObservableSourceCollection.IndexOf(e.Item as T);
            if (PageContainsItem(itemIndex))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        private void PaginationCollectionViewSource_FilterWithPredicate(object sender, FilterEventArgs e)
        {
            var itemIndex = ObservableSourceCollection.IndexOf(e.Item as T);
            if (PageContainsItem(itemIndex) && _filterPredicate.Invoke(e.Item as T))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        private bool PageContainsItem(int itemIndex) => itemIndex >= PageSize * CurrentPage && itemIndex < PageSize * (CurrentPage + 1);

        public void NextPage()
        {
            if (CanGoToNextPage())
            {
                CurrentPage++;
                BeforePaginatingToNextPage();
                PaginationCollectionViewSource?.View?.Refresh(); 
            }
        }

        public void PreviousPage()
        {
            if (CanGoToPreviousPage())
            {
                CurrentPage--;
                PaginationCollectionViewSource?.View?.Refresh(); 
            }
        }

        private void BeforePaginatingToNextPage()
        {
            if (((CurrentPage + 1) * PageSize) >= ObservableSourceCollection.Count)
            {
                var page = _pageRetriever.Retrieve(CurrentPage, PageSize);
                ObservableSourceCollection.AddRange(page);
            }
        }

        public void Reload()
        {
            CurrentPage = 0;
            ObservableSourceCollection.Clear();
            var page = _pageRetriever.Retrieve(CurrentPage, PageSize);
            ObservableSourceCollection.AddRange(page);
            PaginationView?.Refresh();
        }

        public bool CanGoToNextPage()
        {
            if (!PageCount.HasValue) return true;
            return CurrentPage < PageCount;
        }

        public bool CanGoToPreviousPage() => CurrentPage > 0;

    }

    public class PageRetriever<T>
    {
        private Func<(int pageIndex, int pageSize), IEnumerable<T>> _action;
        public PageRetriever(Func<(int pageIndex, int pageSize), IEnumerable<T>> action)
        {
            _action = action;
        }

        public IEnumerable<T> Retrieve(int pageIndex, int pageSize)
        {
            return _action?.Invoke(( pageIndex,  pageSize));
        }
    }
}
