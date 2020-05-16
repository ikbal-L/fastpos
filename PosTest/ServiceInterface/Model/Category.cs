using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
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
        private String _name { get; set; }

        public String Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }



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
 
        public List<Product> Products { get; set; }

        public Brush BackGroundColor => new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundString));








    }

   
}
