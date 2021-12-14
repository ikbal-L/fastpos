using System;
using System.Linq;
using FastPosFrontend.Configurations;
using FastPosFrontend.Events;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using ServiceInterface.Model;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels.Settings
{

    [NavigationItem(title:"General Settings",typeof(GeneralSettingsViewModel),"",groupName:"Settings",isQuickNavigationEnabled:true)]
    [PreAuthorize("Modify_Local_Settings")]
    public  class GeneralSettingsViewModel : AppScreen,ISettingsController
    {
        public GeneralSettings Settings { get; set; }
     

        public GeneralSettingsViewModel() {
            //this.Index = 1;
            Title = "General";

            Settings = ConfigurationManager.Get<PosConfig>().General;
            Settings.PropertyChanged += Settings_PropertyChanged;
           
            var tables = StateManager.GetAll<Table>();
            if (!tables.Any())
            {
                Settings.TableCount = 0;
            }
            else
            {
                if (tables.Count!= Settings.TableCount)
                {
                    Settings.TableCount = tables.Count;
                }
            }

            Settings.Initialized = true;



        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
            Settings.RequestSave();
        }

        public event EventHandler<SettingsUpdatedEventArgs> SettingsUpdated;
        public override void BeforeNavigateAway()
        {
            SettingsUpdated?.Invoke(this,new SettingsUpdatedEventArgs(Settings.CategoryPageSize));
            ProductLayout.Instance.OnSettingsUpdated(this,new SettingsUpdatedEventArgs());
        }
    }
}
