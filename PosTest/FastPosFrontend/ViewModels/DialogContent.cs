using System.Collections.Generic;
using Caliburn.Micro;
using FastPosFrontend.Helpers;

namespace FastPosFrontend.ViewModels
{
    public class DialogContent : Screen
    {
        protected object _content;
        private AppDialogConductor _host;
        private string _title;

        public DialogContent()
        {

        }

        public DialogContent(object content)
        {
            Content = content;
        }

        public DialogContent(object content, IList<GenericCommand> commands)
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

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
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
}