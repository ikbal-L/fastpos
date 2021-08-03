using System;
using System.Windows.Markup;
using ServiceLib.Service;

namespace FastPosFrontend.Extensions
{
    public class Config : MarkupExtension
    {
        
        
        public override object ProvideValue(IServiceProvider serviceProvider)
        {

            if (string.IsNullOrEmpty(PropertyName)) return null;

            if (!AppConfigurationManager.ContainsKey(PropertyName)) return null;
           
            var propertyValue = AppConfigurationManager.Configuration(PropertyName);
            return propertyValue;

        }

        public string PropertyName { get; set; }
    }

    
}