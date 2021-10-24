using SharpVectors.Converters;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace FastPosFrontend
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LoadExternalResourceDictionary("./Resources/External/Reports.xaml");
        }

        private void LoadExternalResourceDictionary(string FilePath)
        {
            try
            {
                XmlReader XmlRead = XmlReader.Create(FilePath);
                var rd = (ResourceDictionary)XamlReader.Load(XmlRead);
              
                Application.Current.Resources.MergedDictionaries.Add(rd);
                
                XmlRead.Close();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }
    }
}
