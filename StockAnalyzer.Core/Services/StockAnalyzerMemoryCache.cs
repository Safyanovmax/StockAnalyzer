using Microsoft.Extensions.Caching.Memory;
using StockAnalyzer.Core.Interfaces;

namespace StockAnalyzer.Core.Services
{
    public class StockAnalyzerMemoryCache : IStockAnalyzerMemoryCache
    {
        private readonly IMemoryCache _memoryCache;
        public StockAnalyzerMemoryCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public TItem Set<TItem>(object key, TItem value, MemoryCacheEntryOptions options)
        {
            return _memoryCache.Set(key, value, options);
        }

        public bool TryGetValue<TItem>(object key, out TItem value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }
    }
}
