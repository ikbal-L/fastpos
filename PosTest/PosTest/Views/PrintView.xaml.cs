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
using PosTest.ViewModels;

namespace PosTest.Views
{
    /// <summary>
    /// Interaction logic for PrintView.xaml
    /// </summary>
    public partial class PrintView : UserControl
    {
        public PrintView()
        {
            InitializeComponent();
        }

        //protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    if (e.Property.Name == nameof(DataContext)&& e.NewValue!=null&& e.NewValue is PrintViewModel)
        //    {
        //        Viewer.Document = (DataContext as PrintViewModel).Document;
        //    }
        //}
    }
    
}
