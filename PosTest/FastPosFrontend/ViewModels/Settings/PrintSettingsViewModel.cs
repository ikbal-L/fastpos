using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using Newtonsoft.Json;
using ServiceLib.Service;

namespace FastPosFrontend.ViewModels.Settings
{
    [NavigationItem("Print Settings", typeof(PrintSettingsViewModel), groupName: "Settings")]
    public class PrintSettingsViewModel : AppScreen
    {
        private ObservableCollection<PrinterItem> _Printers;

        public ObservableCollection<PrinterItem> Printers
        {
            get { return _Printers; }
            set {
                _Printers = value;
                NotifyOfPropertyChange(() => Printers);

            }
        }
        
        public PrintSettingsViewModel()
        {
            //this.Title = "Printers";
            //this.Content = new PrintSettingsView() { DataContext = this };
            Printers = new ObservableCollection<PrinterItem>();
            if (!AppConfigurationManager.ContainsKey("PrintSettings"))
            {
                AppConfigurationManager.Save("PrintSettings", new List<PrinterItem>());
            }
            
            
            var listSettings = AppConfigurationManager.Configuration<List<PrinterItem>>("PrintSettings");
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                var printerSetting = listSettings?.FirstOrDefault(x => x.Name.Equals(printer));
                printerSetting = printerSetting == null ? new PrinterItem(printer) : printerSetting;
                printerSetting.PropertyChanged += PrinterSetting_PropertyChanged;
                Printers.Add(printerSetting);

            }
        }

        private void PrinterSetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(PrinterItem.SelectedKitchen))|| e.PropertyName.Equals(nameof(PrinterItem.SelectedReceipt)))
            {
                //Settings.SaveSettings(Printers.ToList());
                AppConfigurationManager.Save("PrintSettings", Printers.ToList());
            }
        }
    }

    public class PrinterItem : PropertyChangedBase
    {

        private bool _selectedKitchen;
        [JsonProperty]

        public bool SelectedKitchen
        {
            get { return _selectedKitchen; }
            set
            {
                _selectedKitchen = value;
                NotifyOfPropertyChange(() => SelectedKitchen);

            }
        }
        private bool _selectedReceipt;
        [JsonProperty]

        public bool SelectedReceipt
        {
            get { return _selectedReceipt; }
            set
            {
                _selectedReceipt = value;
                NotifyOfPropertyChange(() => SelectedReceipt);
            }
        }

        private string _name;
        [JsonProperty]

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Set(ref _name, value);
            }
        }
        public PrinterItem(string name)
        {
            Name = name;
        }



    }
}
