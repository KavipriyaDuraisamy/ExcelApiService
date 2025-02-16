using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using Dapper;
using Newtonsoft.Json.Linq;
using NLog;
using StockReport.Core;
using StockReport.Infrastructure;

namespace StockReport.Application;

public class StockReportService : StockReportRepository
{

    private readonly string? SqlDbconnectionString = Extension.SqlDbconnectionString;
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public void Dispose()
    {

    }


   public async Task<JObject> InsertExcelData(List<StockReportModels> records)
{
    JObject response = new JObject();

    using (var dbConnection = new SqlConnection(SqlDbconnectionString))
    {
        await dbConnection.OpenAsync();
        using (var transaction = await dbConnection.BeginTransactionAsync())
        {
            try
            {
                var checkQuery = "SELECT COUNT(*) FROM ExcelData";
                int recordCount = await dbConnection.ExecuteScalarAsync<int>(checkQuery, transaction: transaction);

                if (recordCount > 0)
                {
                    var deleteQuery = "DELETE FROM ExcelData";
                    await dbConnection.ExecuteAsync(deleteQuery, transaction: transaction);
                }

               var insertQuery = @"
    INSERT INTO ExcelData 
    (Id, Name, Chg, ChgPrcnt, VolM, AverageVol3mM, MarketCapM, RevenueM, PERatio, Beta, LastTradePrice,
     MovingAvg50DayPrice, MovingAvg200DayPrice, ADX14d, ATR14d, BullBear13d, CCI14d, HighsLows14d, MACD12d26d, ROC1dPrcnt,
     RSI14d, StochasticOscillator14d, StochasticRSI14d, UltimateOscillator14d, WilliamsPercentRange, 
     ChangeFrom52WkHighPrcnt, ChangeFrom52WkLowPrcnt, NseName, MargineRate, PreviousClose, [Open], [Close], High, Low, 
     CreatedDate, LastModifiedDate, [Range]) 
    VALUES 
    (@Id, @Name, @Chg, @ChgPrcnt, @VolM, @AverageVol3mM, @MarketCapM, @RevenueM, @PERatio, @Beta, @LastTradePrice, 
     @MovingAvg50DayPrice, @MovingAvg200DayPrice, @ADX14d, @ATR14d, @BullBear13d, @CCI14d, @HighsLows14d, @MACD12d26d, 
     @ROC1dPrcnt, @RSI14d, @StochasticOscillator14d, @StochasticRSI14d, @UltimateOscillator14d, @WilliamsPercentRange, 
     @ChangeFrom52WkHighPrcnt, @ChangeFrom52WkLowPrcnt, @NseName, @MargineRate, @PreviousClose, @Open, @Close, @High, 
     @Low, @CreatedDate, @LastModifiedDate, @Range)";


                int rowsAffected = await dbConnection.ExecuteAsync(insertQuery, records, transaction);

                if (rowsAffected > 0)
                {
                    await transaction.CommitAsync();
                    response["statuscode"] = "200";
                    response["message"] = "Data inserted successfully.";
                }
                else
                {
                    await transaction.RollbackAsync();
                    response["statuscode"] = "400";
                    response["message"] = "Data insertion failed.";
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response["statuscode"] = "500";
                response["message"] = "An error occurred.";
                response["error"] = ex.Message;
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                {
                    await dbConnection.CloseAsync();
                }
            }
        }
    }

    return response;
}

   public async Task<JObject> GetFilteredStockData(string filterValue)
{
    JObject response = new JObject();
    IEnumerable<StockReportModels> filteredRecords;

    using (var dbConnection = new SqlConnection(SqlDbconnectionString))
    {
        await dbConnection.OpenAsync();

        try
        {
            if (string.IsNullOrWhiteSpace(filterValue) || filterValue.Trim().ToLower() == "null")
            {
                string selectQuery = "SELECT * FROM ExcelData";
                filteredRecords = await dbConnection.QueryAsync<StockReportModels>(selectQuery);
            }
            else
            {
                Console.WriteLine($"filterValue has a value: '{filterValue}'");

                string query = @"
                SELECT * FROM ExcelData
                WHERE
                    NseName = @FilterValue OR
                    CONVERT(VARCHAR, [Range]) = @FilterValue OR
                    CONVERT(VARCHAR, PERatio) = @FilterValue OR
                    CONVERT(VARCHAR, Beta) = @FilterValue OR
                    CONVERT(VARCHAR, Chg) = @FilterValue OR
                    CONVERT(VARCHAR, VolM) = @FilterValue OR
                    CONVERT(VARCHAR, LastTradePrice) = @FilterValue";

                filteredRecords = await dbConnection.QueryAsync<StockReportModels>(query, new { FilterValue = filterValue });
            }

            if (filteredRecords.Count()>0) // Optimized count check
            {
                response["statuscode"] = "200";
                response["message"] = "Data retrieved successfully.";
                response["data"] = JArray.FromObject(filteredRecords);
            }
            else
            {
                response["statuscode"] = "404";
                response["message"] = "No filtered records found.";
            }
        }
        catch (Exception ex)
        {
            response["statuscode"] = "500";
            response["message"] = "An error occurred.";
            response["error"] = ex.Message;
        }
        finally
        {
            if (dbConnection.State == ConnectionState.Open)
            {
                await dbConnection.CloseAsync();
            }
        }
    }

    return response;
}

}
