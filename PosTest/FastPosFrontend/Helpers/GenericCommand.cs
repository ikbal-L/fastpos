using System;
using System.Windows;

namespace FastPosFrontend.Helpers
{
    public class GenericCommand
    {
        public GenericCommand(object content, Action<object> executeMethod, Func<object, bool> canExecuteMethod = null, string style ="")
        {
            Content = content;
            Command = canExecuteMethod == null
                ? new DelegateCommandBase(executeMethod)
                : new DelegateCommandBase(executeMethod, canExecuteMethod);
            if (!string.IsNullOrEmpty(style))
            {
                Style = Application.Current.FindResource(style) as Style;
            }
        }
            
        public object Content { get; set; }

        public DelegateCommandBase Command { get; set; }

        public bool IsDefault { get; set; } = false;

        public Style Style { get; set; }
    }
}