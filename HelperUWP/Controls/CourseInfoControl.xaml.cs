using HelperUWP.CourseRef;
using HelperUWP.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class CourseInfoControl : UserControl
    {
        //public delegate void CourseTapped(object sender, TappedRoutedEventArgs e);
        //public event CourseTapped tapped;
        
        private CourseInfo Info = null;
        private static Random random = new Random();

        public String CourseName
        {
            get
            {
                return TXTBLKcourseName.Text;
            }
            set
            {
                TXTBLKcourseName.Text = value;
            }
        }

        public String Location
        {
            get
            {
                return TXTBLKlocation.Text;
            }
            set
            {
                TXTBLKlocation.Text = value;
            }
        }

        public SolidColorBrush CourseBackground
        {
            set
            {
                GRIDcourse.Background = value;
            }
            get
            {
                return GRIDcourse.Background as SolidColorBrush;
            }
        }

        public CourseInfoControl()
        {
            this.InitializeComponent();
            GRIDcourse.Background = new SolidColorBrush(new Color() { A = 0x6F, R = (byte)random.Next(0, 0xFF), G = (byte)random.Next(0, 0xFF), B = (byte)random.Next(0, 0xFF) });
        }
        public CourseInfoControl(CourseInfo _info)
        {
            this.InitializeComponent();
            Info = _info;
            CourseName = Info.CourseName;
            Location = Info.Location;            
            GRIDcourse.Background = new SolidColorBrush(new Color() { A = 0x6F, R = (byte)random.Next(0, 0xFF), G = (byte)random.Next(0, 0xFF), B = (byte)random.Next(0, 0xFF) });
        }
        
        private void UserControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            String tempStr = "课程名称：" + CourseName +
                "\n上课地点：" + Location +
                "\n考试时间："+Info.Exam+
                "\n考试地点："+Info.ExamPlace+
                "\n其它信息："+Info.Others;
            Constants.BoxPage.ShowMessagePopin(tempStr,"课程详细信息");
        }
        public void CoursePopin()
        {
            STRBDcourse.Begin();            
        }
        public void SetScale0()
        {
            SCALEcourse.ScaleX = SCALEcourse.ScaleY = 0;
        }
    }
}
