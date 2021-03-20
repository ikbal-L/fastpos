using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FastPosFrontend.SL.Controls
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
        }
    }
}
