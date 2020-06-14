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

namespace Softlines.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:NumericPad"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:NumericPad;assembly=NumericPad"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class NumericPad : Control
    {
        public static readonly DependencyProperty NumericValueProperty =
                  DependencyProperty.Register(
                      nameof(NumericValue), typeof(string),
                      typeof(NumericPad),
                      new FrameworkPropertyMetadata
                      {
                          BindsTwoWayByDefault = true,
                          DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                          DefaultValue = null
                      });

        public static readonly DependencyProperty DelCommandProperty =
                    DependencyProperty.Register(
                        "DelCommand", typeof(ICommand),
                        typeof(NumericPad),
                        new FrameworkPropertyMetadata
                        {
                            DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            DefaultValue = null
                        });

        public static readonly DependencyProperty SwitchedProperty =
                    DependencyProperty.Register(nameof(Switched),
                        typeof(bool),
                        typeof(NumericPad),
                       new FrameworkPropertyMetadata
                       {
                           BindsTwoWayByDefault = true,
                           DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                           DefaultValue = false
                       });

        public static readonly DependencyProperty AllowPercentKeyProperty =
                     DependencyProperty.Register(nameof(AllowPercentKey),
                         typeof(bool),
                         typeof(NumericPad),
                        new FrameworkPropertyMetadata
                        {
                            BindsTwoWayByDefault = true,
                            DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            DefaultValue = false
                        });

        public static readonly DependencyProperty AllowDotkeyProperty =
                     DependencyProperty.Register(nameof(AllowDotkey),
                         typeof(bool),
                         typeof(NumericPad),
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
        static NumericPad()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericPad), new FrameworkPropertyMetadata(typeof(NumericPad)));
        }

        public NumericPad()
        {
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

        private void NumericKeypadActions(string key)
        {
            if (String.IsNullOrEmpty(key))
                return;
            if (key.Length > 1)
                return;
            var keys = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
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
                NumericValue += key;
                return;
            }

            if (key.Equals("%"))
            {
                NumericValue = NumericValue.Contains("%") ? NumericValue : NumericValue + "%";

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
                else if (key != "%")
                {
                    NumericValue = NumericValue.Remove(NumericValue.Length - 1, 1) + key + "%";
                }
                return;
            }

            NumericValue += key;
        }
    }
}
