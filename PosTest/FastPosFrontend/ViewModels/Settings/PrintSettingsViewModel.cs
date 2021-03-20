using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using FastPosFrontend.Views.Settings;
using Newtonsoft.Json;

namespace FastPosFrontend.ViewModels.Settings
{
    public class PrintSettingsViewModel : SettingsItemBase
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
        private SettingsManager<List<PrinterItem>> Settings;
        public PrintSettingsViewModel()
        {
            this.Title = "Printers";
            this.Content = new PrintSettingsView() { DataContext = this };
            Printers = new ObservableCollection<PrinterItem>();
            Settings = new SettingsManager<List<PrinterItem>>("PrintSettings.json");
            var listSettings = Settings.LoadSettings();
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
                Settings.SaveSettings(Printers.ToList());
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
            this.Name = name;
        }



    }
}
