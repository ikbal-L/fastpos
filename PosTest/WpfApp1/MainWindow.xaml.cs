using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Resources;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewBase listview;
        ItemsPanelTemplate itemsPanel;
        public MainWindow()
        {
            //isactive = "kkk";
            InitializeComponent();

            this.DataContext = new VewModel();
            listview = lvDataBinding.View;
            itemsPanel = lvDataBinding.ItemsPanel;
            //lvDataBinding.ItemTemplate = (DataTemplate)this.FindResource("MyItemTemplate2");


        }

        private void tempcheck_Checked(object sender, RoutedEventArgs e)
        {
            //lvDataBinding.ItemTemplate = (DataTemplate)this.FindResource("MyItemTemplate2");

            foreach(var item in (DataContext as VewModel).Items)
            {
                item.IsActive = tempcheck.IsChecked;
            }
            (DataContext as VewModel).IsActivev = tempcheck.IsChecked;

            /*lvDataBinding.View = tempcheck.IsChecked == true ? null : listview;

            if (tempcheck.IsChecked == true)
            {
                lvDataBinding.ItemsPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(WrapPanel)));
            }
            else
            {
                lvDataBinding.ItemsPanel = itemsPanel; //new ItemsPanelTemplate(new FrameworkElementFactory(typeof(WrapPanel)));
            }*/

        }
    }

    public class User : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public string Mail { get; set; }

        private bool? _isActive;

        public bool? IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsActive)));
                }
            }
        }


        //public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class VewModel : INotifyPropertyChanged
    {
        private bool? _isActivev;

        public VewModel()
        {
            Items = new ObservableCollection<User>();
            Items.Add(new User() { Name = "John Doe", Age = 42, Mail = "john@doe-family.com",IsActive=true });
            Items.Add(new User() { Name = "Jane Doe", Age = 39, Mail = "jane@doe-family.com" });
            Items.Add(new User() { Name = "Sammy Doe", Age = 13, Mail = "sammy.doe@gmail.com" });
            //lvDataBinding.ItemsSource = Items;
        }
        public ObservableCollection<User> Items { get; set; }
        public bool? IsActivev { 
            get => _isActivev;
            set
            {
                _isActivev = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsActivev)));
                }
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
