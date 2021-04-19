﻿using System;
using System.Reflection;
using Caliburn.Micro;
using FastPosFrontend.Events;
using FastPosFrontend.ViewModels;

namespace FastPosFrontend.Helpers
{
    public interface ILazyScreen
    {
        public bool IsReady { get;}
        void ActivateLoadingScreen();
        void DeactivateLoadingScreen();
    }

    public class AppScreen : Screen, IAppNavigationTarget
    {
        private string _title;
        private EmbeddedCommandBarViewModel _embeddedCommandBar;
        private EmbeddedContentBarViewModel _embeddedContentBar;

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public EmbeddedCommandBarViewModel EmbeddedCommandBar
        {
            get => _embeddedCommandBar;
            set => Set(ref _embeddedCommandBar, value);
        }

        public EmbeddedContentBarViewModel EmbeddedContentBar
        {
            get => _embeddedContentBar;
            set => Set(ref _embeddedContentBar, value);
        }

        public virtual bool CanNavigate()
        {
            return true;
        }
    }

    public class EmbeddedCommandBarCommand
    {
        public EmbeddedCommandBarCommand(object content, Action<object> executeMethod, Func<object, bool> canExecuteMethod = null)
        {
            Content = content;
            Command = canExecuteMethod== null ? 
                new DelegateCommandBase(executeMethod) : 
                new DelegateCommandBase(executeMethod, canExecuteMethod);
        }
        public object Content { get; set; }

        public DelegateCommandBase Command { get; set; }
    }

    public interface IAppNavigationTarget
    {
        bool CanNavigate();
    }

    public class AppNavigationLookupItem : PropertyChangedBase
    {
        private string _title;
        private Type _target;
        private BindableCollection<AppNavigationLookupItem> _subItems;


        public AppNavigationLookupItem(string title, Type target = null,bool keepAlive = false,bool isDefault = false)
        {
            _title = title;
            _target = target;
            KeepAlive = keepAlive;
            IsDefault = isDefault;
        }

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public Type Target
        {
            get => _target;
            set => Set(ref _target, value);
        }

        public bool KeepAlive { get; set; }
        public bool IsDefault { get; set; } 

        public BindableCollection<AppNavigationLookupItem> SubItems
        {
            get => _subItems;
            set => Set(ref _subItems, value);
        }

        public static explicit operator AppNavigationLookupItem(NavigationItemConfigurationAttribute configuration) =>
            new AppNavigationLookupItem(configuration.Title, configuration.Target,configuration.KeepAlive,isDefault:configuration.IsDefault);
    }

  

    public abstract class LazyScreen : AppScreen, ILazyScreen, INotifyViewModelInitialized
    {
        private readonly LoadingScreenViewModel _loadingScreen;

        protected NotifyAllTasksCompletion _data;
        private bool _isReady;

        protected LazyScreen(LoadingScreenType loadingScreenType = LoadingScreenType.Spinner)
        {
            var title = this.GetType().GetCustomAttribute<NavigationItemConfigurationAttribute>()?.Title;
            _loadingScreen = new LoadingScreenViewModel($"Loading {title}")
                {LoadingScreenType = loadingScreenType};
            //ActivateLoadingScreen();
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

        protected abstract void Setup();
        public abstract void Initialize();

        protected virtual void OnReady()
        {
            if (_data.IsCompleted)
            {
                Initialize();
                IsReady = true;

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