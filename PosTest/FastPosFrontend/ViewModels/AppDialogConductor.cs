using System;
using System.Collections.Generic;
using Caliburn.Micro;
using FastPosFrontend.Helpers;

namespace FastPosFrontend.ViewModels
{
    public class AppDialogConductor<T>:Conductor<T> where T:Dialog
    {
        
    }


    public class Dialog : Screen
    {
        private object _content;

        public Dialog(object content)
        {
            Content = content;
        }
        public Dialog(object content,IList<GenericCommand> commands)
        {
            Content = content;
            Commands = new BindableCollection<GenericCommand>(commands);
        }
        public BindableCollection<GenericCommand> Commands { get; set; }

        public object Content
        {
            get => _content;
            set => Set(ref _content, value);
        }
    }

    public class MainDialog : AppDialogConductor<Dialog>
    {
        protected override void OnActivate()
        {
           
            //ActivateItem(GenericDialogViewModel.Default("Do you really want to do this Action"));
        }
    }

    public class GenericDialogViewModel : Dialog
    {
        public GenericDialogViewModel(object content,  params GenericCommand[] commands) : base(content, commands)
        {
        }
    }

    public static class DefaultDialog 

    {
        public static DefaultDialogBuilder New(object content)
        {
            return new DefaultDialogBuilder(){Content = content};
        }

        public static DefaultDialogBuilder Ok( this DefaultDialogBuilder builder ,Action<object> executeMethod, Func<object, bool> canExecuteMethod = null, string style = "")
        {
            builder.Ok =new GenericCommand("Ok",executeMethod,canExecuteMethod,style);
            return builder;
        }

        public static GenericDialogViewModel Cancel(this DefaultDialogBuilder builder,Action<object> executeMethod, Func<object, bool> canExecuteMethod = null, string style = "")
        {
            builder.Cancel =new GenericCommand("Cancel", executeMethod, canExecuteMethod, style);
            return new GenericDialogViewModel(builder.Content,builder.Ok,builder.Cancel);
        }
    }

    public  class DefaultDialogBuilder
    {
        public object Content { get; set; }
        public GenericCommand Ok { get; set; }
        public GenericCommand Cancel { get; set; }
        
    }

    
}