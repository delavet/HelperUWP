using HelperUWP.CourseRef;
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
    public sealed partial class CourseChooseBlock : UserControl
    {
        public int Day, ClassOfDay;
        private week_kind _kind = week_kind.none;
        private Border REC;
        public week_kind Kind
        {
            get
            {
                return _kind;
            }
            set
            {
                _kind = value;
                change();
            }
        }
        private void change()
        {
            switch (_kind)
            {
                case week_kind.all:
                    {
                        REC.Background = new SolidColorBrush(Color.FromArgb(0xaf, 0xff, 0xbb, 0x00));
                    }
                    break;
                case week_kind.even:
                    {
                        REC.Background = new SolidColorBrush(Color.FromArgb(0xaf, 0x00, 0xa1, 0xf1));
                    }
                    break;
                case week_kind.odd:
                    {
                        REC.Background = new SolidColorBrush(Color.FromArgb(0xaf, 0x7c, 0xbb, 0x00));
                    }
                    break;
                default:
                    REC.Background = new SolidColorBrush(Color.FromArgb(0xaf, 0xff, 0xff, 0xff));
                    break;
            }
        }
        public CourseChooseBlock()
        {
            this.InitializeComponent();
            REC = new Border();
            REC.Background = new SolidColorBrush(Color.FromArgb(0XAF, 0XFF, 0XFF, 0XFF));
            REC.BorderBrush = new SolidColorBrush(Colors.Gray);
            REC.BorderThickness = new Thickness(1);
            GRID.Children.Add(REC);
            REC.HorizontalAlignment = HorizontalAlignment.Stretch;
            REC.VerticalAlignment = VerticalAlignment.Stretch;     
        }
        public CourseChooseBlock(int day,int classOfDay,week_kind kind=week_kind.none)
        {
            this.InitializeComponent();
            
            REC = new Border();
            REC.Background = new SolidColorBrush(Color.FromArgb(0XAF, 0XFF, 0XFF, 0XFF));
            REC.BorderBrush = new SolidColorBrush(Colors.Gray);
            REC.BorderThickness = new Thickness(1);
            GRID.Children.Add(REC);
            REC.HorizontalAlignment = HorizontalAlignment.Stretch;
            REC.VerticalAlignment = VerticalAlignment.Stretch;
            Day = day;
            ClassOfDay = classOfDay;
            Kind = kind;
        }

        private void REC_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Kind = (week_kind)(((int)(_kind + 1)) % 4);
        }
    }
}
