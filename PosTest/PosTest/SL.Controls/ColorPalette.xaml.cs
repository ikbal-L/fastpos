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

namespace PosTest.SL.Controls
{
    /// <summary>
    /// Interaction logic for ColorPalette.xaml
    /// </summary>
    public partial class ColorPalette : UserControl
    {



        public SolidColorBrush CPColorBrush
        {
            get { return (SolidColorBrush)GetValue(CPColorBrushProperty); }
            set { SetValue(CPColorBrushProperty, value); }
        }



        public static readonly DependencyProperty CPColorBrushProperty = 
            DependencyProperty.Register("CPColorBrush", 
                typeof(SolidColorBrush), typeof(ColorPalette), 
                new FrameworkPropertyMetadata(default(object),FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ColorPalette()
        {
            InitializeComponent();
            //this.DataContext = this;
        }
    }
}
