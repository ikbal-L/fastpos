using System;
using Caliburn.Micro;
using FastPosFrontend.ViewModels;

namespace FastPosFrontend.Helpers
{
    public interface ILazyScreen
    {
        void ActivateLoadingScreen();
        void DeactivateLoadingScreen();
    }

    public class AppScreen : Screen, IAppNavigationItem
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

    public class AppNavigationLookupItem:PropertyChangedBase
    {
        private string _title;
        private Type _type;
        private BindableCollection<AppNavigationLookupItem> _subItems;

        
        public AppNavigationLookupItem(string title,Type type = null)
        {
            _title = title;
            _type = type;
        }

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public Type Type
        {
            get => _type;
            set => Set(ref _type, value);
        }

        public BindableCollection<AppNavigationLookupItem> SubItems
        {
            get => _subItems;
            set => Set(ref _subItems, value);
        }

        public static explicit operator AppNavigationLookupItem(NavigationItemConfigurationAttribute configuration) => new AppNavigationLookupItem(configuration.Title,configuration.Type);
    }

    public interface IAppNavigationSubItem<T> : IAppNavigationItem where T : IAppNavigationItem
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


    [AttributeUsage(System.AttributeTargets.Class)]
    public class NavigationItemConfigurationAttribute : Attribute
    {
        public NavigationItemConfigurationAttribute(string title, Type type, NavigationItemLoadingStrategy loadingStrategy = NavigationItemLoadingStrategy.Lazy,Type parentNavigationItem = null,string groupName ="")
        {
            Title = title;
            Type = type;
            LoadingStrategy = loadingStrategy;
            ParentNavigationItem = parentNavigationItem;
            GroupName = groupName;
        }

        public string Title { get; }

        public NavigationItemLoadingStrategy LoadingStrategy { get; }
        public Type ParentNavigationItem { get; }
        public string GroupName { get; }

        public Type Type { get; }
    }

    public enum NavigationItemLoadingStrategy
    {
        OnStartup,
        Lazy
    }
}