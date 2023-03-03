using Microsoft.EntityFrameworkCore;
using StockAnalyzer.Core.Interfaces;
using StockAnalyzer.Core.Services;
using StockAnalyzer.Infrastructure.Context;
using StockAnalyzer.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StockAnalyzerDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        x => x.MigrationsAssembly("StockAnalyzer.Infrastructure")));

builder.Services.AddScoped<IStockApiService, PolygonStockApiService>();
builder.Services.AddScoped<IStockPerformanceService, StockPerformanceService>();
builder.Services.AddScoped<IStockPriceRepository, StockPriceRepository>();
builder.Services.AddScoped<IStockAnalysisService, StockAnalysisService>();
builder.Services.AddSingleton<IDateHelper, FluentDateHelper>();
builder.Services.AddSingleton<IStockAnalyzerMemoryCache, StockAnalyzerMemoryCache>();

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
