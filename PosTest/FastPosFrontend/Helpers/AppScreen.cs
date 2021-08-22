using System;
using Caliburn.Micro;
using FastPosFrontend.ViewModels;

namespace FastPosFrontend.Helpers
{
    public class AppScreen : Screen, IAppNavigationTarget
    {
        private string _title;
        private EmbeddedCommandBarViewModel _embeddedCommandBar;
        private EmbeddedCommandBarViewModel _embeddedRightCommandBar;

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public EmbeddedCommandBarViewModel EmbeddedCommandBar
        {
            get => _embeddedCommandBar;
            set => Set(ref _embeddedCommandBar, value);
        }

        public EmbeddedCommandBarViewModel EmbeddedRightCommandBar
        {
            get => _embeddedRightCommandBar;
            set => Set(ref _embeddedRightCommandBar, value);
        }

        

        public virtual bool CanNavigate(Type navigationTargetType = null)
        {
            return true;
        }

        public virtual void BeforeNavigateAway(){}
    }
}