using Caliburn.Micro;
using FastPosFrontend.ViewModels;

namespace FastPosFrontend.Helpers
{
    public interface ILazyScreen
    {
        void ActivateLoadingScreen();
        void DeactivateLoadingScreen();
    }

    public class LazyScreen:Screen, ILazyScreen
    {
        private readonly LoadingScreenViewModel _loadingScreen;

        public LazyScreen(LoadingScreenType loadingScreenType = LoadingScreenType.Spinner)
        {
            _loadingScreen = new LoadingScreenViewModel($"Loading {GetType().Name}"){LoadingScreenType = loadingScreenType};
        }

        public LazyScreen(string loadingMessage,LoadingScreenType loadingScreenType = LoadingScreenType.Spinner)
        {
            _loadingScreen = new LoadingScreenViewModel(loadingMessage) { LoadingScreenType = loadingScreenType };
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
            conductor?.DeactivateItem(_loadingScreen,true);
        }
    }
}