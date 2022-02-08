using System.Windows;
using FastPosFrontend.Helpers;
using Caliburn.Micro;

namespace FastPosFrontend.ViewModels
{
    public class EmbeddedCommandBarViewModel: PropertyChangedBase
    {
        public EmbeddedCommandBarViewModel(object context,string resourseKey)
        {
            Context = context;
            DataTemplate = Application.Current.FindResource(resourseKey) as DataTemplate;
        }

        public static EmbeddedCommandBarViewModel Right<T>(T context)
        {
            string resourseKey = $"{typeof(T).Name.Replace("ViewModel","")}RightCommandBar";
            return new EmbeddedCommandBarViewModel(context, resourseKey);
        }

        public static EmbeddedCommandBarViewModel Left<T>(T context)
        {
            string resourseKey = $"{typeof(T).Name.Replace("ViewModel", "")}LeftCommandBar";
            return new EmbeddedCommandBarViewModel(context, resourseKey);
        }

        public object Context { get; private set; }

        public DataTemplate DataTemplate { get; private set; }


    }

   

}