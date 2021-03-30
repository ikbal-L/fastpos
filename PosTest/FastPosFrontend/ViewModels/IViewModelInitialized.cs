using System;
using FastPosFrontend.Events;

namespace FastPosFrontend.ViewModels
{
    public interface INotifyViewModelInitialized
    {
        event EventHandler<ViewModelInitializedEventArgs> ViewModelInitialized;
        void Initialize();
    }
}