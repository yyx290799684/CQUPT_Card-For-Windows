using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace CQUPT_一卡通_消费
{
    class Card
    {
        public ObservableCollection<CardInfo> Data { get; set; }
        private const string DataKey = "data";
        public Card()
        {
            Data = new ObservableCollection<CardInfo>();
        }

         public Card(string jsonString)
            : this()
        {
            JsonObject jsonObject = JsonObject.Parse(jsonString);
            foreach (IJsonValue jsonValue in jsonObject.GetNamedArray(DataKey, new JsonArray()))
            {
                if (jsonValue.ValueType == JsonValueType.Object)
                {
                    Data.Add(new CardInfo(jsonValue.GetObject()));
                }
            }
        }
    }
}
