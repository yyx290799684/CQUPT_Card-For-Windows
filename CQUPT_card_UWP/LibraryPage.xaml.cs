using CQUPT_card_UWP.Data;
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
    public sealed partial class LibraryPage : Page
    {
        ApplicationDataContainer appSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
        ObservableCollection<LibraryInfo> libraryInfo = new ObservableCollection<LibraryInfo>();
        public LibraryPage()
        {
            this.InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LibraryListView.ItemsSource = libraryInfo;
            initLibraryList();
            UmengSDK.UmengAnalytics.TrackPageStart("LibraryPage");
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            UmengSDK.UmengAnalytics.TrackPageEnd("LibraryPage");
        }

        private async void initLibraryList()
        {
            string library = await NetWork.getHttpWebRequest("oracle_jscx0428.php?UsrID=" + appSetting.Values["cardId"].ToString());
            Debug.WriteLine("library->" + library);
            LibraryListProgressStackPanel.Visibility = Visibility.Collapsed;
            if (library != "{\"data\":}")
            {
                try
                {
                    JObject obj = JObject.Parse(library);
                    string json = obj["data"].ToString();
                    JArray LibraryListArray = (JArray)JsonConvert.DeserializeObject(json);
                    for (int i = 0; i < LibraryListArray.Count; i++)
                    {
                        LibraryInfo mlibraryInfo = new LibraryInfo();
                        mlibraryInfo.GetListAttribute((JObject)LibraryListArray[i]);
                        libraryInfo.Add(mlibraryInfo);
                        Debug.WriteLine(mlibraryInfo.tsmc);
                    }
                }
                catch (Exception)
                {
                    if (library == "{\"data\":No record exists!}")
                    {
                        NullBookStackPanel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        LibraryListFailedStackPanel.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                LibraryListFailedStackPanel.Visibility = Visibility.Visible;
            }
        }


        private void LibraryRefreshAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            libraryInfo.Clear();
            initLibraryList();
        }

        private void LibraryListFailedStackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            LibraryListFailedStackPanel.Visibility = Visibility.Collapsed;
            initLibraryList();
        }

       
    }
}
