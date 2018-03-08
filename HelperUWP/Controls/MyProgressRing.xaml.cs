using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class MyProgressRing : UserControl
    {
        public MyProgressRing()
        {
            this.InitializeComponent();
        }

        public void ProgressStart()
        {
            CVSprogress.Visibility = Visibility.Visible;
            STRBDprogressBegin.Begin();
        }
        public void ProgressEnd()
        {
            STRBDprogress.Stop();
            SCALErec.ScaleX = SCALErec.ScaleY = 1;
            RO1.Angle = RO2.Angle = RO3.Angle = 0;
            CVSprogress.Visibility = Visibility.Collapsed;
        }

        private void STRBDprogressBegin_Completed(object sender, object e)
        {
            STRBDprogress.Begin();
        }
    }
}
