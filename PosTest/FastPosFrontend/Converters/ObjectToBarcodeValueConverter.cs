using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace FastPosFrontend.Converters
{
    public class ObjectToBarcodeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value!= null)
            {
                BarcodeLib.Barcode b = new BarcodeLib.Barcode();
                var stringToEncode = ((int)value).ToString("D8", CultureInfo.InvariantCulture);
                var conv = new Int32Converter();
                //int barcodeWidth = (int) conv.ConvertFromString("4cm");
                //var barcodeHeight = (int) conv.ConvertFromString("2.5cm");
                Image image = b.Encode(BarcodeLib.TYPE.EAN8, StringToEncode: stringToEncode);
                var bitmap = new System.Drawing.Bitmap(image);
                var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                var backGround = new ImageBrush(bitmapSource);
                return backGround;
                //return bitmapSource; 
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}