using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
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
}
