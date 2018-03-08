using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelperUWP.Lib.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace HelperUWP.Lib
{
    class Constants
    {
        public static MainPage BoxPage = null;
        public static bool IsMobile = false;    //判断设备是否运行windows 10 mobile
        public static double ContentMargin = 24;   //所有UI内容与容器的统一上边距
        private static String _version = "0.0.2";


        /**
         * 以下部分是拟定可以在设定中调整的部分
         */
        public static int BgBluredLevel = 30;           //页面背景图的高斯模糊半径  
        public static Boolean CourseUseElective = true;//是否使用elective（或dean）
        public static Boolean CourseUseCustom = true;   //是否显示自选课表
        public static int Week = 1;                     //课程表的当前周数，可以从服务器获取


        public static WriteableBitmap DefaultImg = new WriteableBitmap(528, 528);
        public static String version
        {
            get
            {
                return _version;
            }
        }
        private static String _update_time = "2015-09-24";
        public static String update_time
        {
            get
            {
                return _update_time;
            }
        }
        private static String _domain = "http://www.pkuhelper.com";
        public static String domain
        {
            get
            {
                return _domain;
            }
        }
        public static String token = "";
        public static String user_token = "";
        public static String birthday = "";
        public static String username = "1400012944";
        public static String password = "Delavet0719";
        public static String name = "";
        public static String major = "";
        public static String sex = "";
        public static String phpsessid = "";
       // public static int week = 0;
        public static Boolean inSchool = false;
        public static Boolean connected = false;
        public static Boolean connectedToNoFree = false;
        public static Boolean hasUpdate = false;
        public static int newMsg = 0;
        public static int newPass = 0;

        public static bool isLogin() { return !("".Equals(token)); }
        public static bool isValidLogin()
        {
            return !("".Equals(token)) && !("12345678".Equals(username)) && !("guest".Equals(username));
        }

        public static void reset()
        {
            Editor.putString("token", "");
            Editor.putString("user_token", "");
            Editor.putString("username", "");
            Editor.putString("password", "");
            Editor.putString("name", "");
            Editor.putString("major", "");
            Editor.putString("sex", "");
            Editor.putString("birthday", "");
            user_token = token = username = password = name = major = sex = "";
        }

        public static void init()
        {
            token = Editor.getString("token");
            user_token = Editor.getString("user_token");
            username = Editor.getString("username");
            password = Editor.getString("password");
            name = Editor.getString("name");
            major = Editor.getString("major");
            sex = Editor.getString("sex");
            birthday = Editor.getString("birthday");
            BgBluredLevel = Editor.getInt(nameof(Constants.BgBluredLevel), 30);
            CourseUseElective = Editor.getBoolean(nameof(Constants.CourseUseElective), true);
            CourseUseCustom = Editor.getBoolean(nameof(Constants.CourseUseCustom), true);

            if ("".Equals(token) || "".Equals(user_token)) reset();
        }
    }
}
