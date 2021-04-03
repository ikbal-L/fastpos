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
}