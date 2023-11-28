using Newtonsoft.Json;

namespace CryptoTrade.Models
{
    public partial class CryptoPriceModel
    {
        [JsonProperty("ask")]
        public decimal Ask { get; set; }

        [JsonProperty("totalAsk")]
        public decimal TotalAsk { get; set; }

        [JsonProperty("bid")]
        public decimal Bid { get; set; }

        [JsonProperty("totalBid")]
        public decimal TotalBid { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }
    }

    public partial class CryptoPriceModel
    {
        public static Dictionary<string, CryptoPriceModel>? FromJson(string json) => JsonConvert.DeserializeObject<Dictionary<string, CryptoPriceModel>>(json);
    }
}
