using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
