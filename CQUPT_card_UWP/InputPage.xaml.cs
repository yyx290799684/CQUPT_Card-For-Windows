using Coding4Fun.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Notifications;
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
    public sealed partial class InputPage : Page
    {

        private ApplicationDataContainer _appSetting;
        string cardNameInput = "";
        public InputPage()
        {
            _appSetting = ApplicationData.Current.LocalSettings; //本地存储
            this.InitializeComponent();
            var statusBar = StatusBar.GetForCurrentView();
            statusBar.HideAsync();
            if (_appSetting.Values.ContainsKey("cardId") == true)
                cardIdTextBox.Text = _appSetting.Values["cardId"].ToString();

        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;//注册重写后退按钮事件
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;//注册重写后退按钮事件
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Application.Current.Exit();
        }


        private void cardIdButton_Click(object sender, RoutedEventArgs e)
        {
            if (cardIdTextBox.Text.Length != 7 && cardNameTextBox.Text.Length != 0)
            {
                var toast = new ToastPrompt
                {
                    Title = "错误",
                    Message = "格式错误",
                };
                toast.Show();
            }
            else
            {
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    cardNameInput = cardNameTextBox.Text.ToString();
                    _appSetting.Values["cardId"] = cardIdTextBox.Text;
                    _appSetting.Values["cardGet"] = true;
                    var request = HttpWebRequest.Create("http://219.153.62.77/oracle_ykt0529.php?UsrID=" + _appSetting.Values["cardId"] + "&page=1");
                    request.Method = "GET";
                    request.BeginGetResponse(ResponseStreamCallbackPost, request);
                    LoginProgressBar.Visibility = Visibility.Visible;
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
        }

        private async void ResponseStreamCallbackPost(IAsyncResult result)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)result.AsyncState;
                WebResponse webResponse = httpWebRequest.EndGetResponse(result);
                using (Stream stream = webResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string mcontent = reader.ReadToEnd();
                    string content = ConvertUnicodeStringToChinese(mcontent);
                    Debug.WriteLine(content);
                    if (content.IndexOf("jyls") != -1)
                    {
                        string cardName = (content.Substring(content.IndexOf("xm") + 5, content.IndexOf("sj") - 3 - content.IndexOf("xm") - 5));
                        bool iscardNameAndcardNameInput;
                        Debug.WriteLine(cardNameInput);
                        Debug.WriteLine(cardName);
                        Debug.WriteLine(cardName.Equals(cardNameInput));
                        iscardNameAndcardNameInput = cardName.Equals(cardNameInput);
                        if (iscardNameAndcardNameInput)
                        {
                            _appSetting.Values["cardName"] = cardName;
                            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                {
                                    Frame.Navigate(typeof(MainPage));
                                });
                        }
                        else
                        {
                            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                {
                                    var toast = new ToastPrompt
                                    {
                                        Title = "错误",
                                        Message = "输入信息有误",
                                    };
                                    toast.Show();
                                    LoginProgressBar.Visibility = Visibility.Collapsed;
                                });
                        }
                    }
                    else
                    {
                        Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            var toast = new ToastPrompt
                            {
                                Title = "错误",
                                Message = "无此卡信息",
                            };
                            toast.Show();
                            LoginProgressBar.Visibility = Visibility.Collapsed;
                        });
                    }
                }
            }
            catch(WebException)
            {
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
                XmlNodeList elements = toastXml.GetElementsByTagName("text");
                elements[0].AppendChild(toastXml.CreateTextNode("网络君开小差了"));
                ToastNotification toast = new ToastNotification(toastXml);
                //toast.Activated += toast_Activated;//点击
                //toast.Dismissed += toast_Dismissed;//消失
                //toast.Failed += toast_Failed;//消除
                ToastNotificationManager.CreateToastNotifier().Show(toast);
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

    }
}
