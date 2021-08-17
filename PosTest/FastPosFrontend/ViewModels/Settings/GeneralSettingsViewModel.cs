using System;
using System.Linq;
using FastPosFrontend.Events;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using ServiceInterface.Model;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels.Settings
{
  
    [NavigationItem(title:"General Settings",typeof(GeneralSettingsViewModel),groupName:"Settings")]
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
            Title = "General";
            var setting = AppConfigurationManager.Configuration<GeneralSettings>();
            Settings = setting ?? new GeneralSettings();
            Settings.PropertyChanged += Settings_PropertyChanged;
           
            var tables = StateManager.Get<Table>();
            if (!tables.Any())
            {
                Settings.TableNumber = 0;
            }
            else
            {
                if (tables.Count!= setting.TableNumber)
                {
                    setting.TableNumber = tables.Count;
                }
            }

            Settings.Initialized = true;



        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
            AppConfigurationManager.Save(Settings);
        }

        public event EventHandler<SettingsUpdatedEventArgs> SettingsUpdated;
        public override void BeforeNavigateAway()
        {
            SettingsUpdated?.Invoke(this,new SettingsUpdatedEventArgs(Settings.NumberOfCategories));
            ProductLayout.Instance.OnSettingsUpdated(this,new SettingsUpdatedEventArgs());
        }
    }
}
