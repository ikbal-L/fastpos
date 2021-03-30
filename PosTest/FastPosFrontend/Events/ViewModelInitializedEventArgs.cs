using System;

namespace FastPosFrontend.Events
{
    public class ViewModelInitializedEventArgs: EventArgs
    {
        public bool IsInitialized { get; set; }

        public ViewModelInitializedEventArgs(bool isInitialized)
        {
            IsInitialized = isInitialized;
        }
    }
}