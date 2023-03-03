using StockAnalyzer.Core.Models;

namespace StockAnalyzer.Core.Interfaces
{
    public interface IStockApiService
    {
        Task<StockApiPricesResponse> GetPrices(string symbol, DateTime startDate, DateTime endDate);
    }
}
