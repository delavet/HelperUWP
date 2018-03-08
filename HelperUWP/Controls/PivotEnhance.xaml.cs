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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace HelperUWP.Controls
{
    /// <summary>
    /// 仅为mailPage的pivot控件设计的视觉增强控件
    /// 仅支持有4个item的pivot，所以只能用在邮件界面
    /// 考虑后续扩充
    /// 目前也在用于正常的装饰用途
    /// </summary>
    public sealed partial class PivotEnhance : UserControl
    {
        private Storyboard[] sbs;
        private Rectangle[] recs;
        private int CurIndex = -1;
        public PivotEnhance()
        {
            this.InitializeComponent();
            sbs = new Storyboard[8];
            sbs[0] = in0;
            sbs[1] = out0;
            sbs[2] = in1;
            sbs[3] = out1;
            sbs[4] = in2;
            sbs[5] = out2;
            sbs[6] = in3;
            sbs[7] = out3;
            recs = new Rectangle[4];
            recs[0] = RECred;
            recs[1] = RECyellow;
            recs[2] = RECblue;
            recs[3] = RECgreen;
        }
        public void UpdateUI(int idx)
        {
            if (idx < 0 || idx > 7) return;
            if (CurIndex >= 0)
            {
                sbs[CurIndex * 2 + 1].Begin();
                Canvas.SetZIndex(recs[CurIndex], 0);
            }
            CurIndex = idx;
            sbs[CurIndex*2].Begin();
            Canvas.SetZIndex(recs[CurIndex], 99);
        }
    }
}
