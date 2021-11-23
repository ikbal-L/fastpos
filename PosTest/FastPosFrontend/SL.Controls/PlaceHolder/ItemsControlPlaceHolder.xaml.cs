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

namespace FastPosFrontend.SL.Controls.PlaceHolder
{
    /// <summary>
    /// Interaction logic for ItemsControlPlaceHolder.xaml
    /// </summary>
    public partial class ItemsControlPlaceHolder : UserControl
    {
        public ItemsControlPlaceHolder()
        {
            InitializeComponent();


            //if (ItemsControlPresenter.Content is not ItemsControl ic)
            //{
            //    throw new ArgumentException($"{nameof(Content)} must be of type {nameof(ItemsControl)}");
            //}
            //else
            //{

            //    if (!ic.HasItems)
            //    {
            //        ic.Visibility = Visibility.Collapsed;
            //       
            //    }
            //    else
            //    {
            //        PlaceHolderTextBlock.Visibility = Visibility.Collapsed;
            //    }
            //}
           

        }



        public string PlaceHolderText
        {
            get { return (string)GetValue(PlaceHolderTextProperty); }
            set { SetValue(PlaceHolderTextProperty, value); }
        }



        public Style PlaceholderTextStyle
        {
            get { return (Style)GetValue(PlaceholderTextStyleProperty); }
            set { SetValue(PlaceholderTextStyleProperty, value); }
        }

       
        public static readonly DependencyProperty PlaceholderTextStyleProperty =
            DependencyProperty.Register(nameof(PlaceholderTextStyle), typeof(Style), typeof(ItemsControlPlaceHolder), new PropertyMetadata(default(Style)));



        // Using a DependencyProperty as the backing store for PlaceHolderText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceHolderTextProperty =
            DependencyProperty.Register(nameof(PlaceHolderText), typeof(string), typeof(ItemsControlPlaceHolder), new PropertyMetadata(""));


    }
}
