using CQUPT_card_UWP.Pages;
using CQUPT_card_UWP.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace CQUPT_card_UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MoneyPage : Page
    {
        ApplicationDataContainer appSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
        ObservableCollection<CardInfo> cardInfo = new ObservableCollection<CardInfo>();
        private int page = 1;
        bool show = true;
        public MoneyPage()
        {
            this.InitializeComponent();
            this.SizeChanged += (s, e) =>
            {
                string state = "VisualState0";
                Debug.WriteLine(e.NewSize.Width);
                if (e.NewSize.Width <= 850)
                {
                    MoneyListgrid.Width = e.NewSize.Width;
                    if (MoneyListView.SelectedIndex != -1)
                    {
                        MoneyListgrid.Visibility = Visibility.Collapsed;
                        MoneyContentGrid.Visibility = Visibility.Visible;
                        show = true;
                    }
                    else
                    {
                        MoneyContentGrid.Visibility = Visibility.Collapsed;
                        show = false;
                    }
                }
                if (e.NewSize.Width > 850)
                {
                    state = "VisualState850";
                    MoneyListgrid.Visibility = Visibility.Visible;
                    MoneyListgrid.Width = e.NewSize.Width - 300;
                    show = true;

                }
                VisualStateManager.GoToState(this, state, true);
                cutoffLine.Y2 = e.NewSize.Height;
            };
        }
        public Frame MoneyFrame { get { return this.frame; } }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MoneyListView.ItemsSource = cardInfo;
            initMoneyList();
            UmengSDK.UmengAnalytics.TrackPageStart("MoneyPage");
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (MoneyListgrid.Visibility == Visibility.Collapsed)
                show = false;

            MoneyContentGrid.Visibility = Visibility.Collapsed;
            MoneyListgrid.Visibility = Visibility.Visible;
            MoneyListView.SelectedIndex = -1;
            SystemNavigationManager.GetForCurrentView().BackRequested -= App_BackRequested;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        private async void initMoneyList(int page = 1)
        {
            MoneyListFailedStackPanel.Visibility = Visibility.Collapsed;
            MoneyListProgressStackPanel.Visibility = Visibility.Visible;
            endMoneyGrid.Visibility = Visibility.Collapsed;
#if DEBUG
            string money = await NetWork.getHttpWebRequest("oracle_ykt0529.php?UsrID=1632776&page=" + page);
#else 
            string money = await NetWork.getHttpWebRequest("oracle_ykt0529.php?UsrID=" + appSetting.Values["CardNum"] + "&page=" + page);
#endif
            Debug.WriteLine("money->" + money);
            MoneyListProgressStackPanel.Visibility = Visibility.Collapsed;
            if (money != "")
            {
                try
                {
                    JObject obj = JObject.Parse(money);
                    string json = obj["data"].ToString();
                    JArray MoneyListArray = (JArray)JsonConvert.DeserializeObject(json);
                    for (int i = 0; i < MoneyListArray.Count; i++)
                    {
                        CardInfo mcardInfo = new CardInfo();
                        mcardInfo.GetListAttribute((JObject)MoneyListArray[i]);
                        cardInfo.Add(mcardInfo);
                        Debug.WriteLine(mcardInfo.je);
                    }
                    continueMoneyGrid.Visibility = Visibility.Visible;
                }
                catch (Exception)
                {
                    continueMoneyGrid.Visibility = Visibility.Collapsed;
                    if (money == "{\"data\":No record exists!}")
                    {
                        endMoneyGrid.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MoneyListFailedStackPanel.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                MoneyListFailedStackPanel.Visibility = Visibility.Visible;
                continueMoneyGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void MoneyRefreshAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            page = 1;
            cardInfo.Clear();
            continueMoneyGrid.Visibility = Visibility.Collapsed;
            initMoneyList();
        }

        private void continueMoneyGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            page++;
            initMoneyList(page);
            continueMoneyGrid.Visibility = Visibility.Collapsed;
        }

        private void MoneyListFailedStackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            initMoneyList();
        }

        private void MoneyListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            CardInfo cardInfoitem = new CardInfo(((CardInfo)e.ClickedItem).jyls, ((CardInfo)e.ClickedItem).xm, ((CardInfo)e.ClickedItem).sj, ((CardInfo)e.ClickedItem).lx, ((CardInfo)e.ClickedItem).je, ((CardInfo)e.ClickedItem).ye, ((CardInfo)e.ClickedItem).sh, ((CardInfo)e.ClickedItem).sb);
            MoneyFrame.Visibility = Visibility.Visible;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;

            if (!show)
            {
                MoneyListgrid.Visibility = Visibility.Collapsed;
            }
            MoneyContentGrid.Visibility = Visibility.Visible;
            this.MoneyFrame.Navigate(typeof(MoneyContentPage), cardInfoitem);
        }
    }
}
