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

namespace Softline.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SlCustomKeypad"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SlCustomKeypad;assembly=SlCustomKeypad"
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

        public static readonly DependencyProperty VerifyPercentValueProperty =
                     DependencyProperty.Register(nameof(VerifyPercentValue),
                         typeof(bool),
                         typeof(NumericPad),
                        new FrameworkPropertyMetadata
                        {
                            BindsTwoWayByDefault = true,
                            DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            DefaultValue = true
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
        public bool VerifyPercentValue
        {
            get { return (bool)GetValue(VerifyPercentValueProperty); }
            set { SetValue(VerifyPercentValueProperty, value); }
        }

        static NumericPad()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericPad), new FrameworkPropertyMetadata(typeof(NumericPad)));
        }

        public NumericPad()
        {
            DelCommand = new DelegateCommandBase((_) => DelAction());
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            foreach (var child in ((this.GetVisualChild(0) as Border).Child as Grid).Children)
            {
                var btn = child as Button;
                btn.Click += Button_Click;
            } 

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

            switch (key)
            {
                case ".":
                    if (NumericValue.Contains("."))
                    {
                        return;
                    }
                    else if (NumericValue.Contains("%"))
                    {
                        NumericValue = NumericValue.Insert(NumericValue.Length - 1,  ".");
                    }
                    else
                    {
                        NumericValue = NumericValue + ".";
                    }
                    break;

                case "%":
                    NumericValue = NumericValue.Contains("%") ? NumericValue : NumericValue + "%";
                    if (NumericValue != "%")
                    {
                        var percentStr = NumericValue.Remove(NumericValue.Length - 1, 1);
                        var percent = Convert.ToDecimal(percentStr);
                        if (percent > 100 && VerifyPercentValue)
                        {
                            NumericValue = percentStr.Substring(0, 2) + "%";
                        }
                    }
                    break;

                default:
                    if (NumericValue.Contains("%"))
                    {
                        var percentStr = NumericValue.Replace("%", key);
                        var percent = Convert.ToDecimal(percentStr);
                        if (percent > 100 && VerifyPercentValue)
                        {
                            if (percent == 100)
                            {
                                NumericValue = "100%";
                            }
                            else
                            {
                                NumericValue = percentStr.Substring(0, 2) + "%";
                            }
                        }
                        else
                        {
                            NumericValue = NumericValue.Insert(NumericValue.Length - 1, key);
                        }
                        return;
                    }
                    NumericValue += key;
                    break;
            }

            //if (key.Equals("."))
            //{
            //    if (NumericValue.Contains("."))
            //    {
            //        return;
            //    }
            //    if (NumericValue.Contains("%"))
            //    {
            //        NumericValue = NumericValue.Remove(NumericValue.Length - 1, 1) + "." + "%";
            //    }
            //    //NumericValue += key;
            //    return;
            //}

            //if (key.Equals("%"))
            //{
            //    NumericValue = NumericValue.Contains("%") ? NumericValue : NumericValue + "%";

            //}

            //if (NumericValue.Contains("%"))
            //{
            //    var percentStr = "";
            //    if (key != "%")
            //    {
            //        percentStr = NumericValue.Remove(NumericValue.Length - 1, 1) + key;
            //    }
            //    else
            //    {
            //        percentStr = NumericValue.Remove(NumericValue.Length - 1, 1);
            //    }
            //    if (percentStr == "")
            //    {
            //        return;
            //    }
            //    var percent = Convert.ToDecimal(percentStr);
            //    if (percent < 0 || percent > 100)
            //    {
            //        NumericValue = percentStr.Substring(0, 2) + "%";
            //    }
            //    else if (key != "%")
            //    {
            //        NumericValue = NumericValue.Remove(NumericValue.Length - 1, 1) + key + "%";
            //    }
            //    return;
            //}

            //NumericValue += key;
        }
    }

    public class DelegateCommandBase : ICommand
    {
        private readonly Action<object> executeMethod;
        private readonly Func<object, bool> canExecuteMethod;

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Createse a new instance of a <see cref="DelegateCommandBase"/>, specifying both the execute action and the can execute function.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action"/> to execute when <see cref="ICommand.Execute"/> is invoked.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{Object,Bool}"/> to invoked when <see cref="ICommand.CanExecute"/> is invoked.</param>
        public DelegateCommandBase(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException("executeMethod", "executeMethod cannot be null.");

            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        public DelegateCommandBase(Action<object> executeMethod)
        {
            if (executeMethod == null)
                throw new ArgumentNullException("executeMethod", "executeMethod cannot be null.");

            this.executeMethod = executeMethod;
            this.canExecuteMethod = null;
        }

        /// <summary>
        /// Raises <see cref="ICommand.CanExecuteChanged"/> on the UI thread so every 
        /// command invoker can requery <see cref="ICommand.CanExecute"/> to check if the
        /// <see cref="CompositeCommand"/> can execute.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises <see cref="DelegateCommandBase.CanExecuteChanged"/> on the UI thread so every command invoker
        /// can requery to check if the command can execute.
        /// <remarks>Note that this will trigger the execution of <see cref="DelegateCommandBase.CanExecute"/> once for each invoker.</remarks>
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        void ICommand.Execute(object parameter)
        {
            Execute(parameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter);
        }

        /// <summary>
        /// Executes the command with the provided parameter by invoking the <see cref="Action{Object}"/> supplied during construction.
        /// </summary>
        /// <param name="parameter"></param>
        protected void Execute(object parameter)
        {
            executeMethod(parameter);
        }

        /// <summary>
        /// Determines if the command can execute with the provided parameter by invoing the <see cref="Func{Object,Bool}"/> supplied during construction.
        /// </summary>
        /// <param name="parameter">The parameter to use when determining if this command can execute.</param>
        /// <returns>Returns <see langword="true"/> if the command can execute.  <see langword="False"/> otherwise.</returns>
        protected bool CanExecute(object parameter)
        {
            return canExecuteMethod == null || canExecuteMethod(parameter);
        }
    }
}
