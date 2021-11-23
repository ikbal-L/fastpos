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
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:FastPosFrontend.SL.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:FastPosFrontend.SL.Controls;assembly=FastPosFrontend.SL.Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ItemsControlPlaceHolder/>
    ///
    /// </summary>
    [TemplatePart(Name = "PART_LAYOUT",Type =typeof(ContentControl))]
    [TemplatePart(Name = "PART_ITEMS_CONTROL", Type =typeof(ContentControl))]
    [TemplatePart(Name = "PART_PLACEHOLDER_TEXT", Type =typeof(TextBlock))]
    public class ItemsControlPlaceHolder : Control
    {
        private ContentControl part_items_control;
        private TextBlock part_placeholder_text;

        static ItemsControlPlaceHolder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemsControlPlaceHolder), new FrameworkPropertyMetadata(typeof(ItemsControlPlaceHolder)));
        }

        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        public Style PlaceholderTextStyle
        {
            get { return (Style)GetValue(PlaceholderTextStyleProperty); }
            set { SetValue(PlaceholderTextStyleProperty, value); }
        }

        public ItemsControl ItemsControl
        {
            get { return (ItemsControl)GetValue(ItemsControlProperty); }
            set { SetValue(ItemsControlProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(ItemsControlPlaceHolder), new PropertyMetadata(""));

        public static readonly DependencyProperty PlaceholderTextStyleProperty =
    DependencyProperty.Register(nameof(PlaceholderTextStyleProperty), typeof(Style), typeof(ItemsControlPlaceHolder), new PropertyMetadata(default(Style)));

        public static readonly DependencyProperty ItemsControlProperty =
            DependencyProperty.Register(nameof(ItemsControl), typeof(ItemsControl), typeof(ItemsControlPlaceHolder), new PropertyMetadata(default(ItemsControl)));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            part_items_control =  GetTemplateChild("PART_ITEMS_CONTROL") as ContentControl;
            part_placeholder_text = GetTemplateChild("PART_PLACEHOLDER_TEXT") as TextBlock;
           
            //if (ItemsControl != null && ItemsControl.HasItems)
            //{
            //    part_placeholder_text.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    part_items_control.Visibility = Visibility.Collapsed;
            //}
        }

        




    }
}
