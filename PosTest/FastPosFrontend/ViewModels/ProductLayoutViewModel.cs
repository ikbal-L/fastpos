using FastPosFrontend.Configurations;
using Newtonsoft.Json;
using ServiceLib.Service;
using Action = System.Action;

namespace FastPosFrontend.ViewModels
{
    public class LayoutConfigurationViewModel<T> : DialogContent  where T:LayoutConfiguration
    {
        private int _columns;
        private int _rows;
        private readonly T _configuration;
        private Action _layoutChangedHandler;


        public LayoutConfigurationViewModel(T configuration)
        {
            _configuration =  configuration;
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

        public T Configuration => _configuration;

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


    public class ProductLayoutViewModel: LayoutConfigurationViewModel<ProductLayoutConfiguration>
    {
        public ProductLayoutViewModel() :base(ConfigurationManager.Get<PosConfig>().ProductLayout)
        {
        }
    }

    public class CategoryLayoutViewModel : LayoutConfigurationViewModel<CategoryLayoutConfiguration>
    {
        public CategoryLayoutViewModel() : base(ConfigurationManager.Get<PosConfig>().CategoryLayout)
        {
        }
    }

    public class AdditiveLayoutViewModel : LayoutConfigurationViewModel<AdditiveLayoutConfiguration>
    {
        public AdditiveLayoutViewModel() : base(ConfigurationManager.Get<PosConfig>().AdditiveLayout)
        {
        }
    }
}