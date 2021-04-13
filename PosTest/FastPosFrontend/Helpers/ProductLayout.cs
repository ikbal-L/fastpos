using Caliburn.Micro;
using FastPosFrontend.ViewModels.SubViewModel;

namespace FastPosFrontend.Helpers
{
    public static class ProductLayout
    {
        public static int Columns = 6;
        public static int Rows = 5;
        public static int New;
        public static int Cols;


        static ProductLayout()
        {
            var settingsManager = new SettingsManager<ProductLayoutConfiguration>("product.layout.config");
            var setting = settingsManager.LoadSettings();
            if (setting == null)
            {
                setting = new ProductLayoutConfiguration() { Rows = 5, Columns = 6 };
                settingsManager.SaveSettings(setting);
            }

            Rows = setting.Rows;
            Columns = setting.Columns;
            Cols = Columns;
        }

       
    }
}