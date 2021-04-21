using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using FastPosFrontend.Views.Settings;
using Newtonsoft.Json;
using ServiceInterface.Model;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;

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
    public class GeneralSettings: PropertyChangedBase
    {
        
        private int _tableNumber;
        private string _serverHost;
        private bool _initialized = false;
        private int _numberCategores;

        public bool Initialized
        {
            get => _initialized;
            set => Set(ref _initialized, value);
        }

        [JsonProperty]
        public int TableNumber
        {
            get => _tableNumber;
            set
            {

                if (value>=0)
                {
                    var oldValue = _tableNumber;
                    var changesCommitted = false;
                    if (Initialized)
                    {
                        var d = value- oldValue ;
                        
                        if (d < 0)
                        {
                            changesCommitted = DeleteTables(Math.Abs(d));
                        }
                        else if (d>0)
                        {
                            changesCommitted =CreateTables(Math.Abs(d));
                        }

                        if (changesCommitted)
                        {
                            Set(ref _tableNumber, value);
                        }
                    }
                    else
                    {
                        Set(ref _tableNumber, value);
                    }
                }
                else
                {
                    ToastNotification.Notify("Enter a valid number");
                }
                


            }
        }
        
        [JsonProperty]

        public int NumberCategores
        {
            get { return _numberCategores; }
            set
            {
                _numberCategores = value;
                NotifyOfPropertyChange(nameof(NumberCategores));
            }

        }

        private string _numberProducts;
        [JsonProperty]

        public string NumberProducts
        {
            get { return _numberProducts; }
            set
            {
                _numberProducts = value;
                NotifyOfPropertyChange(nameof(NumberProducts));
            }
        }
        

        [JsonProperty]

        public string ServerHost
        {
            get { return _serverHost; }
            set
            {
                _serverHost = value;
                NotifyOfPropertyChange(nameof(ServerHost));
            }
        }

        public bool DeleteTables(int limit)
        {
             var ids = StateManager.Get<Table>().Where(t=>t.Id != null).OrderByDescending(table => table.Number).Take(limit).Select((table, i) => table.Id.Value );
             return StateManager.Delete<Table,long>(ids);
        }

        public bool CreateTables(int limit)
        {
            var tables = StateManager.Get<Table>();
            var baseNumber = tables.Any()?tables.Max(table => table.Number):0;
            IList<Table> newTables = new List<Table>(limit);
            for (int i = 1 ; i <= limit; i++)
            {

                newTables.Add( new Table(){Number = baseNumber+i});
            }

            var result = StateManager.Save(newTables);
            if (result)
            {
                
            }
            return result;
        }

    }
}
