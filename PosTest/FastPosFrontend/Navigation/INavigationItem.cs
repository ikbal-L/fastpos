using System;

namespace FastPosFrontend.Navigation
{
    public interface INavigationItem
    {
        string GroupName { get; }
        bool IsDefault { get; }
        bool KeepAlive { get; }
        NavigationItemLoadingStrategy LoadingStrategy { get; }
        Type ParentNavigationItem { get; }
        Type Target { get; }
        string Title { get; }
    }

    public enum NavigationItemLoadingStrategy
    {
        OnStartup,
        Lazy
    }
}