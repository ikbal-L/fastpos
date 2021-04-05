using System;

namespace FastPosFrontend.Helpers
{
    [AttributeUsage(System.AttributeTargets.Class)]
    public class NavigationItemConfigurationAttribute : Attribute
    {
        public NavigationItemConfigurationAttribute(string title, Type target, NavigationItemLoadingStrategy loadingStrategy = NavigationItemLoadingStrategy.Lazy,Type parentNavigationItem = null,string groupName ="",bool keepAlive = false)
        {
            Title = title;
            Target = target;    
            LoadingStrategy = loadingStrategy;
            ParentNavigationItem = parentNavigationItem;
            GroupName = groupName;
            KeepAlive = keepAlive;
        }

        public string Title { get; }

        public NavigationItemLoadingStrategy LoadingStrategy { get; }
        public Type ParentNavigationItem { get; }
        public string GroupName { get; }

        public Type Target { get; }

        public bool KeepAlive { get;}
    }

    public enum NavigationItemLoadingStrategy
    {
        OnStartup,
        Lazy
    }
}