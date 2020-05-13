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
using System.Windows.Media;

namespace ServiceInterface.Model
{
    public class Category : PropertyChangedBase
    {
        private Brush _backGroundColor=null;

        public int Id { get; set; }
        public String Name { get; set; }
        public string Description { get; set; }
        
        
        public List<Product> Products { get; set; }

        public Brush BackGroundColor { get => _backGroundColor;  set { _backGroundColor = value;  NotifyOfPropertyChange(() => BackGroundColor); } }
         
    }

   
    public class Category1 : PropertyChangedBase
    {
        
        private Brush _backGroundColor = null;
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public String Name { get; set; }
        [DataMember]
        public string Description { get; set; }

        //public event NotifyCollectionChangedEventHandler CollectionChanged;


        public List<Product> Products { get; set; }
        public bool IsNotifying { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyOfPropertyChange(string propertyName)
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        [DataMember]
        public Brush BackGroundColor { get => new SolidColorBrush(Color.FromArgb(0,5,5,5));  }

    }
}
