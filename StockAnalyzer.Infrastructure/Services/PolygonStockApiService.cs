using Microsoft.Extensions.Configuration;
using StockAnalyzer.Core.Interfaces;
using StockAnalyzer.Core.Models;
using System.Text.Json;

namespace StockAnalyzer.Infrastructure.Services
{
    public class PolygonStockApiService : IStockApiService
    {
        private readonly IConfiguration _configuration;

        public PolygonStockApiService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<StockApiPricesResponse> GetPrices(string symbol, DateTime startDate, DateTime lastDate)
        {
            var apiKey = _configuration["PolygonApiKey"];

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.polygon.io")
            };
            var url = $"/v2/aggs/ticker/{symbol}/range/1/day/{startDate.ToString("yyyy-MM-dd")}/{lastDate.ToString("yyyy-MM-dd")}?adjusted=true&sort=asc&limit=120&apiKey={apiKey}";
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var prices = JsonSerializer.Deserialize<StockApiPricesResponse>(json);

            if (prices == null 
                || prices.Results == null)
            {
                throw new InvalidOperationException("Polygon API request failed");
            }

            return prices;
        }
    }
}
