using System;
using Caliburn.Micro;

namespace FastPosFrontend.ViewModels.Settings
{
    public class DialogViewModel : PropertyChangedBase
    {
        private string _leftkind;
        private string _message;
        private string _rightkind;
        private string _rightLabel;
        private string _leftLabel;
        Action<object> _leftAction;
        System.Action _rightAction;
        object _leftActionParam;
        private bool _IsOpenDailog=true;

        public bool IsOpenDailog
        {
            get { return _IsOpenDailog; }
            set { _IsOpenDailog = value;
                NotifyOfPropertyChange(nameof(IsOpenDailog));
            }
        }

        public DialogViewModel( string message,string leftkind, string leftlabel, string rightkind, string rightlabel,
            Action<object> leftAction, object leftActionParam=null, System.Action? rightAction=null)
        {
            Message = message;
            LeftKind = leftkind;
            RightKind = rightkind;
            LeftLabel = leftlabel;
            RightLabel = rightlabel;
            _leftAction = leftAction;
            _leftActionParam = leftActionParam;
            _rightAction = rightAction;
        }

        public string LeftKind 
        {
            get => _leftkind;
            set
            {
                _leftkind = value;
                NotifyOfPropertyChange();
            }
        }
        public string RightKind 
        {
            get => _rightkind;
            set
            {
                _rightkind = value;
                NotifyOfPropertyChange();
            }
        }
        public string LeftLabel
        {
            get => _leftLabel;
            set
            {
                _leftLabel = value;
                NotifyOfPropertyChange();
            }
        }
        
        public string RightLabel 
        {
            get => _rightLabel;
            set
            {
                _rightLabel = value;
                NotifyOfPropertyChange();
            }
        }
        
        public string Message 
        {
            get => _message;
            set
            {
                _message = value;
                NotifyOfPropertyChange();
            }
        }

        public void LeftButtionClick()
        {
            _leftAction(_leftActionParam);

        }
        public void RightButtionClick()
        {
            _rightAction();
            IsOpenDailog = false;
        }
    }
}
