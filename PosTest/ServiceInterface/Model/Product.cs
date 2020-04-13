using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ServiceInterface.Model
{
    public class Product : PropertyChangedBase
    {
        private string _backgroundString = null;
        private int nothing;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }

        

        public int CategorieId
        {
            get => Category == null ? 0 : Category.Id;
            set { nothing = value; }
        }
        public Category Category { get; set; }

        public string Type { get; set; }
        public string Color { get; set; }
        
        public string PictureFileName { get; set; }
        public string PictureUri { get; set; }

        public int AvailableStock { get; set; }

        public string BackgroundString { get => _backgroundString ?? "#f39c12"; }
        public Brush Background { 
            get => new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundString));
        }

        
    }
}
