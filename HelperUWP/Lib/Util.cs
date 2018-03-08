using HelperUWP.Lib.Web;
using Lumia.Imaging;
using Lumia.Imaging.Adjustments;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace HelperUWP.Lib
{
    class Util
    {
        public static ItemsWrapGrid GetItemsWrapGrid(Windows.UI.Xaml.DependencyObject depObj)
        {
            if (depObj is ItemsWrapGrid)
            {
                return depObj as ItemsWrapGrid;
            }
            for (int i = 0; i < Windows.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(depObj, i);
                var result = GetItemsWrapGrid(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            int count = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }

        public static T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                string controlName = child.GetValue(Control.NameProperty) as string;
                if (controlName == name)
                {
                    return child as T;
                }
                else
                {
                    T result = FindVisualChildByName<T>(child, name);
                    if (result != null)
                        return result;
                }

            }
            return null;
        }

        

        public static VisualStateGroup FindVisualState(FrameworkElement element, string name)
        {
            if (element == null || string.IsNullOrWhiteSpace(name))
                return null;

            IList<VisualStateGroup> groups = VisualStateManager.GetVisualStateGroups(element);
            foreach (var group in groups)
            {
                if (group.Name == name)
                    return group;
            }

            return null;
        }

        public static async Task<WriteableBitmap> Blur(WriteableBitmap srcBmp,int blurLevel)
        {
            using(var stream=await GetPngRanStream(srcBmp))
            {
                try
                {
                    using(var source=new RandomAccessStreamImageSource(stream))
                    using(var effect=new BlurEffect(source, blurLevel))
                    {
                        var target = new WriteableBitmap(srcBmp.PixelWidth, srcBmp.PixelHeight);
                        var renderer = new WriteableBitmapRenderer(effect,target);                 
                        await renderer.RenderAsync();
                        return target;
                        
                    }
                }catch(Exception e)
                {
                    Debug.WriteLine(e.StackTrace+"\r\n");
                    return new WriteableBitmap(1920, 1080);
                }
            }
        }

        public static async Task<FileUpdateStatus> SaveToPngImage(WriteableBitmap bitmap, PickerLocationId location, string fileName)
        {
            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = location
            };
            savePicker.FileTypeChoices.Add("Png Image", new[] { ".png" });
            savePicker.SuggestedFileName = fileName;
            StorageFile sFile = await savePicker.PickSaveFileAsync();
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
                return status;
            }
            return FileUpdateStatus.Failed;
        }

        public static async Task<IRandomAccessStream> GetPngRanStream(WriteableBitmap bmp)
        {
            try
            {
                var pixels = bmp.PixelBuffer.ToArray();
                var displayInformation = DisplayInformation.GetForCurrentView();
                var stream = new InMemoryRandomAccessStream();
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                     BitmapAlphaMode.Premultiplied,
                                     (uint)bmp.PixelWidth,
                                     (uint)bmp.PixelHeight,
                                     96.0,
                                     96.0,
                                     pixels);

                await encoder.FlushAsync();
                stream.Seek(0);
                return stream;
            }catch(Exception e)
            {
                Debug.WriteLine(e.StackTrace+"\r\n");
                return null;
            }
        }

        public static Stream RanStreamToStream(IRandomAccessStream randomStream)
        {
            return WindowsRuntimeStreamExtensions.AsStreamForRead(randomStream.GetInputStreamAt(0));
        }

        public static async Task<InMemoryRandomAccessStream> StreamToRandomAccessStream(Stream stream)
        {
            var randomAccessStream = new InMemoryRandomAccessStream();
            var outputStream = randomAccessStream.GetOutputStreamAt(0);
            await RandomAccessStream.CopyAsync(stream.AsInputStream(), outputStream);
            return randomAccessStream;
        }
        public static void DealWithDisconnect(Parameters parameter)
        {
            if ("-1".Equals(parameter.name))
            {
                Constants.BoxPage.ShowMessage("无法连接网络");
            }
            else
            {
                Constants.BoxPage.ShowMessage("无法连接到服务器 (HTTP " + parameter.name + ")");
            }
        }

        public static DateTime TimestampToDateTime(long timestamp)
        {
            try
            {
                var start = new DateTime(1970, 1, 1, 0, 0, 0);
                return start.AddSeconds(timestamp);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public static long GetTimeStamp(DateTime dt)
        {
            TimeSpan ts = dt - new DateTime(1970, 1, 1, 0, 0, 0, 0,DateTimeKind.Utc);
            long ret = Convert.ToInt64(ts.TotalMilliseconds);
            return ret ;
        }

        private static long MINUTE_SECONDS = 60; //1分钟多少秒
        private static long HOUR_SECONDS = MINUTE_SECONDS * 60;
        private static long DAY_SECONDS = HOUR_SECONDS * 24;
        private static long YEAR_SECONDS = DAY_SECONDS * 365;

        public static String PassedTime(long nowMilliseconds, long oldMilliseconds)
        {
            long passed = (nowMilliseconds - oldMilliseconds) / 1000;//转为秒
            if (passed > YEAR_SECONDS)
            {
                return passed / YEAR_SECONDS + "年前";
            }
            else if (passed > DAY_SECONDS)
            {
                return passed / DAY_SECONDS + "天前";
            }
            else if (passed > HOUR_SECONDS)
            {
                return passed / HOUR_SECONDS + "小时前";
            }
            else if (passed > MINUTE_SECONDS)
            {
                return passed / MINUTE_SECONDS + "分钟前";
            }
            else
            {
                return passed + "秒前";
            }
        }

        public static async Task<StorageFile> ChooseFile(params String[] types)
        {
            try
            {
                StorageFile file = null;
                FileOpenPicker picker = new FileOpenPicker();
                picker.ViewMode = PickerViewMode.Thumbnail;  //设置文件的现实方式，这里选择的是图标
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary; //设置打开时的默认路径，这里选择的是图片库
                foreach (String type_str in types)
                {
                    picker.FileTypeFilter.Add(type_str);                       //添加可选择的文件类型，这个必须要设置

                }
                file = await picker.PickSingleFileAsync();    //只能选择一个文件
                return file;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
    }
}
