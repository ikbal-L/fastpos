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
        private Brush _backGroundColor;

        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
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

        [DataMember]
        public List<long> ProductIds { get; set; }
        public List<Product> Products { get; set; }

        public Brush BackGroundColor => _backGroundColor ?? (_backGroundColor=new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundString)));
    }

   
}
