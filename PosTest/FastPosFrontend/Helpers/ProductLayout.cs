﻿using System;
using Caliburn.Micro;
using FastPosFrontend.Configurations;
using FastPosFrontend.Events;
using ServiceLib.Service;

namespace FastPosFrontend.Helpers
{
    public class ProductLayout : PropertyChangedBase, ISettingsListener
    {
        public static int Columns = 6;
        public static int Rows = 5;
        public static int New;
        public static int Cols;
        private  int _categoryCols;
        private static ProductLayout _instance = new ProductLayout();


        private ProductLayout()
        {
            var setting = ConfigurationManager.Get<PosConfig>().ProductLayout; 
          

            Rows = setting.Rows;
            Columns = setting.Columns;
            Cols = Columns;
            _categoryCols = ConfigurationManager.Get<PosConfig>().General.CategoryPageSize;
        }

        public static ProductLayout Instance => _instance;
        

        public int CategoryCols => _categoryCols;


        public Type[] SettingsControllers { get; }
        public void OnSettingsUpdated(object sender, SettingsUpdatedEventArgs e)
        {
            _categoryCols = ConfigurationManager.Get<PosConfig>().General.CategoryPageSize;
            NotifyOfPropertyChange(nameof(CategoryCols));
           
        }
    }
}