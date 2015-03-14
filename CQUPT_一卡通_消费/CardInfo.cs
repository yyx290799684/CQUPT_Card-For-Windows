using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace CQUPT_一卡通_消费
{
    class CardInfo
    {
        public string jyls { get; set; }
        public string xm { get; set; }
        public string sj { get; set; }
        public string lx { get; set; }
        public string je { get; set; }
        public string ye { get; set; }
        public string sh { get; set; }
        public string sb { get; set; }

        private const string jylsKey = "jyls";
        private const string xmKey = "xm";
        private const string sjKey = "sj";
        private const string lxKey = "lx";
        private const string jeKey = "je";
        private const string yeKey = "ye";
        private const string shKey = "sh";
        private const string sbKey = "sb";

        public CardInfo()
        {
            jyls = "";
            xm = "";
            sj = "";
            lx = "";
            je = "";
            ye = "";
            sh = "";
            sb = "";
        }

        public CardInfo(string jyls, string xm, string sj, string lx,string je,string ye,string sh,string sb)
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

        public CardInfo(JsonObject jsonObject)
        {
            jyls = jsonObject.GetNamedString(jylsKey, "");
            xm = jsonObject.GetNamedString(xmKey, "");
            sj = jsonObject.GetNamedString(sjKey, "");
            lx = jsonObject.GetNamedString(lxKey, "");
            je = jsonObject.GetNamedString(jeKey, "");
            ye = jsonObject.GetNamedString(yeKey, "");
            sh = jsonObject.GetNamedString(shKey, "");
            sb = jsonObject.GetNamedString(sbKey, "");
        }
    }
}
