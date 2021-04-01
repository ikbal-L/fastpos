using System;

namespace FastPosFrontend.Helpers
{
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