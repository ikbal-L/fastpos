using System;
using System.Reflection;
using Caliburn.Micro;
using FastPosFrontend.Events;
using FastPosFrontend.ViewModels;
using FastPosFrontend.Navigation;

namespace FastPosFrontend.Helpers
{
    public interface ILazyScreen
    {
        public bool IsReady { get; }
        void ActivateLoadingScreen();
        void DeactivateLoadingScreen();
    }

    public class EmbeddedCommandBarCommand : GenericCommand
    {
        public EmbeddedCommandBarCommand(object content, Action<object> executeMethod,
            Func<object, bool> canExecuteMethod = null) : base(content, executeMethod, canExecuteMethod)
        {
        }
    }

    public interface IAppNavigationTarget
    {
        bool CanNavigate(Type navigationTargetType = null);
    }


    public abstract class LazyScreen : AppScreen, ILazyScreen, INotifyViewModelInitialized
    {
        private readonly LoadingScreenViewModel _loadingScreen;

        protected NotifyAllTasksCompletion _data;
        private bool _isReady;

        protected LazyScreen(LoadingScreenType loadingScreenType = LoadingScreenType.Spinner)
        {
            var title = GetType().GetCustomAttribute<NavigationItemAttribute>()?.Title;
            _loadingScreen = new LoadingScreenViewModel($"Loading {title}")
                {LoadingScreenType = loadingScreenType};
            //ActivateLoadingScreen();
            Setup();
            OnReady();
        }

        public bool IsReady
        {
            get => _isReady;
            protected set => Set(ref _isReady, value);
        }

        protected LazyScreen(string loadingMessage, LoadingScreenType loadingScreenType = LoadingScreenType.Spinner)
        {
            _loadingScreen = new LoadingScreenViewModel(loadingMessage) {LoadingScreenType = loadingScreenType};
            //ActivateLoadingScreen();
        }

        public void ActivateLoadingScreen()
        {
            if (IsReady)
            {
                DeactivateLoadingScreen();
                return;
            }

            var conductor = Parent as Conductor<object>;
            conductor?.ActivateItem(_loadingScreen);
        }

        public void DeactivateLoadingScreen()
        {
            var conductor = Parent as Conductor<object>;
            conductor?.ActivateItem(this);
            conductor?.DeactivateItem(_loadingScreen, true);
        }

        public event EventHandler<ViewModelInitializedEventArgs> ViewModelInitialized;


        /// <summary>
        /// The <c>Setup</c> Method sets up tasks to retrieve data and Notifications on task completion 
        /// </summary>
        protected abstract void Setup();
        public abstract void Initialize();

        protected virtual void OnReady()
        {
            if (_data.IsCompleted)
            {
                Initialize();
                IsReady = true;
                return;
            }

            _data.AllTasksCompleted += OnAllTasksCompleted;
        }

        protected virtual void OnAllTasksCompleted(object sender, AllTasksCompletedEventArgs e)
        {
            if (!e.IsTaskCollectionCompleted) return;
            Initialize();
            /*
             * Enable this line of code if you want to publish the event to subscribers
             */
            //ViewModelInitialized?.Invoke(this, new ViewModelInitializedEventArgs(true));
            IsReady = true;
            DeactivateLoadingScreen();
        }
    }
}