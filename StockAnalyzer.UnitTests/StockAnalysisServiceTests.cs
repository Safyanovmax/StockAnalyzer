using Moq;
using StockAnalyzer.Core.Services;
using StockAnalyzer.Core.Models;
using StockAnalyzer.Core.Entities;
using StockAnalyzer.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace StockAnalyzer.UnitTests
{
    [TestClass]
    public class StockAnalysisServiceTests
    {
        private StockAnalysisService _stockAnalysisService;
        private Mock<IStockApiService> _mockStockApiService;
        private Mock<IStockPerformanceService> _mockStockPerformanceService;
        private Mock<IDateHelper> _mockWeekService;
        private Mock<IStockPriceRepository> _mockStockPriceRepository;
        private Mock<IStockAnalyzerMemoryCache> _mockStockAnalyzerMemoryCache;

        [TestInitialize]
        public void Initialize()
        {
            _mockStockApiService = new Mock<IStockApiService>();
            _mockStockPerformanceService = new Mock<IStockPerformanceService>();
            _mockWeekService = new Mock<IDateHelper>();
            _mockStockPriceRepository = new Mock<IStockPriceRepository>();
            _mockStockAnalyzerMemoryCache = new Mock<IStockAnalyzerMemoryCache>();
            _stockAnalysisService = new StockAnalysisService(_mockStockApiService.Object, _mockStockPerformanceService.Object, _mockWeekService.Object, _mockStockPriceRepository.Object, _mockStockAnalyzerMemoryCache.Object);
        }

        [TestMethod]
        public async Task GetLastWeekPerformanceComparison_StockPricesFromDb_Success()
        {
            // Arrange
            var symbol = "AAPL";
            var comparedSymbol = "SPY";
            var startDate = new DateTime(2023, 2, 13);
            var endDate = new DateTime(2023, 2, 17);

            var stockPrices = new List<StockPrice>
            {
                new StockPrice { Date = new DateTime(2023, 2, 13), ClosePrice = 100.0M, Symbol = symbol },
                new StockPrice { Date = new DateTime(2023, 2, 14), ClosePrice = 110.0M, Symbol = symbol },
                new StockPrice { Date = new DateTime(2023, 2, 15), ClosePrice = 120.0M, Symbol = symbol },
                new StockPrice { Date = new DateTime(2023, 2, 16), ClosePrice = 130.0M, Symbol = symbol },
                new StockPrice { Date = new DateTime(2023, 2, 17), ClosePrice = 140.0M, Symbol = symbol },
            };

            var comparedStockPrices = new List<StockPrice>
            {
                new StockPrice { Date = new DateTime(2023, 2, 13), ClosePrice = 1000.0M, Symbol = comparedSymbol },
                new StockPrice { Date = new DateTime(2023, 2, 14), ClosePrice = 1200.0M, Symbol = comparedSymbol },
                new StockPrice { Date = new DateTime(2023, 2, 15), ClosePrice = 1400.0M, Symbol = comparedSymbol },
                new StockPrice { Date = new DateTime(2023, 2, 16), ClosePrice = 1600.0M, Symbol = comparedSymbol },
                new StockPrice { Date = new DateTime(2023, 2, 17), ClosePrice = 1800.0M, Symbol = comparedSymbol },
            };

            var stockPerformances = new List<PerformanceResult>
            {
                new PerformanceResult { Date = new DateTime(2022, 2, 13), StockPerformance = 0m },
                new PerformanceResult { Date = new DateTime(2022, 2, 14), StockPerformance = 0.1m },
                new PerformanceResult { Date = new DateTime(2022, 2, 15), StockPerformance = 0.2m },
                new PerformanceResult { Date = new DateTime(2022, 2, 16), StockPerformance = 0.3m },
                new PerformanceResult { Date = new DateTime(2022, 2, 17), StockPerformance = 0.4m },
            };
            var comparedStockPerformances = new List<PerformanceResult>
            {
                new PerformanceResult { Date = new DateTime(2022, 2, 13), StockPerformance = 0m },
                new PerformanceResult { Date = new DateTime(2022, 2, 14), StockPerformance = 0.2m },
                new PerformanceResult { Date = new DateTime(2022, 2, 15), StockPerformance = 0.4m },
                new PerformanceResult { Date = new DateTime(2022, 2, 16), StockPerformance = 0.6m },
                new PerformanceResult { Date = new DateTime(2022, 2, 17), StockPerformance = 0.8m },
            };

            var expected = new PerformanceComparisonResult
            {
                StockPerformances = stockPerformances,
                ComparedStockPerformances = comparedStockPerformances,
                Success = true
            };

            _mockWeekService.Setup(x => x.GetWeekStartDate()).Returns(startDate);
            _mockWeekService.Setup(x => x.GetWeekEndDate()).Returns(endDate);

            _mockStockAnalyzerMemoryCache.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<PerformanceComparisonResult>(), It.IsAny<MemoryCacheEntryOptions>())).Returns(expected);

            _mockStockPriceRepository.Setup(x => x.GetPeriodPrices(symbol, startDate, endDate)).ReturnsAsync(stockPrices);
            _mockStockPriceRepository.Setup(x => x.GetPeriodPrices(comparedSymbol, startDate, endDate)).ReturnsAsync(comparedStockPrices);
            _mockStockPerformanceService.Setup(x => x.GetStockPerformance(stockPrices)).Returns(stockPerformances);
            _mockStockPerformanceService.Setup(x => x.GetStockPerformance(comparedStockPrices)).Returns(comparedStockPerformances);


            // Act
            var result = await _stockAnalysisService.GetLastWeekPerformanceComparison(symbol, comparedSymbol);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.StockPerformances, result.StockPerformances);
            Assert.AreEqual(expected.ComparedStockPerformances, result.ComparedStockPerformances);
            Assert.AreEqual(expected.Success, result.Success);
        }

        [TestMethod]
        public async Task GetLastWeekPerformanceComparison_StockPricesFromApi_Success()
        {
            // Arrange
            var symbol = "AAPL";
            var comparedSymbol = "SPY";
            var startDate = new DateTime(2023, 2, 13);
            var endDate = new DateTime(2023, 2, 17);

            var symbolApiPriceResponse = new StockApiPricesResponse();
            symbolApiPriceResponse.Results = new List<StockApiPriceItem>
            {
                new StockApiPriceItem{ClosePrice = 100.0M, Timestamp = 1676246400000},
                new StockApiPriceItem{ClosePrice = 110.0M, Timestamp = 1676332800000},
                new StockApiPriceItem{ClosePrice = 120.0M, Timestamp = 1676419200000},
                new StockApiPriceItem{ClosePrice = 130.0M, Timestamp = 1676505600000},
                new StockApiPriceItem{ClosePrice = 140.0M, Timestamp = 1676592000000}
            };

            var comparedSymbolApiPriceResponse = new StockApiPricesResponse();
            comparedSymbolApiPriceResponse.Results = new List<StockApiPriceItem>
            {
                new StockApiPriceItem{ClosePrice = 1000.0M, Timestamp = 1676246400000},
                new StockApiPriceItem{ClosePrice = 1200.0M, Timestamp = 1676332800000},
                new StockApiPriceItem{ClosePrice = 1400.0M, Timestamp = 1676419200000},
                new StockApiPriceItem{ClosePrice = 1600.0M, Timestamp = 1676505600000},
                new StockApiPriceItem{ClosePrice = 1800.0M, Timestamp = 1676592000000}
            };

            

            var stockPerformances = new List<PerformanceResult>
            {
                new PerformanceResult { Date = new DateTime(2022, 2, 13), StockPerformance = 0m },
                new PerformanceResult { Date = new DateTime(2022, 2, 14), StockPerformance = 0.1m },
                new PerformanceResult { Date = new DateTime(2022, 2, 15), StockPerformance = 0.2m },
                new PerformanceResult { Date = new DateTime(2022, 2, 16), StockPerformance = 0.3m },
                new PerformanceResult { Date = new DateTime(2022, 2, 17), StockPerformance = 0.4m },
            };
            var comparedStockPerformances = new List<PerformanceResult>
            {
                new PerformanceResult { Date = new DateTime(2022, 2, 13), StockPerformance = 0m },
                new PerformanceResult { Date = new DateTime(2022, 2, 14), StockPerformance = 0.2m },
                new PerformanceResult { Date = new DateTime(2022, 2, 15), StockPerformance = 0.4m },
                new PerformanceResult { Date = new DateTime(2022, 2, 16), StockPerformance = 0.6m },
                new PerformanceResult { Date = new DateTime(2022, 2, 17), StockPerformance = 0.8m },
            };

            var expected = new PerformanceComparisonResult
            {
                StockPerformances = stockPerformances,
                ComparedStockPerformances = comparedStockPerformances,
                Success = true
            };

            _mockWeekService.Setup(x => x.GetWeekStartDate()).Returns(startDate);
            _mockWeekService.Setup(x => x.GetWeekEndDate()).Returns(endDate);

            _mockStockAnalyzerMemoryCache.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<PerformanceComparisonResult>(), It.IsAny<MemoryCacheEntryOptions>())).Returns(expected);

            _mockStockPriceRepository.Setup(x => x.GetPeriodPrices(symbol, startDate, endDate)).ReturnsAsync(new List<StockPrice> { });
            _mockStockPriceRepository.Setup(x => x.GetPeriodPrices(comparedSymbol, startDate, endDate)).ReturnsAsync(new List<StockPrice> { });

            _mockStockApiService.Setup(x => x.GetPrices(symbol, startDate, endDate)).ReturnsAsync(symbolApiPriceResponse);
            _mockStockApiService.Setup(x => x.GetPrices(comparedSymbol, startDate, endDate)).ReturnsAsync(comparedSymbolApiPriceResponse);

            _mockStockPerformanceService.Setup(x => x.GetStockPerformance(It.Is<List<StockPrice>>(y => y.First().ClosePrice == 100.0m))).Returns(stockPerformances);
            _mockStockPerformanceService.Setup(x => x.GetStockPerformance(It.Is<List<StockPrice>>(y => y.First().ClosePrice == 1000.0m))).Returns(comparedStockPerformances);


            // Act
            var result = await _stockAnalysisService.GetLastWeekPerformanceComparison(symbol, comparedSymbol);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.StockPerformances, result.StockPerformances);
            Assert.AreEqual(expected.ComparedStockPerformances, result.ComparedStockPerformances);
            Assert.AreEqual(expected.Success, result.Success);
        }

        [TestMethod]
        public async Task GetLastWeekPerformanceComparison_InvalidSymbol()
        {
            // Arrange
            var symbol = "AAPL";
            var comparedSymbol = "";

            var expected = new PerformanceComparisonResult
            {
                ErrorMessage = "Symbol cannot be null or empty",
                Success = false
            };

            // Act
            var result = await _stockAnalysisService.GetLastWeekPerformanceComparison(symbol, comparedSymbol);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.ErrorMessage, result.ErrorMessage);
            Assert.AreEqual(expected.Success, result.Success);
        }

        [TestMethod]
        public async Task GetLastWeekPerformanceComparison_ApiError_ReturnsUnsuccessfulResult()
        {
            var symbol = "AAPL";
            var comparedSymbol = "SPY";
            var startDate = new DateTime(2023, 2, 13);
            var endDate = new DateTime(2023, 2, 17);

            var stockApiPriceResponse = new StockApiPricesResponse();
            stockApiPriceResponse.Results = new List<StockApiPriceItem>
            {
                new StockApiPriceItem{ClosePrice = 100, Timestamp = 1676239200},
                new StockApiPriceItem{ClosePrice = 100, Timestamp = 1676325600},
                new StockApiPriceItem{ClosePrice = 100, Timestamp = 1676412000},
                new StockApiPriceItem{ClosePrice = 100, Timestamp = 1676498400},
                new StockApiPriceItem{ClosePrice = 100, Timestamp = 1676584800}
            };

            _mockWeekService.Setup(x => x.GetWeekStartDate()).Returns(startDate);
            _mockWeekService.Setup(x => x.GetWeekEndDate()).Returns(endDate);

            _mockStockPriceRepository.Setup(x => x.GetPeriodPrices(symbol, startDate, endDate)).ReturnsAsync(new List<StockPrice> { });
            _mockStockPriceRepository.Setup(x => x.GetPeriodPrices(comparedSymbol, startDate, endDate)).ReturnsAsync(new List<StockPrice> { });

            var errorMessage = "Test error message";

            _mockStockApiService.Setup(x => x.GetPrices(symbol, startDate, endDate)).ThrowsAsync(new Exception(errorMessage));
            _mockStockApiService.Setup(x => x.GetPrices(comparedSymbol, startDate, endDate)).ReturnsAsync(stockApiPriceResponse);

            // Act
            var result = await _stockAnalysisService.GetLastWeekPerformanceComparison(symbol, comparedSymbol);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual(errorMessage, result.ErrorMessage);
            
        }

        [TestMethod]
        public async Task GetLastWeekPerformanceComparison_PerformanceServiceError_ThrowsException()
        {
            // Arrange
            var symbol = "AAPL";
            var comparedSymbol = "GOOGL";
            var startDate = new DateTime(2023, 2, 27);
            var endDate = new DateTime(2023, 3, 5);

            var stockPrices = new List<StockPrice>
            {
                new StockPrice { Date = new DateTime(2023, 2, 13), ClosePrice = 100.0M, Symbol = symbol },
                new StockPrice { Date = new DateTime(2023, 2, 14), ClosePrice = 110.0M, Symbol = symbol },
                new StockPrice { Date = new DateTime(2023, 2, 15), ClosePrice = 120.0M, Symbol = symbol },
                new StockPrice { Date = new DateTime(2023, 2, 16), ClosePrice = 130.0M, Symbol = symbol },
                new StockPrice { Date = new DateTime(2023, 2, 17), ClosePrice = 140.0M, Symbol = symbol },
            };

            var comparedStockPrices = new List<StockPrice>
            {
                new StockPrice { Date = new DateTime(2023, 2, 13), ClosePrice = 1000.0M, Symbol = comparedSymbol },
                new StockPrice { Date = new DateTime(2023, 2, 14), ClosePrice = 1200.0M, Symbol = comparedSymbol },
                new StockPrice { Date = new DateTime(2023, 2, 15), ClosePrice = 1400.0M, Symbol = comparedSymbol },
                new StockPrice { Date = new DateTime(2023, 2, 16), ClosePrice = 1600.0M, Symbol = comparedSymbol },
                new StockPrice { Date = new DateTime(2023, 2, 17), ClosePrice = 1800.0M, Symbol = comparedSymbol },
            };

            _mockWeekService.Setup(x => x.GetWeekStartDate()).Returns(startDate);
            _mockWeekService.Setup(x => x.GetWeekEndDate()).Returns(endDate);

            _mockStockPriceRepository.Setup(x => x.GetPeriodPrices(symbol, startDate, endDate)).ReturnsAsync(stockPrices);
            _mockStockPriceRepository.Setup(x => x.GetPeriodPrices(comparedSymbol, startDate, endDate)).ReturnsAsync(comparedStockPrices);

            var errorMessage = "Test error message";

            _mockStockPerformanceService.Setup(x => x.GetStockPerformance(stockPrices)).Throws(new Exception(errorMessage));

            var result = await _stockAnalysisService.GetLastWeekPerformanceComparison(symbol, comparedSymbol);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(errorMessage, result.ErrorMessage);
        }
    }
}