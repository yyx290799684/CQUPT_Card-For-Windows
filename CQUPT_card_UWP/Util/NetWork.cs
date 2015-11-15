using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CQUPT_card_UWP.Util
{
    class NetWork
    {
        public static async Task<string> getHttpWebRequest(string api )
        {
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("", ""));
            string content = "";
            return await Task.Run(() =>
            {
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    try
                    {
                        HttpClient httpClient = new HttpClient();
                        string uri = "http://219.153.62.77/" + api;
                        HttpResponseMessage response = httpClient.PostAsync(new Uri(uri), new FormUrlEncodedContent(paramList)).Result;
                        if (response.StatusCode == HttpStatusCode.OK)
                            content = response.Content.ReadAsStringAsync().Result;
                        //else if (response.StatusCode == HttpStatusCode.NotFound)
                        //    Utils.Message("Oh...服务器又跪了，给我们点时间修好它");

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message + "网络请求异常");
                    }
                }
                else
                {
                }
                return "{\"data\":"+content+"}";

            });
        }
    }
}
