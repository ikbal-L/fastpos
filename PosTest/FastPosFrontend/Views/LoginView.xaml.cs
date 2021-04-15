using System.Windows.Controls;
using FastPosFrontend.Helpers;
using FastPosFrontend.SL.Controls;

namespace FastPosFrontend.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void VirtualNumpadKeyboard_OnKeyClicked(object sender, VirtualKeyboardKeyClickedEventArgs e)
        {
            ToastNotification.Notify(e.Key);
        }
    }
}
