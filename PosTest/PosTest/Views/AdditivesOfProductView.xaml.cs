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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PosTest.ViewModels.SubViewModel;

namespace PosTest.Views
{
    /// <summary>
    /// Interaction logic for AdditivesSettingsView.xaml
    /// </summary>
    public partial class AdditivesOfProductView : UserControl
    {
        public AdditivesOfProductView()
        {
            InitializeComponent();
        }

        public void onClickToggleButton(object sender, RoutedEventArgs args)
        {
            EditProductViewModel editProductViewModel = (EditProductViewModel) this.DataContext;
            //editProductViewModel.CheckAdditive(sender,args,);
        }
    }
}