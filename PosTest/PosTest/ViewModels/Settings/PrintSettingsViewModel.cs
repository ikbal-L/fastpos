using Caliburn.Micro;
using PosTest.Helpers;
using PosTest.Views.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PosTest.ViewModels.Settings
{
  public class PrintSettingsViewModel: SettingsItemBase
    {
        private ObservableCollection<PrinterItem> _ReceiptPrinters;

        public ObservableCollection<PrinterItem> ReceiptPrinters
        {
            get { return _ReceiptPrinters; }
            set {
                _ReceiptPrinters = value;
                NotifyOfPropertyChange(() => _ReceiptPrinters);

            }
        }
        public PrintSettingsViewModel()
        {
            this.Title = "Printers";
            this.Content = new PrintSettingsView() { DataContext = this };
            ReceiptPrinters = new ObservableCollection<PrinterItem>();
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                ReceiptPrinters.Add(new PrinterItem(printer)); ;
            }
        }
    }
    public class  PrinterItem:PropertyChangedBase
    {

        private bool _selected;

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value;
                NotifyOfPropertyChange(() => Selected);
              }
        }
        private string _name;

        public string Name
        {
            get { return _name; }
            set {  
                _name = value;
                Set(ref _name, value);
            }
        }
      public  PrinterItem(string name) {
            this.Name = name;
        }


     

    }
}
