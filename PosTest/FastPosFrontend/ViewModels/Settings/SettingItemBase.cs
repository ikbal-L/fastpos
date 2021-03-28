using Caliburn.Micro;

namespace FastPosFrontend.ViewModels.Settings
{
   public class SettingsItemBase: PropertyChangedBase
    {
        private string _Title;

        public string Title
        {
            get { return _Title; }
            set { _Title = value;
                NotifyOfPropertyChange(nameof(Title));
            }
        }
        private object _Content;

        public object Content
        {
            get { return _Content; }
            set { _Content = value;
                NotifyOfPropertyChange(nameof(Content));
            }
        }
        public int Index { get; set; } = 5;

        public virtual void CommitChanges(){}
        
    }
}
