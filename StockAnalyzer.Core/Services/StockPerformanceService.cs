using StockAnalyzer.Core.Entities;
using StockAnalyzer.Core.Interfaces;
using StockAnalyzer.Core.Models;

namespace StockAnalyzer.Core.Services
{
    public class StockPerformanceService : IStockPerformanceService
    {
        public List<PerformanceResult> GetStockPerformance(List<StockPrice> stockPrices)
        {
            var performances = new List<PerformanceResult>();
            var firstPrice = stockPrices[0].ClosePrice;
            foreach (var price in stockPrices)
            {
                var performance = (price.ClosePrice - firstPrice) / firstPrice;
                var performanceItem = new PerformanceResult
                {
                    Date = price.Date,
                    StockPerformance = performance
                };
                performances.Add(performanceItem);
            }
            return performances;
        }
    }
}
