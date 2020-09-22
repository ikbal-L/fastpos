using PosTest.Helpers;
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
    /// Interaction logic for NumericKeypad.xaml
    /// </summary>
    public partial class NumericKeypad : UserControl
    {
        public static readonly DependencyProperty NumericValueProperty =
                   DependencyProperty.Register(
                       nameof(NumericValue),
                       typeof(string),
                       typeof(NumericKeypad), 
                       new FrameworkPropertyMetadata
                        {
                            BindsTwoWayByDefault = true,
                            DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            DefaultValue=null
                        });

        public static readonly DependencyProperty DelCommandProperty =
                    DependencyProperty.Register(
                        "DelCommand", typeof(ICommand),
                        typeof(NumericKeypad), 
                        new FrameworkPropertyMetadata
                        {
                            DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            DefaultValue = null
                        });

        public static readonly DependencyProperty SwitchedProperty =
                    DependencyProperty.Register(nameof(Switched), 
                        typeof(bool),
                        typeof(NumericKeypad),
                       new FrameworkPropertyMetadata
                       {
                           BindsTwoWayByDefault = true,
                           DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                           DefaultValue = false
                       });

       public static readonly DependencyProperty AllowPercentKeyProperty =
                    DependencyProperty.Register(nameof(AllowPercentKey), 
                        typeof(bool),
                        typeof(NumericKeypad),
                       new FrameworkPropertyMetadata
                       {
                           BindsTwoWayByDefault = true,
                           DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                           DefaultValue = false
                       });

       public static readonly DependencyProperty AllowDotkeyProperty =
                    DependencyProperty.Register(nameof(AllowDotkey), 
                        typeof(bool),
                        typeof(NumericKeypad),
                       new FrameworkPropertyMetadata
                       {
                           BindsTwoWayByDefault = true,
                           DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                           DefaultValue = false
                       });

        public string NumericValue
        {
            get { return (string)GetValue(NumericValueProperty); }
            set { SetValue(NumericValueProperty, value); }
        }

        public ICommand DelCommand
        {
            get { return (ICommand)GetValue(DelCommandProperty); }
            set { SetValue(DelCommandProperty, value); }
        }

        public bool Switched
        {
            get { return (bool)GetValue(SwitchedProperty); }
            set { SetValue(SwitchedProperty, value); }
        }

        public bool AllowPercentKey
        {
            get { return (bool)GetValue(AllowPercentKeyProperty); }
            set { SetValue(AllowPercentKeyProperty, value); }
        }

        public bool AllowDotkey
        {
            get { return (bool)GetValue(AllowDotkeyProperty); }
            set { SetValue(AllowDotkeyProperty, value); }
        }

        public NumericKeypad()
        {
            InitializeComponent();
            DelCommand = new DelegateCommandBase((_) => DelAction());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string btnValue = (string)(sender as Button).Tag;
            NumericKeypadActions(btnValue);
        }

        private void DelAction()
        {
            if (string.IsNullOrEmpty(NumericValue))
            {
                return;
            }
            NumericValue = String.IsNullOrEmpty(NumericValue) ?
                String.Empty : NumericValue.Remove(NumericValue.Length - 1);
        }

        public void NumericKeypadActions(string key)
        {
            if (String.IsNullOrEmpty(key))
                return;
            if (key.Length > 1)
                return;
            var keys = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"};
            if (AllowDotkey)
            {
                keys = keys.Concat(new[] { "." }).ToArray();
            }
            if (AllowPercentKey)
            {
                keys = keys.Concat(new[] { "%" }).ToArray();
            }

            if (!keys.Contains(key))
                return;
            if (NumericValue == null || Switched)
            {
                NumericValue = String.Empty;
                Switched = false;
            }

            if (key.Equals("."))
            {
                if (NumericValue.Contains("."))
                {
                    return;
                }
                if (NumericValue.Contains("%"))
                {
                    NumericValue = NumericValue.Remove(NumericValue.Length - 1, 1) + "." + "%";

                }
                //NumericValue += key;
                return;
            }

            if (key.Equals("%"))
            {
                NumericValue = NumericValue.Contains("%") ? NumericValue : NumericValue + "%";
                return;
            }

            if (NumericValue.Contains("%"))
            {
                var percentStr = "";
                if (key != "%")
                {
                    percentStr = NumericValue.Remove(NumericValue.Length - 1, 1) + key;
                }
                else
                {
                    percentStr = NumericValue.Remove(NumericValue.Length - 1, 1);
                }
                if (percentStr == "")
                {
                    return;
                }
                    var percent = Convert.ToDecimal(percentStr);
                if (percent < 0 || percent > 100)
                {
                    NumericValue = percentStr.Substring(0, 2) + "%";
                }
                else if(key != "%")
                {
                    NumericValue = NumericValue.Remove(NumericValue.Length - 1, 1) + key + "%";
                }
                return;
            }

            NumericValue += key;
        }
    }
}
