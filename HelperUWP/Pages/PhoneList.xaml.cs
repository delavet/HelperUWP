﻿using HelperUWP.InfoRef;
using HelperUWP.Lib;
using HelperUWP.Lib.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Calls;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace HelperUWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PhoneList : Page
    {
        private String Phone= "数学科学学院 62751804 物理学院 62751732 "
            + "化学与分子工程学院 62751710 生命科学学院 62751840 城市与环境学院 62751172 "
            + "地球与空间科学学院 62751150 心理学系 62751831 建筑与景观设计学院 62759003 "
            + "信息科学技术学院 62751760 工学院 62751812 计算机科学技术研究所 82529922 "
            + "软件与微电子学院 61273588 环境科学与工程学院 62751480 中国语言文学系 62751601 "
            + "历史学系 62751652 考古文博学院 62767539 哲学系（宗教学系） 62757589 "
            + "外国语学院 62765007 艺术学院 62751905 对外汉语教育学院 62754120 歌剧研究院 58876072 "
            + "国际关系学院 62751634 经济学院 62751460 光华管理学院 62747033 法学院 62751691 "
            + "信息管理系 13693116736 社会学系 62751676 政府管理学院 62751641 "
            + "马克思主义学院 62751941 教育学院 62751911 新闻与传播学院 62754683 "
            + "人口研究所 62751974 国家发展研究院 62758993 体育教研部 62751900 "
            + "元培学院 62758326 先进技术研究院 62753925 前沿交叉学科研究院 62753237 "
            + "中国社会科学调查中心 62767908 分子医学研究所 62755557 核磁共振中心 62756187 "
            + "北京国际数学研究中心 13910650253 儒藏中心 62767810 深圳研究生院 0755-26035866 "
            + "党委办公室校长办公室 62751201 发展规划部 62751309 纪委办公室监察室 62755622 "
            + "党委组织部 62759120 统战部 62765688 党委宣传部 62751310 "
            + "学生工作部人民武装部 62753595 校园综合治理服务热线 62755110 "
            + "校外车辆入校预约电话 62751321 校园报警电话 62751331 校园火警电话 62752119 "
            + "保密委员会办公室 62754073 教务部（含教务长办公室） 62751430 科学研究部 62751448 "
            + "社会科学部 62751440 研究生院 62758068 继续教育部 62751451 人事部 62759360 "
            + "财务部 62757079 国有资产管理委员会办公室 62757079 国际合作部 62757453 "
            + "实验室与设备管理部 62751411 北京大学实验动物中心 62756977-0 总务部 62751501 "
            + "房地产管理部 62751531 基建工程部 62754131 审计室 62751261 科技开发部 62751371 "
            + "校办产业管理委员会办公室 62751236 211/985办公室 62755571 "
            + "昌平校区管理办公室 89748732 学生就业指导服务中心 62751275 青年研究中心 62763322 "
            + "学生资助中心 400-650-7191 心理健康教育与咨询中心 62760852 工会 62757550 "
            + "团委 62751282 图书馆 62757167 档案馆 62758188 校史馆 62765931 计算中心 62751023 "
            + "现代教育科技中心 1326104911 教育基金会 62759066 校医院 62754213 "
            + "首都发展研究院 82529539 会议中心 62752233 燕园社区服务中心 62752041 "
            + "燕园街道办事处 62751338 北大附中 58751023 北大附小 62760982 餐饮中心 62751541 "
            + "特殊用房管理中心 51605808 水电报修电话 62753319 供暖报修电话 62755433 "
            + "邱德拔体育馆 62750789 校园服务中心 62751561 继续教育学院 62750196 "
            + "公寓服务中心 62756163 校友工作办公室 62758419 招生办公室 62751407";
        private List<Parameters> Phones = new List<Parameters>();
        public PhoneList()
        {
            this.InitializeComponent();
            GenerateList();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            STRBDpopin.Begin();
        }
        private void GenerateList()
        {
            if (Phones == null) Phones = new List<Parameters>();
            else Phones.Clear();
            String[] temp = Phone.Split(new char[] { ' ' });
            for(int i = 0; i < temp.Length; i += 2)
            {
                Phones.Add(new Parameters(temp[i], temp[i + 1]));
            }
            LSTVWphone.ItemsSource = Phones;
        }
        private void BTNback_Click(object sender, RoutedEventArgs e)
        {
            InfoUtil.BackRequest();
        }

        private void LSTVWphone_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!Constants.IsMobile) return;
            Parameters temp = e.ClickedItem as Parameters;
            PhoneCallManager.ShowPhoneCallUI(temp.value, temp.name);
        }
    }
}
