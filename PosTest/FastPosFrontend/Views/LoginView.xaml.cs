using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

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

        private void UserControl_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility == System.Windows.Visibility.Visible)
            {
                //FocusManager.SetFocusedElement(this, UserPincode);

                Dispatcher.BeginInvoke((Action)delegate
                {
                    Keyboard.Focus(UserPincode);
                }, DispatcherPriority.Render);
            }
        }

    }
}
