using System;
using System.Text.RegularExpressions;
using System.Windows.Markup;

namespace FastPosFrontend.Extensions
{
    public class CondExprEx: MarkupExtension
    {

        public string Condition { get; set; }
        public object[] Args { get; set; }

        public string InterpolateArgsIntoConditionString()
        {
            for (int i = 0; i < Args.Length; i++)
            {
                Condition = Regex.Replace(Condition, $"{{{i}}}", Args[i].ToString());
            }

            return Condition;
        }


        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!string.IsNullOrEmpty(Condition) && (Args!= null&& Args.Length>0))
            {
                
                bool.TryParse(Condition,out bool result);
                return result;
            }
            return new InvalidOperationException("Provide the required Arguments");
        }
    }
}