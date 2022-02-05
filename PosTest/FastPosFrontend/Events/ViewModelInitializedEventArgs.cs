using System;
using System.Collections.Generic;
using System.Linq;

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

    public class SettingsUpdatedEventArgs : EventArgs
    {
        
        public SettingsUpdatedEventArgs(params object[] args)
        {
            Settings = args.ToList();
        }

        public List<object> Settings { get; }
    }

    public interface ISettingsController
    {
        public event EventHandler<SettingsUpdatedEventArgs> SettingsUpdated;


    }

    public interface ISettingsListener
    {
        public Type[] SettingsControllers{ get;}
        public void OnSettingsUpdated(object sender, SettingsUpdatedEventArgs e);

    }
}