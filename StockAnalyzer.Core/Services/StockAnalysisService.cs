using Microsoft.Extensions.Caching.Memory;
using StockAnalyzer.Core.Entities;
using StockAnalyzer.Core.Interfaces;
using StockAnalyzer.Core.Models;

namespace StockAnalyzer.Core.Services
{
    public class StockAnalysisService : IStockAnalysisService
    {
        private readonly IStockApiService _stockApiService;
        private readonly IStockPerformanceService _stockPerformanceService;
        private readonly IDateHelper _weekService;
        private readonly IStockPriceRepository _stockPriceRepository;
        private readonly IStockAnalyzerMemoryCache _memoryCache;

        public StockAnalysisService(IStockApiService stockApiService,
            IStockPerformanceService stockPerformanceService,
            IDateHelper weekService,
            IStockPriceRepository stockPriceRepository,
            IStockAnalyzerMemoryCache memoryCache)
        {
            _stockApiService = stockApiService;
            _stockPerformanceService = stockPerformanceService;
            _weekService = weekService;
            _stockPriceRepository = stockPriceRepository;
            _memoryCache = memoryCache;
        }

        public async Task<PerformanceComparisonResult> GetLastWeekPerformanceComparison(string symbol, string comparedSymbol)
        {
            if (string.IsNullOrEmpty(symbol) || string.IsNullOrEmpty(comparedSymbol))
            {
                return new PerformanceComparisonResult()
                {
                    ErrorMessage = "Symbol cannot be null or empty",
                    Success = false
                };
            }

            var cacheKey = $"{nameof(GetLastWeekPerformanceComparison)}-{symbol}-{comparedSymbol}";
            if (_memoryCache.TryGetValue(cacheKey, out PerformanceComparisonResult performanceComparisonResult))
            {
                return performanceComparisonResult;
            }

            performanceComparisonResult = new PerformanceComparisonResult();

            try
            {
                var stockPrices = await GetOrAddLastWeekPrices(symbol);
                var comparedStockPrices = await GetOrAddLastWeekPrices(comparedSymbol);
                var stockPerformace = _stockPerformanceService.GetStockPerformance(stockPrices);
                var comparedStockPerformance = _stockPerformanceService.GetStockPerformance(comparedStockPrices);
                performanceComparisonResult.StockPerformances = stockPerformace;
                performanceComparisonResult.ComparedStockPerformances = comparedStockPerformance;
                performanceComparisonResult.Success = true;

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(60));
                _memoryCache.Set(cacheKey, performanceComparisonResult, cacheEntryOptions);
            }
            catch (Exception ex)
            {
                performanceComparisonResult.Success = false;
                performanceComparisonResult.ErrorMessage = ex.Message;
            }

            return performanceComparisonResult;
        }

        private async Task<List<StockPrice>> GetOrAddLastWeekPrices(string symbol)
        {
            var startDate = _weekService.GetWeekStartDate();
            var endDate = _weekService.GetWeekEndDate();

            var stockPricesFromDb = await _stockPriceRepository.GetPeriodPrices(symbol, startDate, endDate);
            if (stockPricesFromDb.Count == 0)
            {
                var stockPricesFromApi = await _stockApiService.GetPrices(symbol, startDate, endDate);
                foreach (var price in stockPricesFromApi.Results)
                {
                    var stockPrice = new StockPrice
                    {
                        Date = DateTimeOffset.FromUnixTimeMilliseconds(price.Timestamp).UtcDateTime.Date,
                        ClosePrice = price.ClosePrice,
                        Symbol = symbol
                    };
                    stockPricesFromDb.Add(stockPrice);
                }

                await _stockPriceRepository.AddRange(stockPricesFromDb);
            }

            stockPricesFromDb = stockPricesFromDb.OrderBy(x => x.Date).ToList();
            return stockPricesFromDb;
        }
    }
}

