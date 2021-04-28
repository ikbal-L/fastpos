using FastPosFrontend.Helpers;
using ServiceLib.Service;

namespace FastPosFrontend.ViewModels.Settings
{
  
    [NavigationItemConfiguration(title:"General Settings",typeof(GeneralSettingsViewModel),groupName:"Settings")]
    public  class GeneralSettingsViewModel : AppScreen
    {
      
        private SettingsManager<GeneralSettings> Manager;

        private GeneralSettings generalSettings;

        public GeneralSettings Settings
        {
            get { return generalSettings; }
            set { generalSettings = value;

                NotifyOfPropertyChange(nameof(Settings));
            }
        }

        public GeneralSettingsViewModel() {
            //this.Index = 1;
            this.Title = "General";
            var setting = AppConfigurationManager.Configuration<GeneralSettings>();
            Settings = setting ?? new GeneralSettings();
            Settings.PropertyChanged += Settings_PropertyChanged;
            Settings.Initialized = true;
            //this.Content = new GeneralSettingsView() { DataContext = this };
            
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            AppConfigurationManager.Save(Settings);
        }
    }
}
