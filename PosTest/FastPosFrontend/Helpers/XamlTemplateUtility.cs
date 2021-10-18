using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Resources;
using System.Xml;
using System.Xml.Linq;


namespace FastPosFrontend.Helpers
{
    public class XamlTemplateUtility
    {
        

        private static string LoadXamlFromFile()
        {
            StreamResourceInfo strm = Application.GetResourceStream(new Uri("./Resources/Dynamic/Reports.xaml"));
            XElement x = XElement.Load(strm.Stream);
            var t = new DataTemplate();
            return x.ToString();
        }
    }
}
