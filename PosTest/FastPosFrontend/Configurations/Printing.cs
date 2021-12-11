using FastPosFrontend.ViewModels.Settings;
using Newtonsoft.Json;
using ServiceLib.Service;
using System;
using System.Collections.Generic;

namespace FastPosFrontend.Configurations
{
    public class Printing :IConfigurationProperty
    {
        [JsonProperty]
        public List<PrinterItem> Printers { get; set; } = new List<PrinterItem>();

        public event EventHandler<SaveRequestedEventArgs> SaveRequested;

        public void RequestSave()
        {
            SaveRequested?.Invoke(this, new SaveRequestedEventArgs());
        }
    }
}