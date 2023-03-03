using System.Text.Json.Serialization;

namespace StockAnalyzer.Core.Models
{
    public class StockApiPricesResponse
    {
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; }

        [JsonPropertyName("queryCount")]
        public int QueryCount { get; set; }

        [JsonPropertyName("resultsCount")]
        public int ResultsCount { get; set; }

        [JsonPropertyName("adjusted")]
        public bool Adjusted { get; set; }

        [JsonPropertyName("results")]
        public List<StockApiPriceItem> Results { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("request_id")]
        public string RequestId { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class StockApiPriceItem
    {
        [JsonPropertyName("c")]
        public decimal ClosePrice { get; set; }

        [JsonPropertyName("t")]
        public long Timestamp { get; set; }
    }
}
