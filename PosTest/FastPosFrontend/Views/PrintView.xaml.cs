using System.Windows.Controls;

namespace FastPosFrontend.Views
{
    /// <summary>
    /// Interaction logic for PrintView.xaml
    /// </summary>
    public partial class PrintView : UserControl
    {
        public PrintView()
        {
            InitializeComponent();
        }

        //protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    if (e.Property.Name == nameof(DataContext)&& e.NewValue!=null&& e.NewValue is PrintViewModel)
        //    {
        //        Viewer.Document = (DataContext as PrintViewModel).Document;
        //    }
        //}
    }
    
}
