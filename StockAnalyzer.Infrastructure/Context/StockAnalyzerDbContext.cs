using Microsoft.EntityFrameworkCore;
using StockAnalyzer.Core.Entities;

namespace StockAnalyzer.Infrastructure.Context
{
    public class StockAnalyzerDbContext : DbContext
    {
        public StockAnalyzerDbContext(DbContextOptions<StockAnalyzerDbContext> options) : base(options)
        {
            Database.Migrate();
        }
        public DbSet<StockPrice> StockPricies { get; set; }
    }
}
