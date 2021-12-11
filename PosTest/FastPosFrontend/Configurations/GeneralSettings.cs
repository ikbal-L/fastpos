using System;
using System.Collections.Generic;
using System.Linq;
using FastPosFrontend.Helpers;
using Caliburn.Micro;
using Newtonsoft.Json;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;
using ServiceLib.Service;

namespace FastPosFrontend.Configurations
{
    public class GeneralSettings : PropertyChangedBase,IConfigurationProperty
    {

        private int _tableNumber;
        private string _serverHost;
        private bool _initialized = false;
        private int _categoryPageSize = 4;
        private string _restaurantName;
        private bool _isRefundEnabled;
        private bool isDeliveryEnabled;
        private bool isMultiCashRegisterEnabled;
        private bool _isBarcodeEnabled;

        public event EventHandler<SaveRequestedEventArgs> SaveRequested;

        public bool Initialized
        {
            get => _initialized;
            set => Set(ref _initialized, value);
        }

        [JsonProperty]
        public int TableCount
        {
            get => _tableNumber;
            set
            {

                if (value >= 0)
                {
                    var oldValue = _tableNumber;
                    var changesCommitted = false;
                    if (Initialized)
                    {
                        var d = value - oldValue;

                        if (d < 0)
                        {
                            changesCommitted = DeleteTables(Math.Abs(d));
                        }
                        else if (d > 0)
                        {
                            changesCommitted = CreateTables(Math.Abs(d));
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

        public int CategoryPageSize
        {
            get { return _categoryPageSize; }
            set
            {
                _categoryPageSize = value;
                NotifyOfPropertyChange(nameof(CategoryPageSize));
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

        [JsonProperty]
        public string RestaurantName
        {
            get => _restaurantName;
            set => Set(ref _restaurantName, value);
        }

        [JsonProperty]
        public bool IsRefundEnabled
        {
            get => _isRefundEnabled;
            set
            {
                Set(ref _isRefundEnabled, value);
            }
        }

        [JsonProperty]
        public bool IsDeliveryEnabled
        {
            get => isDeliveryEnabled;
            set
            {
                Set(ref isDeliveryEnabled, value);
            }
        }

        [JsonProperty]
        public bool IsMultiCashRegisterEnabled
        {
            get => isMultiCashRegisterEnabled;
            set { Set(ref isMultiCashRegisterEnabled, value); }
        }

        [JsonProperty]
        public bool IsBarcodeEnabled
        {
            get => _isBarcodeEnabled;
            set => Set(ref _isBarcodeEnabled, value);
        }



        public bool DeleteTables(int limit)
        {
            var ids = StateManager.Get<Table>().Where(t => t.Id != null).OrderByDescending(table => table.Number).Take(limit).Select((table, i) => table.Id.Value);
            return StateManager.Delete<Table, long>(ids);
        }

        public bool CreateTables(int limit)
        {
            var tables = StateManager.Get<Table>();
            var baseNumber = tables.Any() ? tables.Max(table => table.Number) : 0;
            IList<Table> newTables = new List<Table>(limit);
            for (int i = 1; i <= limit; i++)
            {

                newTables.Add(new Table() { Number = baseNumber + i });
            }

            var result = StateManager.Save(newTables);
            return result;
        }

        public void RequestSave()
        {
            SaveRequested?.Invoke(this,new SaveRequestedEventArgs());
        }


    }

}