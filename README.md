# Stock Analyzer

How to run:
------------
Prerequisites: 
- .NET 6
- SQL Server
- Visual Studio 2022 (optional)

Using Visual Studio:
1. Open file StockAnalyzer.sln in a Visual Studio
2. Open file appsettings.json and replace a value of "PolygonApiKey" with api key from email and value of "DefaultConnection" with your local MS SQL Server connection string
3. Set StockAnalyzer.WebAPI as startup project 
4. Click on the run button
5. Swagger URL will automatically open

Using command line:
1. Open file appsettings.json and replace a value of "PolygonApiKey" with provided in email api key and value of "DefaultConnection" with your local MS SQL Server connection string
2. Open a solution folder in the command line
3. Run a command "dotnet run --project ./StockAnalyzer.WebAPI/StockAnalyzer.WebAPI.csproj"
4. Go to url https://localhost:7031/swagger/index.html to explore swagger
