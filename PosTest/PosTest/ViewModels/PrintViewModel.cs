using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Caliburn.Micro;

namespace PosTest.ViewModels
{
    class PrintViewModel :Screen
    {
        private FixedDocument _document;
        private object _previousScreen;

        public FixedDocument Document
        {
            get => _document;
            set => Set(ref _document, value);
        }

        public object PreviousScreen
        {
            get => _previousScreen;
            set => Set(ref _previousScreen, value);
        }

        public void NavigateBackToPreviousScreen()
        {
            ((MainViewModel) this.Parent).ActivateItem(PreviousScreen);
        }
    }
}
