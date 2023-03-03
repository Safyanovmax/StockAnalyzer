using StockAnalyzer.Core.Entities;
using StockAnalyzer.Core.Models;

namespace StockAnalyzer.Core.Interfaces
{
    public interface IStockPerformanceService
    {
        List<PerformanceResult> GetStockPerformance(List<StockPrice> stockPrices);
    }
}
