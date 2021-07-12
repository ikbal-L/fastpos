using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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

        public static bool Show(string message, string title)
        {
            var vm = new GenericDialogContentViewModel(message, title,
                new GenericCommand("Yes", o => { Instance.DialogResult = true; Instance.Close();}),
                new GenericCommand("No", o => { Instance.DialogResult = false; Instance.Close();}));
            Instance = new ModalDialogBox(){DataContext = vm};
            var  result = Instance.ShowDialog();
            return result != null && result.Value;
        }
    }
}