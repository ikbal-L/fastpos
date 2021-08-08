using System.Windows;
using Caliburn.Micro;
using FastPosFrontend.Helpers;

namespace FastPosFrontend.ViewModels
{
    public class EmbeddedCommandBarViewModel: PropertyChangedBase
    {
        private BindableCollection<EmbeddedCommandBarCommand> _commands;
        private double _size = 40;

        public BindableCollection<EmbeddedCommandBarCommand> Commands
        {
            get => _commands;
            set => Set(ref _commands, value);
        }

        public double Size
        {
            get => _size;
            set => Set(ref _size, value);
        }
    }

    public class EmbeddedContentBarViewModel : PropertyChangedBase
    {
        private DataTemplate _embeddedStatusBarTemplate;
        private object _owner;

        public EmbeddedContentBarViewModel(object owner)
        {
            _owner = owner;
        }

        public DataTemplate EmbeddedStatusBarTemplate
        {
            get => _embeddedStatusBarTemplate;
            set => Set(ref _embeddedStatusBarTemplate, value);
        }

        public object Owner
        {
            get => _owner;
            set => Set(ref _owner, value);
        }
    }

}