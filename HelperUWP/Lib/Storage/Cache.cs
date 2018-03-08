using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Effects;

namespace HelperUWP.Lib.Storage
{
    class Cache
    {
        public static async Task saveBufferToFile(IBuffer buffer, String filename)
        {
            try
            {
                
                StorageFolder local = Windows.Storage.ApplicationData.Current.LocalCacheFolder;
                StorageFile new_file = await local.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                using (IRandomAccessStream stream = await new_file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await stream.WriteAsync(buffer);
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }

            return;
        }
        public static async Task savePngFile(WriteableBitmap bitmap, String filename)
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalCacheFolder;
            StorageFile sFile = await local.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            if (sFile != null)
            {
                CachedFileManager.DeferUpdates(sFile);
                using (var fileStream = await sFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
                    Stream pixelStream = bitmap.PixelBuffer.AsStream();
                    byte[] pixels = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                              (uint)bitmap.PixelWidth,
                              (uint)bitmap.PixelHeight,
                              96.0,
                              96.0,
                              pixels);
                    await encoder.FlushAsync();
                }
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(sFile);
            }
        }

        public static async Task<StorageFile> getCacheFile(String name)
        {
            try
            {
                StorageFolder local = ApplicationData.Current.LocalCacheFolder;
                StorageFile file = null;
                file = await local.GetFileAsync(name);
                return file;
            }
            catch 
            {
                return null;

            }
        }
    }
}
