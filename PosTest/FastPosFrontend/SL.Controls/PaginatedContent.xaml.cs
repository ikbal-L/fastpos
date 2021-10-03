using FastPosFrontend.Helpers;
using FastPosFrontend.ViewModels;
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

namespace FastPosFrontend.SL.Controls
{
    /// <summary>
    /// Interaction logic for PaginatedContent.xaml
    /// </summary>
    public partial class PaginatedContent : UserControl
    {


        public IPaginator Paginator
        {
            get { return (IPaginator)GetValue(PaginatorProperty); }
            set 
            { 
                SetValue(PaginatorProperty, value);
                if (value!= null)
                {
                    Controller = new PaginationController(value); 
                }
            }
        }

        public PaginationController Controller { get; set; }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PaginatorProperty =
            DependencyProperty.Register("Paginator", typeof(IPaginator), typeof(PaginatedContent),new PropertyMetadata(PaginatorChangedCallBack));

        private static void PaginatorChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var paginatedContent = (PaginatedContent)d;
            var paginator = (IPaginator)paginatedContent.GetValue(PaginatorProperty);
            if (paginator!= null)
            {
                paginatedContent.Controller = new PaginationController(paginator); 
            }
          
        }

        public PaginatedContent()
        {
            InitializeComponent();
        }
    }
}
