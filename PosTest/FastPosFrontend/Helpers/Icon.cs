using System.Windows;

namespace FastPosFrontend.Helpers
{
    public class Icon
    {
        public static object Get(string key,string suffix = "Icon",string prefix ="")
        {
            return Application.Current.FindResource($"{prefix}{key}{suffix}");
        }
    }
}