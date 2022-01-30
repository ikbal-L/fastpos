using System.Collections.ObjectModel;
using System.Windows.Data;

namespace FastPosFrontend.Helpers
{
    public class CollectionPaginator<T> : PaginatorBase<T> where T : class
    {

        public CollectionPaginator(ObservableCollection<T> source)
        {
            ObservableSourceCollection = source;
            PageCount = CalculatePageCount(ObservableSourceCollection.Count);
            source.CollectionChanged += Source_CollectionChanged;
            PaginationCollectionViewSource = new (){ Source = ObservableSourceCollection };
            PaginationCollectionViewSource.Filter += PaginationCollectionViewSource_Filter;
        }

        private void Source_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var pageCount = CalculatePageCount(ObservableSourceCollection.Count);
            if (PageCount!= pageCount)
            {
                PageCount = pageCount;
                NotifyOfPropertyChange(nameof(PageCount));
                PaginationCollectionViewSource.View.Refresh();
            }
        }

        private int CalculatePageCount(int itemcount)
        {
            if (itemcount % PageSize == 0)
            {
                return (itemcount / PageSize);
            }
            else
            {
                return (itemcount / PageSize) + 1;
            }
        }

        private void PaginationCollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            var itemIndex = (ObservableSourceCollection.Count-1)- ObservableSourceCollection.IndexOf(e.Item as T);
            e.Accepted = PageContainsItem(itemIndex);
        }

        public override bool CanGoToNextPage()
        {
            return _currentPage + 1 < PageCount;
        }

        public override bool CanGoToPreviousPage() => _currentPage > 0;

        public override void NextPage()
        {
            CurrentPage++;
            PaginationCollectionViewSource?.View?.Refresh();
        }

        public override void PreviousPage()
        {
            CurrentPage--;
            PaginationCollectionViewSource?.View?.Refresh();
        }

        public override void Reload()
        {
            CurrentPage = 0;
            PaginationCollectionViewSource?.View?.Refresh();
        }
    }
}
