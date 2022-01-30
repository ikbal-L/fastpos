using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FastPosFrontend.Helpers
{
    public class Paginator<T> : PaginatorBase<T> where T : class
    {

        private readonly PageRetriever<T> _pageRetriever;
        Func<T, bool> _filterPredicate;
        Func<bool> _canGoNext;
        Func<bool> _canGoPrevious;
        Action<T> _beforePaginate;

        public Paginator(PageRetriever<T> pageRetriever , IEnumerable<T> data = null, Func<T, bool> filterPredicate = null, Func<bool> canGoNext = null, Func<bool> canGoPrevious = null,Action<T> beforePaginate = null)
        {
            _pageRetriever = pageRetriever;
            _filterPredicate = filterPredicate;
            _beforePaginate = beforePaginate;
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

        

        public override void NextPage()
        {
            if (CanGoToNextPage())
            {
                CurrentPage++;
                if (_beforePaginate!= null)
                {
                    foreach (var item in ObservableSourceCollection)
                    {
                        _beforePaginate(item);
                    }
                }
                BeforePaginatingToNextPage();
                PaginationCollectionViewSource?.View?.Refresh();
            }
        }

        public override void PreviousPage()
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
                RetrievePage();
            }
        }

        private void RetrievePage()
        {
            var page = _pageRetriever.Retrieve(CurrentPage, PageSize);
            PageCount = page.TotalPages;
            ObservableSourceCollection.AddRange(page.Elements);
        }

        

        public override void Reload()
        {
            CurrentPage = 0;
            ObservableSourceCollection.Clear();
            var page = _pageRetriever.Retrieve(CurrentPage, PageSize);
            PageCount = page.TotalPages;
            ObservableSourceCollection.AddRange(page.Elements);
            PaginationView?.Refresh();
        }

        public void GoToPage(int pageIndex)
        {
            CurrentPage = pageIndex;
            ObservableSourceCollection.Clear();
            var page = _pageRetriever.Retrieve(CurrentPage, PageSize);
            PageCount = page.TotalPages;
            ObservableSourceCollection.AddRange(page.Elements);
            PaginationView?.Refresh();
        }

       

        public override bool CanGoToNextPage()
        {
            var predicate = _canGoNext?.Invoke() ?? true;
            if (!PageCount.HasValue && predicate) return true;
            return _currentPage+1 < PageCount;
        }

        public override bool CanGoToPreviousPage() => _currentPage > 0;

       

        

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
