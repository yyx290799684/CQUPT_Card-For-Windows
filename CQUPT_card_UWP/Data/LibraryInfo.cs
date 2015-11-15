using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQUPT_card_UWP.Data
{
    class LibraryInfo
    {
        public string tsmc { get; set; }
        public string jsrq { get; set; }
        public string yhrq { get; set; }

        public void GetListAttribute(JObject LibraryListJObject)
        {
            tsmc = "《" + LibraryListJObject["tsmc"].ToString() + "》";
            jsrq = "借书时间：" + LibraryListJObject["jsrq"].ToString();
            yhrq = "应还时间：" + LibraryListJObject["yhrq"].ToString();
        }
    }
}
