using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace CQUPT_一卡通_消费
{
    class libraryInfo
    {
        public string tsmc { get; set; }
        public string jsrq { get; set; }
        public string yhrq { get; set; }

        private const string tsmcKey = "tsmc";
        private const string jsrqKey = "jsrq";
        private const string yhrqKey = "yhrq";

        public libraryInfo()
        {
            tsmc = "";
            jsrq = "";
            yhrq = "";
        }

        public libraryInfo(JsonObject jsonObject)
        {
            tsmc = jsonObject.GetNamedString(tsmcKey, "");
            jsrq = jsonObject.GetNamedString(jsrqKey, "");
            yhrq = jsonObject.GetNamedString(yhrqKey, "");
         }
    }
}
