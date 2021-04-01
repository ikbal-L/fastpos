using System;

namespace FastPosFrontend.Helpers
{
    [AttributeUsage(System.AttributeTargets.Class)]
    public class NavigationItemConfigurationAttribute : Attribute
    {
        public NavigationItemConfigurationAttribute(string title, Type target, NavigationItemLoadingStrategy loadingStrategy = NavigationItemLoadingStrategy.Lazy,Type parentNavigationItem = null,string groupName ="")
        {
            Title = title;
            Target = target;    
            LoadingStrategy = loadingStrategy;
            ParentNavigationItem = parentNavigationItem;
            GroupName = groupName;
        }

        public string Title { get; }

        public NavigationItemLoadingStrategy LoadingStrategy { get; }
        public Type ParentNavigationItem { get; }
        public string GroupName { get; }

        public Type Target { get; }
    }

    public enum NavigationItemLoadingStrategy
    {
        OnStartup,
        Lazy
    }
}