using Caliburn.Micro;
using FastPosFrontend.ViewModels;
using FastPosFrontend.ViewModels.Settings;
using FastPosFrontend.ViewModels.SubViewModel;
using ServiceLib.Service;

namespace FastPosFrontend.Helpers
{
    public static class ProductLayout
    {
        public static int Columns = 6;
        public static int Rows = 5;
        public static int New;
        public static int Cols;
        public static int CategoryCols;


        static ProductLayout()
        {
            var setting = AppConfigurationManager.Configuration<ProductLayoutConfiguration>();
            if (setting == null)
            {
                setting = new ProductLayoutConfiguration() { Rows = 5, Columns = 6 };
                AppConfigurationManager.Save(nameof(ProductLayoutConfiguration),setting);
            }

            Rows = setting.Rows;
            Columns = setting.Columns;
            Cols = Columns;
            CategoryCols = AppConfigurationManager.Configuration<GeneralSettings>().NumberOfCategories;
        }

       
    }
}