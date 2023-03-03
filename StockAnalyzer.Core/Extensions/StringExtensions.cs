using System.Globalization;

namespace StockAnalyzer.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ToAbsPercents(this decimal value)
        {
            return Math.Abs(value).ToString("P", CultureInfo.InvariantCulture);
        } 
    }
}
