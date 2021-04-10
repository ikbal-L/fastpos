using Caliburn.Micro;
using FastPosFrontend.ViewModels.SubViewModel;

namespace FastPosFrontend.Helpers
{
    public class ProductLayout :PropertyChangedBase
    {
        private static int _columns;
        private static int _rows;


        static ProductLayout()
        {
            var settingsManager = new SettingsManager<ProductLayoutConfiguration>("product.layout.config");
            var setting = settingsManager.LoadSettings();
            if (setting == null)
            {
                setting = new ProductLayoutConfiguration() { Rows = 5, Columns = 6 };
                settingsManager.SaveSettings(setting);
            }

            _rows = setting.Rows;
            _columns = setting.Columns;
        }

        private ProductLayout()
        {
            
        }

        public ProductLayout Instance => new ProductLayout();
        

        public int Columns
        {
            get => _columns;
            set => Set(ref _columns, value);
        }

        public int Rows
        {
            get => _rows;
            set => Set(ref _rows, value);
        }
    }
}