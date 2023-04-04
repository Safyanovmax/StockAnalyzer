using Microsoft.EntityFrameworkCore;
using StockAnalyzer.Core.Entities;
using StockAnalyzer.Core.Interfaces;
using StockAnalyzer.Infrastructure.Context;

namespace StockAnalyzer.Infrastructure.Services
{
    public class StockPriceRepository : IStockPriceRepository
    {
        private readonly StockAnalyzerDbContext _context;
        private readonly IStockApiService _stockApiService;

        public StockPriceRepository(StockAnalyzerDbContext context, IStockApiService stockApiService)
        {
            _context = context;
            _stockApiService = stockApiService;
        }

        public async Task<List<StockPrice>> GetPeriodPrices(string symbol, DateTime startDate, DateTime endDate)
        {
            var stockPrices = await _context.StockPricies.Where(x => x.Date >= startDate.Date && x.Date <= endDate.Date && x.Symbol == symbol).ToListAsync();
            return stockPrices;
        }

        public async Task AddRange(List<StockPrice> prices)
        {
            try
            {
                _context.AddRange(prices);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_StockPricies_Symbol_Date"))
                    throw new Exception("StockPrices table cannot contains duplicate prices for same date and symbol");
                throw;
            }
        }
    }
}
