using Caliburn.Micro;
using FastPosFrontend.Helpers;
using Newtonsoft.Json;

namespace FastPosFrontend.ViewModels.SubViewModel
{
    public class ProductLayoutViewModel : PropertyChangedBase 
    {
        private int _columns;
        private int _rows;
        private readonly ProductLayoutConfiguration _configuration;


        public ProductLayoutViewModel()
        {
            Manager = new SettingsManager<ProductLayoutConfiguration>("product.layout.config");
            var configuration = Manager.LoadSettings();
            _configuration = configuration;
            if (_configuration == null)
            {
                _configuration = new ProductLayoutConfiguration(){Columns = 0,Rows = 0};
            }

            Columns = _configuration.Columns;
            Rows = _configuration.Rows;
        }

        public SettingsManager<ProductLayoutConfiguration> Manager { get; set; }

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
            Manager.SaveSettings(_configuration);
        }

        
    }

    public class ProductLayoutConfiguration : PropertyChangedBase
    {
        private int _columns;
        private int _rows;

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
    }
}