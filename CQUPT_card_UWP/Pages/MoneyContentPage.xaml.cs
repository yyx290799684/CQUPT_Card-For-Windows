using CQUPT_card_UWP.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace CQUPT_card_UWP.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MoneyContentPage : Page
    {
        ObservableCollection<Content> ContentInfo = new ObservableCollection<Content>();
        public MoneyContentPage()
        {
            this.InitializeComponent();
            MoneyContentListView.ItemsSource = ContentInfo;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var card = (CardInfo)e.Parameter;
            ContentInfo.Add(new Content { lx = "交易流水：", xq = card.jyls });
            ContentInfo.Add(new Content { lx = "姓名：", xq = card.xm });
            ContentInfo.Add(new Content { lx = "时间：", xq = card.sj });
            ContentInfo.Add(new Content { lx = "金额：", xq = card.je });
            ContentInfo.Add(new Content { lx = "", xq = card.ye });
            ContentInfo.Add(new Content { lx = "商户：", xq = card.sh });
            ContentInfo.Add(new Content { lx = "设备：", xq = card.sb });

            UmengSDK.UmengAnalytics.TrackPageStart("MoneyContentPage");
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            UmengSDK.UmengAnalytics.TrackPageEnd("MoneyContentPage");
        }

    }
}
