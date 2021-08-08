using Caliburn.Micro;
using FastPosFrontend.Helpers;
using Action = System.Action;

namespace FastPosFrontend.ViewModels
{
    public class AppDialogConductor : Conductor<DialogContent>
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
            _handler?.Invoke();
        }
    }


    public class MainDialog : AppDialogConductor
    {

    }

    public class GenericDialogContentViewModel : DialogContent
    {
        public GenericDialogContentViewModel(object content, string title, params GenericCommand[] commands) : base(
            content, commands)
        {
            Title = title;
        }

        

    }

    public class DefaultDialogBuilder
    {
        public object Content { get; set; }
        public string Title { get; set; }
        public GenericCommand Ok { get; set; }
        public GenericCommand Cancel { get; set; }

    }
}