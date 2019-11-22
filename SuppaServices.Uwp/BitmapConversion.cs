using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace SuppaServices.Uwp
{
    public static class BitmapConversion
    {
        public static BitmapSource ToBitmapSource(this Byte[] imageBytes)
        {
            BitmapImage bmpImage = new BitmapImage();
            MemoryStream mystream = new MemoryStream(imageBytes);
            bmpImage.SetSource(mystream.AsRandomAccessStream());
            return bmpImage;
        }
    }

    public static class StreamExtensions 
    {
        public static async Task<byte[]> ReadBytes(this Stream stream)
        {
            int read;
            var buffer = new byte[stream.Length];
            int receivedBytes = 0;

            while ((read = await stream.ReadAsync(buffer, receivedBytes, buffer.Length)) < receivedBytes)
            {
                receivedBytes += read;
            }

            return buffer;
        }
    }
}