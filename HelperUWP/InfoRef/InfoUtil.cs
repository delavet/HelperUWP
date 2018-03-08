using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using HelperUWP.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace HelperUWP.InfoRef
{
    class InfoUtil
    {
        public static InfoPage infoPage = null;
        public static void BackRequest()
        {
            if (infoPage == null) return;
            infoPage.InnerBackRequest();
        }
        public static async Task GetCardAmount()
        {
            Parameters parameters = await WebConnection.Connect(Constants.domain + "/services/card_amount.php"
                                + "?token=" + Constants.token, null);
            String card_info;
            if (parameters.name != "200")
            {
                Util.DealWithDisconnect(parameters);
                return;
            }
            else
            {
                JsonObject jsonObject;
                try
                {
                    jsonObject = JsonObject.Parse(parameters.value);
                    double amount = jsonObject.GetNamedNumber("data");
                    card_info = "你的校园卡余额是：" + amount;
                }
                catch 
                {
                    Constants.BoxPage.ShowMessage(parameters.value, "获取校园卡信息失败");
                    return;
                }
                
            }
            Constants.BoxPage.ShowMessagePopin(card_info, "校园卡信息");
        }
    }
}
