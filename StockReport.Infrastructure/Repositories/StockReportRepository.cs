using Newtonsoft.Json.Linq;
using StockReport.Core;

namespace StockReport.Infrastructure;

public interface StockReportRepository:IDisposable
{
    public  Task<JObject> InsertExcelData(List<StockReportModels> records);
    public  Task<JObject> GetFilteredStockData(string filterValue);
} 
