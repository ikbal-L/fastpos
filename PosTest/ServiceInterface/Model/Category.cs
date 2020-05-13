using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media;

namespace ServiceInterface.Model
{
    public class Category : PropertyChangedBase
    {
        private string _backGroundString=null;
        public int Id { get; set; }
        public String Name { get; set; }
        public string Description { get; set; }

        public string BackgroundString 
        { 
            get => _backGroundString; 
            set 
            { 
                _backGroundString = value;
                NotifyOfPropertyChange(() => BackgroundString); 
                NotifyOfPropertyChange(() => BackGroundColor); 
            } 
        }
        [ScriptIgnore]
        public List<Product> Products { get; set; }

        [ScriptIgnore]
        public Brush BackGroundColor => new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundString));
    }
}
