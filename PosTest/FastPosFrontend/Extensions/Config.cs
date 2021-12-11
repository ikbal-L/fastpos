using System;
using System.Windows.Markup;
using FastPosFrontend.Configurations;
using ServiceLib.Service;

namespace FastPosFrontend.Extensions
{
    public class Config : MarkupExtension
    {
        
        
        public override object ProvideValue(IServiceProvider serviceProvider)
        {

            if (string.IsNullOrEmpty(PropertyName)) return null;

            var obj = ConfigurationManager.Get<PosConfig>();

            var path = Property.From(PropertyName);
            var value = Property.GetPropertyValueOf(obj, path);
            return value;

        }

        public string PropertyName { get; set; }


    }

    
}