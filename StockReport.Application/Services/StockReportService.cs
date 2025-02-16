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
        int rowsAffected = 0 ;
       try
        {
            using (var dbConnection = new SqlConnection(SqlDbconnectionString))
            {
                await dbConnection.OpenAsync();
                using (var transaction = await dbConnection.BeginTransactionAsync())
                {
                    try
                    {
                        var checkQuery = "SELECT COUNT(*) FROM ExcelData WHERE Id = @Id";
                        
                        foreach (var record in records) 
                        {
                            int recordCount = await dbConnection.ExecuteScalarAsync<int>(checkQuery, new { Id = record.Id }, transaction);
                            
                            if (recordCount > 0)
                            {
                                // Update existing record
                                var updateQuery = @"
                                    UPDATE ExcelData
                                    SET 
                                        Name = @Name,
                                        Chg = @Chg,
                                        ChgPrcnt = @ChgPrcnt,
                                        VolM = @VolM,
                                        AverageVol3mM = @AverageVol3mM,
                                        MarketCapM = @MarketCapM,
                                        RevenueM = @RevenueM,
                                        PERatio = @PERatio,
                                        Beta = @Beta,
                                        LastTradePrice = @LastTradePrice,
                                        MovingAvg50DayPrice = @MovingAvg50DayPrice,
                                        MovingAvg200DayPrice = @MovingAvg200DayPrice,
                                        ADX14d = @ADX14d,
                                        ATR14d = @ATR14d,
                                        BullBear13d = @BullBear13d,
                                        CCI14d = @CCI14d,
                                        HighsLows14d = @HighsLows14d,
                                        MACD12d26d = @MACD12d26d,
                                        ROC1dPrcnt = @ROC1dPrcnt,
                                        RSI14d = @RSI14d,
                                        StochasticOscillator14d = @StochasticOscillator14d,
                                        StochasticRSI14d = @StochasticRSI14d,
                                        UltimateOscillator14d = @UltimateOscillator14d,
                                        WilliamsPercentRange = @WilliamsPercentRange,
                                        ChangeFrom52WkHighPrcnt = @ChangeFrom52WkHighPrcnt,
                                        ChangeFrom52WkLowPrcnt = @ChangeFrom52WkLowPrcnt,
                                        NseName = @NseName,
                                        MargineRate = @MargineRate,
                                        PreviousClose = @PreviousClose,
                                        [Open] = @Open,
                                        [Close] = @Close,
                                        High = @High,
                                        Low = @Low,
                                        CreatedDate = @CreatedDate,
                                        LastModifiedDate = @LastModifiedDate,
                                        [Range] = @Range
                                    WHERE Id = @Id;
                                ";

                                await dbConnection.ExecuteAsync(updateQuery, record, transaction);
                            }
                            else
                            {
                                // Insert new record
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
                                    
                                await dbConnection.ExecuteAsync(insertQuery, record, transaction);
                            }
                        }

                        await transaction.CommitAsync();
                        logger.Info($"{this.ClassName()} | {this.MethodName()} | Data processed successfully.");
                        response["statuscode"] = "200";
                        response["message"] = "Data processed successfully.";
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        logger.Fatal($"{this.ClassName()} | {this.MethodName()} |  Exception: {ex}");
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
        }
        catch (Exception ex)
        {
            logger.Fatal($"{this.ClassName()} | {this.MethodName()} |  Exception: {ex}");
            response["statuscode"] = "500";
            response["message"] = "An error occurred.";
            response["error"] = ex.Message;
        }

        return response;

    }

    public async Task<JObject> GetFilteredStockData(string filterValue)
    {
        JObject response = new JObject();
        IEnumerable<StockReportModels> filteredRecords;
        try
        {
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
                    if (filteredRecords.Any())
                    {
                        logger.Info($"{this.ClassName()} | {this.MethodName()} | $'Found {filteredRecords.Count()} matching records.'");

                        response["statuscode"] = "200";
                        response["message"] = "Data retrieved successfully.";
                        response["data"] = JArray.FromObject(filteredRecords);
                    }
                    else
                    {
                        logger.Warn($"{this.ClassName()} | {this.MethodName()} | No filtered records found");

                        response["statuscode"] = "404";
                        response["message"] = "No filtered records found.";
                    }
                }
                catch (Exception ex)
                {
                    logger.Fatal($"{this.ClassName()} | {this.MethodName()} |  Exception: {ex}");
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
        catch(Exception ex){
            logger.Fatal($"{this.ClassName()} | {this.MethodName()} |  Exception: {ex}");
            response["statuscode"] = "500";
            response["message"] = "An error occurred.";
            response["error"] = ex.Message;
        }
        return response;
    }
}
