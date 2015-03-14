using Coding4Fun.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace CQUPT_一卡通_消费
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class libraryPage : Page
    {
        private ApplicationDataContainer _appSetting;
        public libraryPage()
        {
            _appSetting = ApplicationData.Current.LocalSettings;
            this.InitializeComponent();
            var statusBar = StatusBar.GetForCurrentView();
            statusBar.HideAsync();

        }

        private void libraryGet()
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                var request = HttpWebRequest.Create("http://219.153.62.77/oracle_jscx0428.php?UsrID=" + _appSetting.Values["cardId"]);
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
                if (content.IndexOf("tsmc") == -1)
                {
                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        NullBookStackPanel.Visibility = Visibility.Visible;
                    }
                    );
                }
                else
                {
                    library mLibrary = new library(content);
                    List<libraryInfo> mLibraryInfodata = new List<libraryInfo>();
                    foreach (var item in mLibrary.Data)
                    {
                        mLibraryInfodata.Add(new libraryInfo { tsmc = "《" + item.tsmc + "》", jsrq = "借书日期：" + item.jsrq, yhrq = "应还日期：" + item.yhrq });
                    }

                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        LibraryListView.ItemsSource = mLibraryInfodata;
                    }
                    );
                }

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



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            libraryGet();
            cardNameTextBlock.Text = "姓名：" + _appSetting.Values["cardName"].ToString(); 
            cardIdTextBlock.Text = "卡号：" + _appSetting.Values["cardId"].ToString();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
        }
        //离开页面时，取消事件
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
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

    }
}
