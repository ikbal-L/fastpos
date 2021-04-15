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
using System.Windows.Threading;
using FastPosFrontend.Helpers;

namespace FastPosFrontend.SL.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:FastPosFrontend.SL.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:FastPosFrontend.SL.Controls;assembly=FastPosFrontend.SL.Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:VirtualNumpadKeyboard/>
    ///
    /// </summary>
    public class VirtualNumpadKeyboard : Control,IVirtualKeyboardKeyClickedEventHandler,IVirtualKeyboardBackspaceKeyHoldClickedEventHandler
    {
        private List<Button> _numpadNumericKeys;
        private Button _numpadKeyEnterButton;
        private Button _numpadKeyBackspaceButton;
        private DispatcherTimer _dispatcherTimer;

        static VirtualNumpadKeyboard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VirtualNumpadKeyboard),
                new FrameworkPropertyMetadata(typeof(VirtualNumpadKeyboard)));
        }

        public override void OnApplyTemplate()
        {
            _dispatcherTimer = new DispatcherTimer(DispatcherPriority.Background) {Interval = new TimeSpan(0, 0, 3)};
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
            _numpadNumericKeys = new List<Button>();
            for (var i = 0; i <= 9; i++)
            {
                if (!(this.Template.FindName($"Part_Numpad_Key_{i}_Button", this) is Button numpadKey)) continue;
                numpadKey.Click += NumpadKey_Click;
                _numpadNumericKeys.Add(numpadKey);
            }

            _numpadKeyEnterButton = this.Template.FindName("Part_Numpad_Key_Enter_Button", this) as Button;
            if (_numpadKeyEnterButton != null) _numpadKeyEnterButton.Click += NumpadKey_Click;
            _numpadKeyBackspaceButton = this.Template.FindName("Part_Numpad_Key_Backspace_Button", this) as Button;
            if (_numpadKeyBackspaceButton != null)
            {
                _numpadKeyBackspaceButton.Click += NumpadKey_Click;
                _numpadKeyBackspaceButton.PreviewMouseLeftButtonDown += _numpadKeyBackspaceButton_PreviewMouseLeftButtonDown;
                _numpadKeyBackspaceButton.PreviewMouseLeftButtonUp += _numpadKeyBackspaceButton_PreviewMouseLeftButtonUp;
                
            }
            base.OnApplyTemplate();
        }
        
        private void _numpadKeyBackspaceButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dispatcherTimer.Start();
        }

        private void _numpadKeyBackspaceButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _dispatcherTimer.Stop();
        }

        private void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ToastNotification.Notify("Clearing");
            _dispatcherTimer.Stop();
            BackspaceKeyHoldClicked?.Invoke(this,new VirtualKeyboardBackspaceKeyHoldClickedEventArgs());

        }

        

        

        private void NumpadKey_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Control control)
            {
                var key = control.Tag as string;
                KeyClicked?.Invoke(this,new VirtualKeyboardKeyClickedEventArgs(key));
            }
        }

        public event EventHandler<VirtualKeyboardKeyClickedEventArgs> KeyClicked;
        public event EventHandler<VirtualKeyboardBackspaceKeyHoldClickedEventArgs> BackspaceKeyHoldClicked;
    }

    public interface IVirtualKeyboardKeyClickedEventHandler
    {
        event EventHandler<VirtualKeyboardKeyClickedEventArgs> KeyClicked;
    }
    public interface IVirtualKeyboardBackspaceKeyHoldClickedEventHandler
    {
        event EventHandler<VirtualKeyboardBackspaceKeyHoldClickedEventArgs> BackspaceKeyHoldClicked;
    }

    public class VirtualKeyboardBackspaceKeyHoldClickedEventArgs:EventArgs
    {   
    }

    public class VirtualKeyboardKeyClickedEventArgs:EventArgs
    {
        public VirtualKeyboardKeyClickedEventArgs(string key)
        {
            Key = key;
        }

        public string Key { get; private set; }
    }
}