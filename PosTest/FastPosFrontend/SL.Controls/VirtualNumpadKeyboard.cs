using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
        private const byte KEYEVENTF_KEYUP = 0x0002;

        public static readonly RoutedEvent KeyClickedEvent = EventManager.RegisterRoutedEvent(nameof(KeyClicked),
            RoutingStrategy.Bubble, typeof(VirtualKeyboardKeyClickedEventHandler), typeof(VirtualNumpadKeyboard));
        public static readonly RoutedEvent EnterKeyClickedEvent = EventManager.RegisterRoutedEvent(nameof(EnterKeyClicked),
            RoutingStrategy.Bubble, typeof(VirtualKeyboardKeyClickedEventHandler), typeof(VirtualNumpadKeyboard));

        static VirtualNumpadKeyboard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VirtualNumpadKeyboard),
                new FrameworkPropertyMetadata(typeof(VirtualNumpadKeyboard)));
        }

        public override void OnApplyTemplate()
        {
            _dispatcherTimer = new DispatcherTimer(DispatcherPriority.Background) {Interval = new TimeSpan(0, 0,0, 1,200)};
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
            _numpadNumericKeys = new List<Button>();
            for (var i = 0; i <= 9; i++)
            {
                if (!(this.Template.FindName($"Part_Numpad_Key_{i}_Button", this) is Button numpadKey)) continue;
                numpadKey.Click += NumpadKey_Click;
                _numpadNumericKeys.Add(numpadKey);
            }

            _numpadKeyEnterButton = this.Template.FindName("Part_Numpad_Key_Enter_Button", this) as Button;
            if (_numpadKeyEnterButton != null)
            {
                _numpadKeyEnterButton.Click += NumpadKey_Click;
                _numpadKeyEnterButton.Focusable = true;
            }
                
            
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
            
            _dispatcherTimer.Stop();
            var vCtrlKey = KeyInterop.VirtualKeyFromKey(Key.LeftCtrl);
            var vBackKey = KeyInterop.VirtualKeyFromKey(Key.Back);
            var vscanCtrl = MapVirtualKey((uint) vCtrlKey, 0);
            var vscanBack = MapVirtualKey((uint) vBackKey, 0);
            
            keybd_event((byte)vCtrlKey, (byte)vscanCtrl, 0, UIntPtr.Zero);
            keybd_event((byte)vBackKey, (byte) vscanBack, 0, UIntPtr.Zero);
            keybd_event((byte)vBackKey, (byte) vscanBack, KEYEVENTF_KEYUP, UIntPtr.Zero);
            keybd_event((byte)vCtrlKey, (byte) vscanCtrl, KEYEVENTF_KEYUP, UIntPtr.Zero);
            
        }


        private void NumpadKey_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Control control)) return;
            var key = (Key)control.Tag;
            RaiseKeyClickedEvent(key);
            var vKey = KeyInterop.VirtualKeyFromKey(key);
            keybd_event((byte)vKey, 0, 0, UIntPtr.Zero);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        public event VirtualKeyboardKeyClickedEventHandler KeyClicked
        {
            add => AddHandler(KeyClickedEvent, value);
            remove => RemoveHandler(KeyClickedEvent, value);
        }

        public event VirtualKeyboardKeyClickedEventHandler EnterKeyClicked
        {
            add => AddHandler(EnterKeyClickedEvent, value);
            remove => RemoveHandler(EnterKeyClickedEvent, value);
        }

        public event EventHandler<VirtualKeyboardBackspaceKeyHoldClickedEventArgs> BackspaceKeyHoldClicked;

        private void RaiseKeyClickedEvent(Key key)
        {
            RaiseEvent(new VirtualKeyboardKeyClickedEventArgs(KeyClickedEvent,this,key));
            if (key == Key.Enter)
            {
                RaiseEvent(new VirtualKeyboardKeyClickedEventArgs(EnterKeyClickedEvent,this,Key.Enter));
            }
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
        public VirtualKeyboardKeyClickedEventArgs(Key key)
        {
            Key = key;
        }

        public VirtualKeyboardKeyClickedEventArgs(RoutedEvent routedEvent, object source, Key key) : base(routedEvent, source)
        {
            Key = key;
        }

        public Key Key { get;  private set; }
    }
}