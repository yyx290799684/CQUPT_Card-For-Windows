using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace CQUPT_card_UWP
{
    class CardInfo
    {
        public CardInfo() { }
        public CardInfo(string jyls, string xm, string sj, string lx, string je, string ye, string sh, string sb)
        {
            this.jyls = jyls;
            this.xm = xm;
            this.sj = sj;
            this.lx = lx;
            this.je = je;
            this.ye = ye;
            this.sh = sh;
            this.sb = sb;
        }

        public string jyls { get; set; }
        public string xm { get; set; }
        public string sj { get; set; }
        public string lx { get; set; }
        public string je { get; set; }
        public string ye { get; set; }
        public string sh { get; set; }
        public string sb { get; set; }
        public string place { get; set; }

        public void GetListAttribute(JObject JWListJObject)
        {
            jyls = JWListJObject["jyls"].ToString();//流水号
            xm = JWListJObject["xm"].ToString();//姓名
            sj = JWListJObject["sj"].ToString();//时间
            lx = JWListJObject["lx"].ToString();//类型
            je = JWListJObject["je"].ToString();//金额
            ye = "余额：" + JWListJObject["ye"].ToString();//余额
            sh = JWListJObject["sh"].ToString();//商户
            sb = JWListJObject["sb"].ToString();//设备
            place = sh + " " + sb;
        }

    }
}
