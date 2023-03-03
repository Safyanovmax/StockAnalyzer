using Microsoft.AspNetCore.Mvc;
using StockAnalyzer.Core.Extensions;
using StockAnalyzer.Core.Interfaces;
using StockAnalyzer.Core.Models;
using StockAnalyzer.WebApi.Models;
using System.Globalization;

namespace StockAnalyzer.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockAnalysisService _stockAnalysisService;
        
        public StockController(IStockAnalysisService stockAnalysisService)
        {
            _stockAnalysisService = stockAnalysisService;
        }

        [HttpGet("performance/{symbol}/spy")]
        public async Task<IActionResult> GetSpyPerformanceComparison([FromRoute] string symbol)
        {
            var performanceComparisonResult = await _stockAnalysisService.GetLastWeekPerformanceComparison(symbol.ToUpper(), "SPY");
            if (!performanceComparisonResult.Success)
            {
                return BadRequest(performanceComparisonResult.ErrorMessage);
            }

            var response = MapModel(performanceComparisonResult);
            return Ok(response);
        }

        private GetSpyPerformanceComparisonResponse MapModel(PerformanceComparisonResult performanceComparisonResult)
        {
            var response = new GetSpyPerformanceComparisonResponse
            {
                Performances = new List<GetSpyPerformanceComparisonResponseItem>()
            };

            foreach (var performance in performanceComparisonResult.StockPerformances)
            {
                var spyPerformance = performanceComparisonResult.ComparedStockPerformances.Single(x => x.Date.DayOfWeek == performance.Date.DayOfWeek);
                var performanceItem = new GetSpyPerformanceComparisonResponseItem
                {
                    Date = performance.Date.ToString("d", CultureInfo.InvariantCulture),
                    SymbolPerformance = performance.StockPerformance.ToAbsPercents(),
                    SpyPerformance = spyPerformance.StockPerformance.ToAbsPercents(),
                    DayOfWeek = performance.Date.DayOfWeek.ToString()
                };
                response.Performances.Add(performanceItem);
            }

            return response;
        }
    }
}
