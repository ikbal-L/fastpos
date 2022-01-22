using Caliburn.Micro;
using Caliburn.Micro;
using ServiceInterface.Model;
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
        Func<T, bool> _filterPredicate;
        Func<bool> _canGoNext;
        Func<bool> _canGoPrevious;

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

        public CollectionViewSource PaginationCollectionViewSource { get; private set; }

        public ICollectionView PaginationView => PaginationCollectionViewSource.View;

        public event EventHandler<PageChangedEventArgs> PageChanged;

        public ObservableCollection<T> ObservableSourceCollection { get; private set; }

        public Paginator(PageRetriever<T> pageRetriever, IEnumerable<T> data = null, Func<T, bool> filterPredicate = null, Func<bool> canGoNext = null, Func<bool> canGoPrevious = null)
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
            _canGoNext = canGoNext;
            _canGoPrevious = canGoPrevious;
        }

        private void PaginationCollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            var itemIndex = ObservableSourceCollection.IndexOf(e.Item as T);

            e.Accepted = PageContainsItem(itemIndex);
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
                PageCount = page.TotalPages;
                ObservableSourceCollection.AddRange(page.Elements);
            }
        }

        public void RaisePageChanged()
        {
            PageChanged?.Invoke(this, new PageChangedEventArgs(_currentPage));
        }

        public void Reload()
        {
            CurrentPage = 0;
            ObservableSourceCollection.Clear();
            var page = _pageRetriever.Retrieve(CurrentPage, PageSize);
            PageCount = page.TotalPages;
            ObservableSourceCollection.AddRange(page.Elements);
            PaginationView?.Refresh();
        }

        public bool CanGoToNextPage()
        {
            var predicate = _canGoNext?.Invoke() ?? true;
            if (!PageCount.HasValue && predicate) return true;
            return _currentPage+1 < PageCount;
        }

        public bool CanGoToPreviousPage() => _currentPage > 0;

    }

    public class PageRetriever<T>
    {
        private Func<(int pageIndex, int pageSize), IEnumerable<T>> _delegate;

        private Retriever<T> _retriever;
        private RetrieverAsync<T> _retrieverAsync;

        public bool IsAsync { get; set; } = false;
        public PageRetriever(Retriever<T> retriverDelegate)
        {
            _retriever = retriverDelegate;
        }

        public PageRetriever(RetrieverAsync<T> retriever)
        {
            _retrieverAsync = retriever;
            IsAsync = true;
        }

        public Page<T> Retrieve(int pageIndex, int pageSize)
        {
            return _retriever?.Invoke(pageIndex, pageSize);
        }

        public Task<Page<T>> RetrieveAsync(int pageIndex, int pageSize)
        {
            if (!IsAsync) throw new InvalidOperationException($"Can not Call {nameof(RetrieveAsync)} if initialized by Syncronious Retriever");
            
            return _retrieverAsync?.Invoke(pageIndex, pageSize);
        }

    }

    public delegate Page<T> Retriever<T>(int pageIndex, int pageSize);
    public delegate Task<Page<T>> RetrieverAsync<T>(int pageIndex, int pageSize);

    

    public class PageChangedEventArgs:EventArgs
    {
        public int PageIndex { get; private set; }
        public PageChangedEventArgs(int pageIndex)
        {
            PageIndex = pageIndex;
        }
    }
}
