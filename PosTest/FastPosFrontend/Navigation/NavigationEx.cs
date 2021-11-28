using FastPosFrontend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Utilities.Extensions;

namespace FastPosFrontend.Navigation
{
    public class NavigationEx
    {


        public static Type GetNavigationTarget(DependencyObject obj)
        {
            return (Type)obj.GetValue(NavigationTargetProperty);
        }

        public static void SetNavigationTarget(DependencyObject obj, Type value)
        {
            obj.SetValue(NavigationTargetProperty, value);
        }

        
        public static readonly DependencyProperty NavigationTargetProperty =
            DependencyProperty.RegisterAttached("NavigationTargetProperty", typeof(Type), typeof(NavigationEx), new PropertyMetadata(default(Type), NavigationTargetChangedCallBack));


        public static void NavigationTargetChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {


            if (d is Button button)
            {
                button.Click += Button_Click;
            }

            if (d is Hyperlink hyperlink)
            {
                hyperlink.Click += Hyperlink_Click;
            }
        }

        private static void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is DependencyObject d)
            {
                Navigate(d);
            }
        }

        private static void Navigate(DependencyObject hyperlink)
        {
            var navigator = NavigationManager<object>.Instance;
            var target = GetNavigationTarget(hyperlink);
            var navItem = navigator.QuickNavigationItems.FirstOrDefault(i => i.Target == target);

            if (navigator != null)
            {
                navigator.SelectedNavigationItem = navItem;
                DrawerManager.Instance?.CloseAll();
            }

            
        }

        private static void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is DependencyObject d)
            {
                Navigate(d);
            }
        }
    }
}
