using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FastPosFrontend.SL.Controls.MDColorPalette
{
    /// <summary>
    /// Interaction logic for MDColorPalette.xaml
    /// </summary>
    public partial class MDColorPalette : UserControl,INotifyPropertyChanged
    {

        private readonly PaletteHelper _paletteHelper = new PaletteHelper();

        public List<ISwatch> Swatches { get; private set; }

        private static string[] _exclude= { "Brown" , "Grey" , "Blue Grey" };

        private static List<ISwatch> _swatches = SwatchHelper.Swatches.Where(s => !_exclude.Contains(s.Name)).ToList();

        public MDColorPalette()
        {
            
            InitializeComponent();
            Swatches = _swatches;
            ChangeHueCommand = new AnotherCommandImplementation(ChangeHue);
        }

        public Color MDColor
        {
            get { 
                return (Color)GetValue(MDColorProperty); 
            }
            set {

                if (value!= MDColor)
                {
                    SetValue(MDColorProperty, value);
                    OnProperyChanged();
                }

            }
        }

        public static readonly DependencyProperty MDColorProperty =
            DependencyProperty.Register("MDColor",
                typeof(Color), typeof(MDColorPalette),new FrameworkPropertyMetadata(default(Color),FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColorPropertyChangedCallback));

        private static void ColorPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colorPicker = (MDColorPalette)d;
            
            colorPicker.SetCurrentValue(MDColorProperty, (Color)e.NewValue);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ChangeHueCommand { get; }

        private void ChangeHue(object obj)
        {
            var hue = (Color)obj;
            MDColor = hue;
        }

        private void OnProperyChanged([CallerMemberName] string propertName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertName));
        }

    }
}
