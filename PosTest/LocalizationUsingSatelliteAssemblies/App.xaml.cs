using System.Globalization;
using System.Threading;
using System.Windows;

namespace LocalizationUsingSatelliteAssemblies
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            CultureInfo ci = new CultureInfo("ar-DZ");
            //CultureInfo ci = new CultureInfo("fr-DZ");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}
