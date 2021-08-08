using System.Windows;

namespace LocalizationUsingSatelliteAssemblies
{
    public class MessageHelper
    {
        

        public  string ERROR_404;
        public  string ERROR500;
        public  string CODE_200;

        public MessageHelper()
        {
            ERROR_404 = Application.Current.FindResource("ERROR404") as string;
            ERROR500 = Application.Current.FindResource("ERROR500") as string;
            CODE_200 = Application.Current.FindResource("ERROR200") as string;
        }


    }
}
