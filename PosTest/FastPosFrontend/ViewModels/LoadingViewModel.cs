using Caliburn.Micro;

namespace FastPosFrontend.ViewModels
{
    public class LoadingScreenViewModel:Screen
    {
        private string _loadingMessage;
        private LoadingScreenType _loadingScreenType;

        public LoadingScreenViewModel(string loadingMessage)
        {
            _loadingMessage = loadingMessage;
        }

       

        public string LoadingMessage
        {
            get => _loadingMessage;
            set => Set(ref _loadingMessage, value);
        }

        public LoadingScreenType LoadingScreenType
        {
            get => _loadingScreenType;
            set => Set(ref _loadingScreenType, value);
        }
    }

    public enum LoadingScreenType
    {
        Spinner,
        ProgressBar
    }
}