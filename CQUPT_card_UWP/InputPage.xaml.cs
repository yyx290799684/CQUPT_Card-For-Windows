using CQUPT_card_UWP.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

namespace CQUPT_card_UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class InputPage : Page
    {

        ApplicationDataContainer appSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
        string cardNameInput = "";
        public InputPage()
        {
            this.InitializeComponent();
            if (appSetting.Values.ContainsKey("cardId") == true)
                cardIdTextBox.Text = appSetting.Values["cardId"].ToString();

        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
        }


        private async void cardIdButton_Click(object sender, RoutedEventArgs e)
        {
            LoginProgressBar.Visibility = Visibility.Visible;
            string money = await NetWork.getHttpWebRequest("oracle_ykt0529.php?UsrID=" + cardIdTextBox.Text + "&page=1");
            Debug.WriteLine("money->" + money);
            if (money != "{\"data\":}")
            {
                if (money != "{\"data\":No record exists!}")
                {
                    try
                    {
                        JObject obj = JObject.Parse(money);
                        string json = obj["data"].ToString();
                        JArray MoneyListArray = (JArray)JsonConvert.DeserializeObject(json);
                        CardInfo mcardInfo = new CardInfo();
                        mcardInfo.GetListAttribute((JObject)MoneyListArray[0]);
                        if (mcardInfo.xm == cardNameTextBox.Text)
                        {
                            string[] xmID = new string[] { cardNameTextBox.Text, cardIdTextBox.Text };
                            appSetting.Values["cardId"] = cardIdTextBox.Text;
                            Frame.Navigate(typeof(MainPage), xmID);
                        }
                        else
                        {
                            Utils.Message("卡号姓名不匹配");
                        }
                    }
                    catch (Exception)
                    {
                        Utils.Message("未知错误");
                    }
                }
                else
                {
                    Utils.Message("此卡不存在");
                }
            }
            else
            {
                Utils.Message("未知错误");
            }
            LoginProgressBar.Visibility = Visibility.Collapsed;
        }

        private void cardIdTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            isLoginButtonEnable();
        }

        private void cardNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            isLoginButtonEnable();
        }
        private void isLoginButtonEnable()
        {
            if (cardIdTextBox.Text != "" && cardNameTextBox.Text != "")
                cardIdButton.IsEnabled = true;
            else
                cardIdButton.IsEnabled = false;
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
