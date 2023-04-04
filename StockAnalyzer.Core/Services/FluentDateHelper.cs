using FluentDate;
using FluentDateTime;
using StockAnalyzer.Core.Interfaces;

namespace StockAnalyzer.Core.Services
{
    public class FluentDateHelper : IDateHelper
    {
        public DateTime GetWeekStartDate()
        {
            var lastWeekDate = 1.Weeks().Ago().Date;
            var startDate = lastWeekDate.Previous(DayOfWeek.Monday);
            return startDate;
        }

        public DateTime GetWeekEndDate()
        {
            var endDate = GetWeekStartDate().Next(DayOfWeek.Friday);
            return endDate;
        }
    }
}
