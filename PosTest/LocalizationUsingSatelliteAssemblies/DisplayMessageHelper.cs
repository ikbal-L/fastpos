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
