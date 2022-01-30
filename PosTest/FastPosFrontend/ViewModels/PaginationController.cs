using FastPosFrontend.Helpers;
using Caliburn.Micro;
using System;
using System.Windows.Input;

namespace FastPosFrontend.ViewModels
{
    public class PaginationController : PropertyChangedBase
    {
        private readonly IPaginator _paginator;
        private int _currentPage;

        public PaginationController(IPaginator paginator)
        {
            _paginator = paginator ?? throw new ArgumentNullException(nameof(paginator));
            CurrentPage = _paginator.CurrentPage;
            NextPageCommand = new DelegateCommandBase(NextPage, CanGoToNextPage);
            PreviousPageCommand = new DelegateCommandBase(PreviousPage, CanGoToPreviousPage);
            _paginator.PageChanged += OnPageChanged;
            _paginator.PropertyChanged += _paginator_PropertyChanged;
        }

        private void _paginator_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IPaginator.PageCount))
            {
                (NextPageCommand as DelegateCommandBase)?.RaiseCanExecuteChanged();
                (PreviousPageCommand as DelegateCommandBase)?.RaiseCanExecuteChanged();
            }
        }

        private void OnPageChanged(object sender, PageChangedEventArgs e)
        {
            CurrentPage = e.PageIndex;
        }

        public ICommand NextPageCommand { get; private set; }
        public ICommand PreviousPageCommand { get; private set; }
        public ICommand FirstPageCommand { get; private set; }
        public ICommand LastPageCommand { get; private set; }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                Set( ref _currentPage , ++value);
                (NextPageCommand as DelegateCommandBase)?.RaiseCanExecuteChanged();
                (PreviousPageCommand as DelegateCommandBase)?.RaiseCanExecuteChanged();
            }
        }

        protected virtual void NextPage(object obj)
        {
            _paginator?.NextPage();
        }

        protected virtual void PreviousPage(object obj)
        {
            _paginator?.PreviousPage();
        }

        private bool CanGoToNextPage(object obj) => _paginator.CanGoToNextPage();
        private bool CanGoToPreviousPage(object obj) => _paginator.CanGoToPreviousPage();

    }
}
