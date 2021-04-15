using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using FastPosFrontend.Helpers;

namespace FastPosFrontend.SL.Controls
{
    public class VirtualNumpadKeyboard : Control, IVirtualKeyboardKeyClickedEventHandler,
        IVirtualKeyboardBackspaceKeyHoldClickedEventHandler
    {
        private List<Button> _numpadNumericKeys;
        private Button _numpadKeyEnterButton;
        private Button _numpadKeyBackspaceButton;
        private DispatcherTimer _dispatcherTimer;

        public static readonly RoutedEvent KeyClickedEvent = EventManager.RegisterRoutedEvent(nameof(KeyClickedEvent),
            RoutingStrategy.Bubble, typeof(VirtualKeyboardKeyClickedEventHandler), typeof(VirtualNumpadKeyboard));

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
                _numpadKeyBackspaceButton.PreviewMouseLeftButtonDown +=
                    _numpadKeyBackspaceButton_PreviewMouseLeftButtonDown;
                _numpadKeyBackspaceButton.PreviewMouseLeftButtonUp +=
                    _numpadKeyBackspaceButton_PreviewMouseLeftButtonUp;
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
            BackspaceKeyHoldClicked?.Invoke(this, new VirtualKeyboardBackspaceKeyHoldClickedEventArgs());
        }


        private void NumpadKey_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Control control)) return;
            var key = control.Tag as string;
            RaiseKeyClickedEvent(key);
        }

        public event VirtualKeyboardKeyClickedEventHandler KeyClicked
        {
            add => AddHandler(KeyClickedEvent, value);
            remove => RemoveHandler(KeyClickedEvent, value);
        }

        public event EventHandler<VirtualKeyboardBackspaceKeyHoldClickedEventArgs> BackspaceKeyHoldClicked;

        private void RaiseKeyClickedEvent(string key)
        {
            RaiseEvent(new VirtualKeyboardKeyClickedEventArgs(key));
        }
    }

    public delegate void VirtualKeyboardKeyClickedEventHandler(object sender, VirtualKeyboardKeyClickedEventArgs e);

    public interface IVirtualKeyboardKeyClickedEventHandler
    {
        event VirtualKeyboardKeyClickedEventHandler KeyClicked;
    }

    public interface IVirtualKeyboardBackspaceKeyHoldClickedEventHandler
    {
        event EventHandler<VirtualKeyboardBackspaceKeyHoldClickedEventArgs> BackspaceKeyHoldClicked;
    }

    public class VirtualKeyboardBackspaceKeyHoldClickedEventArgs : EventArgs
    {
    }

    public class VirtualKeyboardKeyClickedEventArgs : RoutedEventArgs
    {
        public VirtualKeyboardKeyClickedEventArgs(string key)
        {
            Key = key;
        }

        public string Key { get; private set; }
    }
}