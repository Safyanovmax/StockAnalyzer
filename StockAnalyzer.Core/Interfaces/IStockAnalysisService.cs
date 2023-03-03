using StockAnalyzer.Core.Models;

namespace StockAnalyzer.Core.Interfaces
{
    public interface IStockAnalysisService
    {
        Task<PerformanceComparisonResult> GetLastWeekPerformanceComparison(string symbol, string comparedSymbol);
    }
}
