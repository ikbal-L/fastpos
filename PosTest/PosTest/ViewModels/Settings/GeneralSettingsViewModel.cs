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
        private string _tableNumber;

        public string TableNumber
        {
            get { return _tableNumber; }
            set { _tableNumber = value;
                NotifyOfPropertyChange(nameof(TableNumber));
            }
        }
        private string _numberCategores;

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

        public string NumberProducts
        {
            get { return _numberProducts; }
            set { _numberProducts = value;
                NotifyOfPropertyChange(nameof(NumberProducts));
            }
        }
        private string _serverHost;

        public string ServerHost
        {
            get { return _serverHost; }
            set { _serverHost = value;
                NotifyOfPropertyChange(nameof(ServerHost));
            }
        }


        public GeneralSettingsViewModel() {
            this.Index = 1;
            this.Title = "General Settings";
            this.Content = new GeneralSettingsView() { DataContext = this };
        }
    }
}
