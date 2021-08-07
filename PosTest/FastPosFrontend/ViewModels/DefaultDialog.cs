using System;
using FastPosFrontend.Helpers;

namespace FastPosFrontend.ViewModels
{
    public static class DefaultDialog

    {
        public static DefaultDialogBuilder New(object content)
        {
            return new DefaultDialogBuilder() {Content = content};
        }

        public static DefaultDialogBuilder Title(this DefaultDialogBuilder builder, string title)
        {
            builder.Title = title;
            return builder;
        }

        public static DefaultDialogBuilder Ok(this DefaultDialogBuilder builder, Action<object> executeMethod,
            Func<object, bool> canExecuteMethod = null, string style = "")
        {
            builder.Ok = new GenericCommand("Ok", executeMethod, canExecuteMethod, style);
            return builder;
        }

        public static GenericDialogContentViewModel Cancel(this DefaultDialogBuilder builder,
            Action<object> executeMethod, Func<object, bool> canExecuteMethod = null, string style = "")
        {
            builder.Cancel = new GenericCommand("Cancel", executeMethod, canExecuteMethod, style);
            return new GenericDialogContentViewModel(builder.Content, builder.Title, builder.Ok, builder.Cancel);
        }
    }
}