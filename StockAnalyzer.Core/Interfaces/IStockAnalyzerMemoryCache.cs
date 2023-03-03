using Microsoft.Extensions.Caching.Memory;

namespace StockAnalyzer.Core.Interfaces
{
    public interface IStockAnalyzerMemoryCache
    {
        bool TryGetValue<TItem>(object key, out TItem value);
        TItem Set<TItem>(object key, TItem value, MemoryCacheEntryOptions options);
    }
}
