using System.Windows;

namespace LocalizationUsingSatelliteAssemblies
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MessageHelper helper;
        public MainWindow()
        {
            
            InitializeComponent();
            helper = new MessageHelper();
        }

        private void Message1Button_OnClick(object sender, RoutedEventArgs e)
        {
            DisplayMessageHelper.DisplayMessageBox(helper.CODE_200);
        }

        private void Message2Button_OnClick(object sender, RoutedEventArgs e)
        {
            DisplayMessageHelper.DisplayMessageBox(helper.ERROR500);
        }

        private void Message3Button_OnClick(object sender, RoutedEventArgs e)
        {
            DisplayMessageHelper.DisplayMessageBox(helper.ERROR_404);
        }


    }
}
