using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using Caliburn.Micro;
using Newtonsoft.Json;
using ServiceLib.Service;
using FastPosFrontend.Configurations;

namespace FastPosFrontend.ViewModels.Settings
{
    [NavigationItem("Print Settings", typeof(PrintSettingsViewModel),"", groupName: "Settings",isQuickNavigationEnabled:true)]
    
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
       
            Printers = new ObservableCollection<PrinterItem>();

            
            
            var listSettings = ConfigurationManager.Get<PosConfig>().Printing.Printers;
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
                var printsettings = ConfigurationManager.Get<PosConfig>().Printing;
                printsettings.Printers = Printers.ToList();
                printsettings.RequestSave();
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
