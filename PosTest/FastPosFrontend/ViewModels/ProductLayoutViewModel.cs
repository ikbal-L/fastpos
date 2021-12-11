using FastPosFrontend.Configurations;
using Newtonsoft.Json;
using ServiceLib.Service;
using Action = System.Action;

namespace FastPosFrontend.ViewModels
{
    public class ProductLayoutViewModel : DialogContent 
    {
        private int _columns;
        private int _rows;
        private readonly ProductLayoutConfiguration _configuration;
        private Action _layoutChangedHandler;


        public ProductLayoutViewModel()
        {
            _configuration = ConfigurationManager.Get<PosConfig>().ProductLayout;

            Columns = _configuration.Columns;
            Rows = _configuration.Rows;
        }

        [JsonProperty]
        public int Columns
        {
            get => _columns;
            set => Set(ref _columns, value);
        }

        [JsonProperty]
        public int Rows
        {
            get => _rows;
            set => Set(ref _rows, value);
        }

        public ProductLayoutConfiguration Configuration => _configuration;

        public void Apply()
        {
            _configuration.Rows = Rows;
            _configuration.Columns = Columns;
            _configuration.RequestSave();
            _layoutChangedHandler?.Invoke();
            Host.Close(this);
        }

        public void Cancel()
        {
            Host.Close(this);
        }

        public void OnLayoutChanged(Action layoutChangedHandler)
        {
            _layoutChangedHandler = layoutChangedHandler;
        }

        
    }
}