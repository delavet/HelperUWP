using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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
    public delegate void ReplyRefreshHandler();
    public sealed partial class ReplyBox : UserControl
    {
        private Boolean sent = false;
        public event ReplyRefreshHandler Refresh;
        private int CurPid = 0;
        public ReplyBox()
        {
            this.InitializeComponent();
        }

        public void Show(int pid,String pre = "")
        {
            this.Visibility = Visibility.Visible;
            CurPid = pid;
            STRBDpopin.Begin();
            if (!pre.Equals(""))
            {
                TXTBXreply.Text = pre;
                TXTBXreply.SelectionStart = pre.Length;
            }           
        }

        private void BTNcancel_Click(object sender, RoutedEventArgs e)
        {
            TXTBXreply.Text = "";
            PopOut();
        }

        private async void BTNreply_Click(object sender, RoutedEventArgs e)
        {
            PopOut();
            if (TXTBXreply.Text.Equals(""))
            {
                Constants.BoxPage.ShowMessage("请写点什么");
                return;
            }
            List<Parameters> param = new List<Parameters>();
            param.Add(new Parameters("action", "docomment"));
            param.Add(new Parameters("token", Constants.token));
            param.Add(new Parameters("pid", CurPid.ToString()));
            param.Add(new Parameters("text", TXTBXreply.Text));
            Parameters result = await WebConnection.Connect(Constants.domain + "/services/pkuhole/api.php", param);
            if (result.name != "200")
            {
                Util.DealWithDisconnect(result);
                return;
            }
            else
            {
                Refresh();               
            }
        }

        private void STRBDpopout_Completed(object sender, object e)
        {
            this.Visibility = Visibility.Collapsed;
            ScrollViewer a = new ScrollViewer();
           
        }

        private void PopOut()
        {
            STRBDpopout.Begin();
        }

        private void TXTBXreply_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TXTBXreply.Text.Equals("")) return;
        }

        private void TXTBXreply_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (sent) return;
            if (e.Key == VirtualKey.Enter)
            {
                sent = true;
                BTNreply_Click(null, null);            
            }
        }
    }
}
