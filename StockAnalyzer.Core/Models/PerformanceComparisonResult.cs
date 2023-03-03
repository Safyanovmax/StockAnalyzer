namespace StockAnalyzer.Core.Models
{
    public class PerformanceComparisonResult
    {
        public List<PerformanceResult> StockPerformances { get; set; }
        public List<PerformanceResult> ComparedStockPerformances { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
