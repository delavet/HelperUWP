using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace HelperUWP.Controls
{
    public sealed partial class MyAudioPlayer : UserControl
    {
        private BackgroundWorker bk;
        private Boolean end = false;
        private object o = new object();
        private TimeSpan pre = new TimeSpan(0);
        Windows.UI.Xaml.Media.FontFamily font = new Windows.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets");
        private enum AudioStatus
        {
            playing, pause, stop
        }
        private AudioStatus status = AudioStatus.stop;
        public Uri AudioSource
        {
            get
            {
                return MEvoice.Source;
            }
            set
            {
                MEvoice.Source = value;
            }
        }
        public int TotalLength
        {
            get
            {
                try
                {
                    var ts = MEvoice.NaturalDuration.TimeSpan;
                    return (int)ts.TotalSeconds;
                }
                catch
                {
                    return 0;
                }
            }
        }
        public void SetSource(IRandomAccessStream s)
        {
            MEvoice.SetSource(s, "audio/amr");
        }
        public MyAudioPlayer()
        {
            this.InitializeComponent();
        }

        private void BTNplay_Click(object sender, RoutedEventArgs e)
        {
            switch (status)
            {
                case AudioStatus.stop:
                    {
                        ICONplay.Glyph = "\xE103";
                        end = false;
                        MEvoice.Position = new TimeSpan(0);
                        pre = new TimeSpan(0);
                        MEvoice.Play();
                        status = AudioStatus.playing;
                        getProgress();
                    }
                    break;
                case AudioStatus.pause:
                    {
                        ICONplay.Glyph = "\xE103";
                        status = AudioStatus.playing;
                        MEvoice.Play();
                    }
                    break;
                case AudioStatus.playing:
                    {
                        ICONplay.Glyph = "\xE102";
                        status = AudioStatus.pause;
                        MEvoice.Pause();
                    }
                    break;
            }

        }
        
        private async void getProgress()
        {
            Boolean tempEnd;
            while (true)
            {               
                lock (o)
                {
                    tempEnd = end;
                }
                if (tempEnd) return;
                await Task.Delay(500);
                if ((long)pre.TotalSeconds == (long)(MEvoice.Position.TotalSeconds) - 1)
                {
                    pre = new TimeSpan(MEvoice.Position.Ticks);
                    String min, sec;
                    if (pre.Minutes < 10)
                    {
                        min = "0" + pre.Minutes;
                    }
                    else min = pre.Minutes.ToString();
                    if (pre.Seconds < 10)
                    {
                        sec = "0" + pre.Seconds;
                    }
                    else sec = pre.Seconds.ToString();
                    TXTBLKprogress.Text = min+":"+sec;
                }
            }
        }

        

        private void MEvoice_MediaEnded(object sender, RoutedEventArgs e)
        {
            lock (o)
            {
                end = true;
            }
            status = AudioStatus.stop;
            ICONplay.Glyph = "\xE102";
        }
    }
}
