using System;
using Caliburn.Micro;
using FastPosFrontend.Events;
using FastPosFrontend.ViewModels;
using FastPosFrontend.ViewModels.Settings;
using FastPosFrontend.ViewModels.SubViewModel;
using ServiceLib.Service;

namespace FastPosFrontend.Helpers
{
    public class ProductLayout : PropertyChangedBase,ISettingsListener
    {
        public static int Columns = 6;
        public static int Rows = 5;
        public static int New;
        public static int Cols;
        private  int _categoryCols;
        private static ProductLayout _instance = new ProductLayout();


        private ProductLayout()
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
            _categoryCols = AppConfigurationManager.Configuration<GeneralSettings>().NumberOfCategories;
        }

        public static ProductLayout Instance => _instance;
        

        public int CategoryCols => _categoryCols;


        public Type[] SettingsControllers { get; }
        public void OnSettingsUpdated(object sender, SettingsUpdatedEventArgs e)
        {
            _categoryCols = AppConfigurationManager.Configuration<GeneralSettings>().NumberOfCategories;
            NotifyOfPropertyChange(nameof(CategoryCols));
           
        }
    }
}