using System.Windows;

namespace FastPosFrontend.Extensions
{
    public abstract class ModifierBase
    {
        public abstract void Apply(DependencyObject target);
    }
}