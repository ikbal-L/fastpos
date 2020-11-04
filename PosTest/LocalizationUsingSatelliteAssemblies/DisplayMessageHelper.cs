using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LocalizationUsingSatelliteAssemblies
{
    public class DisplayMessageHelper
    {
        public static void DisplayMessageBox(string message)
        {
            MessageBox.Show(message, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
