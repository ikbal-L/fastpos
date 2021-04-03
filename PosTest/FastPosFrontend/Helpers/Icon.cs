using System.Windows;

namespace FastPosFrontend.Helpers
{
    public class Icon
    {
        public static object Get(string key)
        {
            return Application.Current.FindResource(key);
        }
    }
}