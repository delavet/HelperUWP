using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class MyProgressBar : UserControl
    {
        private int currentIdx = 0;
        private SolidColorBrush[] colors = new SolidColorBrush[4]
        {
            new SolidColorBrush(Color.FromArgb(0XFF,0XF6,0X53,0X14)),
            new SolidColorBrush(Color.FromArgb(0XFF,0X00,0XA1,0XF1)),
            new SolidColorBrush(Color.FromArgb(0XFF,0X7C,0XBB,0X00)),
            new SolidColorBrush(Color.FromArgb(0XFF,0XFF,0XBB,0X00))
        };
        private SolidColorBrush getNextColor()
        {
            currentIdx = (currentIdx + 1) % 4;
            return colors[currentIdx];
        }
        public MyProgressBar()
        {
            this.InitializeComponent();
        }

        public void ProgressStart()
        {
            GRIDbar.Visibility = Visibility.Visible;
            STRBDprogress1.Begin();
        }

        public void ProgressEnd()
        {
            try
            {
                STRBDprogress1.Stop();
                STRBDprogress2.Stop();
                REC1.Width = REC2.Width = 0;
                GRIDbar.Visibility = Visibility.Collapsed;
            }
            catch { }
        }


        private void STRBDprogress1_Completed(object sender, object e)
        {
            REC2.Fill = getNextColor();
            Canvas.SetZIndex(REC2, 999);
            Canvas.SetZIndex(REC1, 1);
            STRBDprogress2.Begin();
        }

        private void STRBDprogress2_Completed(object sender, object e)
        {
            REC1.Fill = getNextColor();
            Canvas.SetZIndex(REC1, 999);
            Canvas.SetZIndex(REC2, 1);
            STRBDprogress1.Begin();
        }
    }
}
