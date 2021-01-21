using Caliburn.Micro;
using Newtonsoft.Json;
using PosTest.Helpers;
using PosTest.Views.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.ViewModels.Settings
{
  public  class GeneralSettingsViewModel : SettingsItemBase
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
            this.Index = 1;
            this.Title = "General Settings";
            Manager = new SettingsManager<GeneralSettings>("GeneralSettings.json");
            var setting = Manager.LoadSettings();
            Settings = setting != null ? setting : new GeneralSettings();
            Settings.PropertyChanged += Settings_PropertyChanged;
            this.Content = new GeneralSettingsView() { DataContext = this };
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Manager.SaveSettings(this.Settings);
        }
    }
    public class GeneralSettings: PropertyChangedBase
    {

        private string _tableNumber;
        [JsonProperty]
        public string TableNumber
        {
            get { return _tableNumber; }
            set
            {
                _tableNumber = value;
                NotifyOfPropertyChange(nameof(TableNumber));
            }
        }
        private string _numberCategores;
        [JsonProperty]

        public string NumberCategores
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
        private string _serverHost;
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

    }
}
