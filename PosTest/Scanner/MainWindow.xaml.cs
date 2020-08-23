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

namespace Scanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private string barCode = string.Empty; //TO DO use a StringBuilder instead

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new CustomerViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        //private void Label_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //        if (44 == (int)e.Key) e.Handled = true;
            
        //        barCode += e.Key;
        //    //look for a terminator char (different barcode scanners output different 
        //    //control characters like tab and line feeds), a barcode char length and other criteria 
        //    //like human typing speed &/or a lookup to confirm the scanned input is a barcode, eg.
        //    if (barCode.Length == 7)
        //    {
        //        //var foundItem = DoLookUp(barCode);
        //        barCode = string.Empty;
        //    }
        //}
    }
}
