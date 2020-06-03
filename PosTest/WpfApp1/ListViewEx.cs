using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    

    public class ListViewEx : DependencyObject
    {
        public static readonly DependencyProperty IsWrapTemplateProperty =
                    DependencyProperty.RegisterAttached(
                          "IsWrapTemplate", typeof(bool), typeof(ListViewEx),
                          new PropertyMetadata(false, OnIsWrapTemplateChanged));

        static ViewBase view;
        private static void OnIsWrapTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listv = d as ListView;
            if (view == null)
            {
                view = listv.View;
            }

             bool v = (bool)e.NewValue;

            if (v)
            {
                listv.View = null;
            }
            else
            {
                listv.View = view;
            }
        }

        public static bool GetIsWrapTemplate(
            DependencyObject d)
        {
            return (bool)d.GetValue(IsWrapTemplateProperty);
        }
        public static void SetIsWrapTemplate(
            DependencyObject d, bool value)
        {
            d.SetValue(IsWrapTemplateProperty, value);
        }
    }
}
