using System;
using System.Linq;
using FastPosFrontend.Events;
using FastPosFrontend.Helpers;
using ServiceInterface.Model;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels.Settings
{
  
    [NavigationItemConfiguration(title:"General Settings",typeof(GeneralSettingsViewModel),groupName:"Settings")]
    public  class GeneralSettingsViewModel : AppScreen,ISettingsController
    {
        private GeneralSettings _generalSettings;

        public GeneralSettings Settings
        {
            get => _generalSettings;
            set { _generalSettings = value;

                NotifyOfPropertyChange(nameof(Settings));
            }
        }

        public GeneralSettingsViewModel() {
            //this.Index = 1;
            this.Title = "General";
            var setting = AppConfigurationManager.Configuration<GeneralSettings>();
            Settings = setting ?? new GeneralSettings();
            if (!StateManager.Get<Table>().Any())
            {
                Settings.TableNumber = 0;
            }
            Settings.PropertyChanged += Settings_PropertyChanged;
            Settings.Initialized = true;
            
            
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            AppConfigurationManager.Save(Settings);
        }

        public event EventHandler<SettingsUpdatedEventArgs> SettingsUpdated;
        public override void BeforeNavigateAway()
        {
            SettingsUpdated?.Invoke(this,new SettingsUpdatedEventArgs());
        }
    }
}
