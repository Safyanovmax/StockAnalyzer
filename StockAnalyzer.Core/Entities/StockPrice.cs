namespace StockAnalyzer.Core.Entities
{
    public class StockPrice
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public decimal ClosePrice { get; set; }
    }
}
