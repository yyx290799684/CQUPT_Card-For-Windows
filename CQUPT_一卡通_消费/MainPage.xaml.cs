using Coding4Fun.Toolkit.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍

namespace CQUPT_一卡通_消费
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ApplicationDataContainer _appSetting;
        int page = 1;

        public MainPage()
        {
            _appSetting = ApplicationData.Current.LocalSettings;
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            var statusBar = StatusBar.GetForCurrentView();
            statusBar.HideAsync();
            setUI();
        }

        private void setUI()
        {
            Back.IsEnabled = false;
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
            cardNameTextBlock.Text = "姓名：" + _appSetting.Values["cardName"].ToString(); 
            cardIdTextBlock.Text = "卡号：" + _appSetting.Values["cardId"].ToString();
            if ((bool)_appSetting.Values["cardGet"])
            {
                cardGet();
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
        }

        private void cardGet()
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                var request = HttpWebRequest.Create("http://219.153.62.77/oracle_ykt0529.php?UsrID=" + _appSetting.Values["cardId"] + "&page=" + page);
                request.Method = "GET";
                request.BeginGetResponse(ResponseStreamCallbackPost, request);
            }
            else
            {
                var toast = new ToastPrompt
                {
                    Title = "错误",
                    Message = "网络错误",
                };
                toast.Show();
            }
        }

        private async void ResponseStreamCallbackPost(IAsyncResult result)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)result.AsyncState;
            WebResponse webResponse = httpWebRequest.EndGetResponse(result);
            using (Stream stream = webResponse.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                string mcontent = "{\"data\": " + reader.ReadToEnd() + "}";
                Debug.WriteLine(mcontent);
                string content = ConvertUnicodeStringToChinese(mcontent);
                Debug.WriteLine(content);
                Card mCard = new Card(content);
                List<CardInfo> mCardInfodata = new List<CardInfo>();
                foreach (var item in mCard.Data)
                {
                    mCardInfodata.Add(new CardInfo { je = item.je, jyls = item.jyls, lx = item.lx, sb = item.sb, sh = item.sh, sj = item.sj, xm = item.xm, ye = item.ye });
                }

                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CardListView.ItemsSource = mCardInfodata;
                }
                );
            }
        }

        //UNICODE字符转为中文 
        public static string ConvertUnicodeStringToChinese(string unicodeString)
        {
            if (string.IsNullOrEmpty(unicodeString))
                return string.Empty;

            string outStr = unicodeString;

            Regex re = new Regex("\\\\u[0123456789abcdef]{4}", RegexOptions.IgnoreCase);
            MatchCollection mc = re.Matches(unicodeString);
            foreach (Match ma in mc)
            {
                outStr = outStr.Replace(ma.Value, ConverUnicodeStringToChar(ma.Value).ToString());
            }
            return outStr;
        }

        private static char ConverUnicodeStringToChar(string str)
        {
            char outStr = Char.MinValue;
            outStr = (char)int.Parse(str.Remove(0, 2), System.Globalization.NumberStyles.HexNumber);
            return outStr;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (page == 2)
            {
                page--;
                Back.IsEnabled = false;
                cardGet();
            }
            else if (page >= 3)
            {
                page--;
                cardGet();
            }
            if(page <32)
            {
                Forward.IsEnabled = true;
            }
        }

        private void Rotate_Click(object sender, RoutedEventArgs e)
        {
            cardGet();
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            page++;
            if (page >= 2)
            {
                Back.IsEnabled = true;
            }
            if(page == 32)
            {
                Forward.IsEnabled = false;
            }
            cardGet();
        }


        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)//重写后退按钮，如果要对所有页面使用，可以放在App.Xaml.cs的APP初始化函数中重写。
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                e.Handled = true;
            }

        }

        private void CardListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            _appSetting.Values["cardGet"] = false;
            CardInfo Infoitem = new CardInfo(((CardInfo)e.ClickedItem).jyls, ((CardInfo)e.ClickedItem).xm, ((CardInfo)e.ClickedItem).sj, ((CardInfo)e.ClickedItem).lx, ((CardInfo)e.ClickedItem).je, ((CardInfo)e.ClickedItem).ye, ((CardInfo)e.ClickedItem).sh, ((CardInfo)e.ClickedItem).sb);
            this.Frame.Navigate(typeof(detailPage), Infoitem);
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }

        private void Flag_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(libraryPage));
        }

    }
}
