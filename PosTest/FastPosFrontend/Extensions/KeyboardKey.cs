using System;
using System.Windows.Input;
using System.Windows.Markup;

namespace FastPosFrontend.Extensions
{
    [MarkupExtensionReturnType(typeof(Key))]
    public class KeyboardKey: MarkupExtension
    {
        [ConstructorArgument(nameof(Key))]
        public Key Key { get; }

        public KeyboardKey(Key key)
        {
            Key = key;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Key;
        }
    }
}