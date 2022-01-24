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

        private int _categoryPageSize = 4;
        private string _restaurantName;
        private bool _isRefundEnabled;
        private bool isDeliveryEnabled;
        private bool isMultiCashRegisterEnabled;
        private bool _isBarcodeEnabled;

        public event EventHandler<SaveRequestedEventArgs> SaveRequested;

      

        [JsonProperty]
        public int TableCount
        {
            get => _tableNumber;
            set
            {
                Set(ref _tableNumber, value);
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



        public void RequestSave()
        {
            SaveRequested?.Invoke(this,new SaveRequestedEventArgs());
        }




    }

}