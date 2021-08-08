using System.Windows;
using FastPosFrontend.Helpers;
using FastPosFrontend.ViewModels;

namespace FastPosFrontend
{
    /// <summary>
    /// Interaction logic for ModalDialogBox.xaml
    /// </summary>
    public partial class ModalDialogBox : Window
    {
        private ModalDialogBox()
        {
            InitializeComponent();
        }

        private static ModalDialogBox Instance;

        public bool Show()
        {
            var  result = Instance.ShowDialog();
            return result != null && result.Value;
            
        }

        public static ModalDialogBox YesNo(string message, string title)
        {
            var vm = new GenericDialogContentViewModel(message, title,
                new GenericCommand("Yes", o => { Instance.DialogResult = true; Instance.Close(); }),
                new GenericCommand("No", o => { Instance.DialogResult = false; Instance.Close(); }));
            Instance = new ModalDialogBox() { DataContext = vm };
            return Instance;
        }

        public static ModalDialogBox Ok(string message, string title)
        {
            var vm = new GenericDialogContentViewModel(message, title,
                new GenericCommand("Ok", o => { Instance.DialogResult = null; Instance.Close(); }));
            Instance = new ModalDialogBox() { DataContext = vm };
            return Instance;
        }
    }
}