namespace StockAnalyzer.WebApi.Models
{
    public class GetSpyPerformanceComparisonResponse
    {
        public List<GetSpyPerformanceComparisonResponseItem> Performances { get; set; }
    }

    public class GetSpyPerformanceComparisonResponseItem
    {
        public string SymbolPerformance { get; set; }
        public string SpyPerformance { get; set; }
        public string Date { get; set; }
        public string DayOfWeek { get; set; }
    }
}
