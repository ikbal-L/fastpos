
using Caliburn.Micro;
using Newtonsoft.Json;
using ServiceLib.Service;
using System;

namespace FastPosFrontend.Configurations
{
    public class ProductLayoutConfiguration : PropertyChangedBase,IConfigurationProperty
    {
        private int _columns = 5;
        private int _rows = 4;

        [JsonProperty]
        public int Columns
        {
            get => _columns;
            set
            {
                Set(ref _columns, value);
                NotifyOfPropertyChange(nameof(NumberOfProducts));
            }
        }

        [JsonProperty]
        public int Rows
        {
            get => _rows;
            set
            {
                Set(ref _rows, value);
                NotifyOfPropertyChange(nameof(NumberOfProducts));
            }
        }

        public int NumberOfProducts => Rows * Columns;

        public event EventHandler<SaveRequestedEventArgs> SaveRequested;

        public void RequestSave()
        {
            SaveRequested?.Invoke(this, new SaveRequestedEventArgs());
        }
    }
}