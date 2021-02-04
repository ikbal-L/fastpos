using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace PosTest.Extensions
{
    public enum AbilityApplicability
    {
        Hide, Collapse, Readonly, Disable
    }
    public class AbilityEx
    {
        public static readonly DependencyProperty AutorityProperty =
                    DependencyProperty.RegisterAttached(
                          "Autority", typeof(string), typeof(AbilityEx),
                          new PropertyMetadata(null, OnAutorityChanged));

        public static readonly DependencyProperty AppliedByProperty =
                    DependencyProperty.RegisterAttached(
                          "AppliedBy", typeof(AbilityApplicability?), typeof(AbilityEx),
                          new PropertyMetadata(null, OnAutorityChanged));

        ////////////////////getters setters 
        public static string GetAutority(DependencyObject d)
        {
            return (string)d.GetValue(AutorityProperty);
        }
        public static void SetAutority(DependencyObject d, string value)
        {
            d.SetValue(AutorityProperty, value);
        }
        public static AbilityApplicability? GetAppliedBy(DependencyObject d)
        {
            return (AbilityApplicability?)d.GetValue(AppliedByProperty);
        }
        public static void SetAppliedBy(DependencyObject d, AbilityApplicability value)
        {
            d.SetValue(AppliedByProperty, value);
        }


        /////////////////////methodes
        private static void OnAutorityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uIElement = d as UIElement;
            if (uIElement == null)
                throw new AbiltyStringException("Can be applayed only on UIElement controls");

            if (GetAppliedBy(d) == null || GetAutority(d) == null)
                return;
            var authBool = Thread.CurrentPrincipal.IsInRole(GetAutority(d));
            switch (GetAppliedBy(d))
            {
                case AbilityApplicability.Collapse:
                    uIElement.Visibility = authBool ? Visibility.Visible : Visibility.Collapsed;
                    break;
                case AbilityApplicability.Hide:
                    uIElement.Visibility = authBool ? Visibility.Visible : Visibility.Hidden;
                    break;
                case AbilityApplicability.Disable:
                    uIElement.IsEnabled = authBool;
                    break;
                case AbilityApplicability.Readonly:
                    if (uIElement is TextBoxBase textBox)
                    {
                        textBox.IsReadOnly = !authBool;
                    }
                    else
                        throw new AbiltyStringException("Can be applayed only on  TextBoxBase controls");
                    break;
            }

        }
       

    }
}