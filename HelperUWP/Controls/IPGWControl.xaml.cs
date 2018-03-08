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
    public sealed partial class IPGWControl : UserControl
    {
        TranslateTransform translate;
        CompositeTransform scale;
        TranslateTransform pathtrans;
        double center;
        double r;
        private double distans;
        /// &lt;summary&gt;
        /// 已拖动距离
        /// &lt;/summary&gt;
        public double Distans { get { return distans; } }
        /// &lt;summary&gt;
        /// 消除距离
        /// &lt;/summary&gt;
        public double MaxDistans { set { r = value; } }
        /// &lt;summary&gt;
        /// 颜色
        /// &lt;/summary&gt;
        public new Brush Background
        {
            get { return top.Background; }
            set
            {
                top.Background = down.Background = path.Fill = value;
            }
        }
        /// &lt;summary&gt;
        /// 字体颜色
        /// &lt;/summary&gt;
        public new Brush Foreground
        {
            get { return MsgCount.Foreground; }
            set { MsgCount.Foreground = value; }
        }
        /// &lt;summary&gt;
        /// 文字
        /// &lt;/summary&gt;
        public string Text
        {
            get { return MsgCount.Text; }
            set { MsgCount.Text = value; }
        }
        /// &lt;summary&gt;
        /// 大小： 直径
        /// &lt;/summary&gt;
        public double Radius
        {
            get { return top.Width; }
            set
            {
                top.Width = top.Height = down.Width = down.Height = grid.Width = grid.Height = value;

            }
        }
        /// &lt;summary&gt;
        /// 圆角
        /// &lt;/summary&gt;
        public CornerRadius CornerRadius
        {
            get { return top.CornerRadius; }
            set { top.CornerRadius = down.CornerRadius = value; }
        }

        public IPGWControl()
        {
            this.InitializeComponent();
            translate = new TranslateTransform();
            scale = new CompositeTransform();
            pathtrans = new TranslateTransform();
            path.RenderTransform = pathtrans;

            top.RenderTransform = translate;
            down.RenderTransform = scale;

        }
        private void top_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            center = Radius / 2;
            scale.CenterX = scale.CenterY = Radius / 2;
            if (!e.IsInertial)
            {
                translate.X += e.Delta.Translation.X;
                translate.Y += e.Delta.Translation.Y;

                distans = Math.Sqrt(translate.X * translate.X + translate.Y * translate.Y);

                if (distans < r)
                {
                    scale.ScaleX = scale.ScaleY = (r - distans) / r;

                    TransformPath();
                }
                else
                {
                    path.Fill = null;
                    down.Background = null;
                }

            }

        }
        async private void TransformPath()
        {
            distans += Math.Sqrt((1 - scale.ScaleY) * down.ActualHeight * (1 - scale.ScaleY) * down.ActualHeight * 2);

            double cos = translate.Y / distans;
            double sin = translate.X / distans;
            double rdown = Radius / 2 * scale.ScaleY;
            double rtop = Radius / 2;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {

                start1.StartPoint = new Point(center - rdown * cos, center + rdown * sin);
                ber1.Point3 = new Point(center + translate.X - (rtop) * cos, center + translate.Y + (rtop) * sin);
                ber1.Point1 = new Point(translate.X * 1 / 3 + center - (rdown / 2) * cos, translate.Y * 1 / 3 + center + rdown / 2 * sin);
                ber1.Point2 = new Point(translate.X * 2 / 3 + center - (rdown / 2) * cos, translate.Y * 2 / 3 + center + rdown / 2 * sin);

                start2.StartPoint = new Point(center + translate.X + (rtop) * cos, center + translate.Y - (rtop) * sin);//

                ber2.Point3 = new Point(center + rdown * cos, center - rdown * sin);

                ber2.Point1 = new Point(translate.X * 2 / 3 + center + rdown / 2 * cos, translate.Y * 2 / 3 + center - rdown / 2 * sin);

                ber2.Point2 = new Point(translate.X * 1 / 3 + center + rdown / 2 * cos, translate.Y * 1 / 3 + center - rdown / 2 * sin);
                path.Fill = top.Background;
                down.Background = top.Background;

            });
        }
        private void top_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            translate.X = translate.Y = 0;
            scale.ScaleX = scale.ScaleY = (r - distans) / r;
            TransformPath();

        }
    }
}
