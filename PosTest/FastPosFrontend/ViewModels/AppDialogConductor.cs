using System;
using System.Collections.Generic;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using Action = System.Action;

namespace FastPosFrontend.ViewModels
{
    public class AppDialogConductor:Conductor<DialogContent> 
    {
        private bool _isDialogOpen;


        public AppDialogConductor()
        {
            DialogClosed = new DialogClosedHandler();
        }
        

        public void OnClose()
        {
            DialogClosed?.Execute();
        }

        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set => Set(ref _isDialogOpen, value);
        }

        public DialogClosedHandler Open(DialogContent dialogContent) 
        {
            dialogContent.Host = this;
            ActivateItem(dialogContent);
            IsDialogOpen = true;
            return DialogClosed;
        }

        

        public bool Close(DialogContent dialogContent)
        {
            dialogContent.Host = null;
            DeactivateItem(dialogContent, true);
            OnClose();
            IsDialogOpen = false;
            return !IsDialogOpen;
        }

        public bool Close()
        {
            OnClose();
            this.DeactivateItem(this.ActiveItem, true);
            IsDialogOpen = false;
            return !IsDialogOpen;
        }

        public DialogClosedHandler DialogClosed { get; set; }

        
    }
    public class DialogClosedHandler
    {
        private Action _handler;
        public void OnClose(Action handler)
        {
            _handler = handler;
        }

        public void Execute()
        {
            _handler.Invoke();
        }
    }


    public class DialogContent : Screen
    {
        protected object _content;
        private AppDialogConductor _host;

        public DialogContent()
        {
            
        }

        public DialogContent(object content)
        {
            Content = content;
        }
        public DialogContent(object content,IList<GenericCommand> commands)
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

        public AppDialogConductor Host
        {
            get => _host;
            set
            {
                Set(ref _host, value);
                Parent = value;
            }
        }
    }

    public class MainDialog : AppDialogConductor
    {
       
    }

    public class GenericDialogContentViewModel : DialogContent
    {
        public GenericDialogContentViewModel(object content,  params GenericCommand[] commands) : base(content, commands)
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

        public static GenericDialogContentViewModel Cancel(this DefaultDialogBuilder builder,Action<object> executeMethod, Func<object, bool> canExecuteMethod = null, string style = "")
        {
            builder.Cancel =new GenericCommand("Cancel", executeMethod, canExecuteMethod, style);
            return new GenericDialogContentViewModel(builder.Content,builder.Ok,builder.Cancel);
        }
    }

    public  class DefaultDialogBuilder
    {
        public object Content { get; set; }
        public GenericCommand Ok { get; set; }
        public GenericCommand Cancel { get; set; }
        
    }

    
}