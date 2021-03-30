using Caliburn.Micro;

namespace FastPosFrontend.ViewModels
{
    public class LoadingViewModel:Screen
    {
        private string _loadingMessage;

        public LoadingViewModel(string loadingMessage)
        {
            _loadingMessage = loadingMessage;
        }

        public string LoadingMessage
        {
            get => _loadingMessage;
            set => Set(ref _loadingMessage, value);
        }
    }
}