using Caliburn.Micro;
using FastPosFrontend.ViewModels;

namespace FastPosFrontend.Helpers
{
    public interface ILazyScreen
    {
        void ActivateLoadingScreen();
        void DeactivateLoadingScreen();
    }

    public class AppScreen : Screen,IAppNavigationItem
    {
        private string _title;

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }
    }

    public interface IAppNavigationItem
    {
    }

    public interface IAppNavigationSubItem<T>:IAppNavigationItem where T : IAppNavigationItem
    {
    }

    public abstract class LazyScreen : AppScreen, ILazyScreen
    {
        private readonly LoadingScreenViewModel _loadingScreen;

        protected LazyScreen(LoadingScreenType loadingScreenType = LoadingScreenType.Spinner)
        {
            _loadingScreen = new LoadingScreenViewModel($"Loading {GetType().Name}")
                {LoadingScreenType = loadingScreenType};
        }

        protected LazyScreen(string loadingMessage, LoadingScreenType loadingScreenType = LoadingScreenType.Spinner)
        {
            _loadingScreen = new LoadingScreenViewModel(loadingMessage) {LoadingScreenType = loadingScreenType};
        }

        public void ActivateLoadingScreen()
        {
            var conductor = Parent as Conductor<object>;
            conductor?.ActivateItem(_loadingScreen);
        }

        public void DeactivateLoadingScreen()
        {
            var conductor = Parent as Conductor<object>;
            conductor?.ActivateItem(this);
            conductor?.DeactivateItem(_loadingScreen, true);
        }
    }
}