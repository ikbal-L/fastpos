
using Caliburn.Micro;
using Newtonsoft.Json;
using ServiceLib.Service;
using System;
using System.Diagnostics;

namespace FastPosFrontend.Configurations
{
    public class LayoutConfiguration : PropertyChangedBase,IConfigurationProperty
    {
        protected int _columns ;
        protected int _rows ;

        public LayoutConfiguration(int columns, int rows)
        {
            Debug.WriteLine($"from base LayoutConfiguration {_columns} {_rows}");
            _columns = columns;
            _rows = rows;
        }


        [JsonProperty]
        public int Columns
        {
            get => _columns;
            set
            {
                Set(ref _columns, value);
                NotifyOfPropertyChange(nameof(TotalElements));
            }
        }

        [JsonProperty]
        public int Rows
        {
            get => _rows;
            set
            {
                Set(ref _rows, value);
                NotifyOfPropertyChange(nameof(TotalElements));
            }
        }

        public int TotalElements => Rows * Columns;


        public event EventHandler<SaveRequestedEventArgs> SaveRequested;

        public void RequestSave()
        {
            SaveRequested?.Invoke(this, new SaveRequestedEventArgs());
        }
    }

    public class ProductLayoutConfiguration: LayoutConfiguration
    {


        public ProductLayoutConfiguration():base(5,4)
        {
           
        }
    }

    public class CategoryLayoutConfiguration : LayoutConfiguration
    {


        public CategoryLayoutConfiguration():base(5,3)
        {
          
        }
    }

    public class AdditiveLayoutConfiguration : LayoutConfiguration
    {
        public AdditiveLayoutConfiguration() : base(4, 4)
        {
        }
    }
}