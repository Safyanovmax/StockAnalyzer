using StockAnalyzer.Core.Entities;

namespace StockAnalyzer.Core.Interfaces
{
    public interface IStockPriceRepository
    {
        Task<List<StockPrice>> GetPeriodPrices(string symbol, DateTime startDate, DateTime endDate);
        Task AddRange(List<StockPrice> prices);
    }
}
