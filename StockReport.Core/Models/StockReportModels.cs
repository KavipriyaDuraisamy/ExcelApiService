using System;
using System.Text.Json.Serialization;

namespace StockReport.Core;
public class StockReportModels
{
    [JsonPropertyName("Id")]
    public int Id { get; set; }

    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    [JsonPropertyName("Chg")]
    public double Chg { get; set; }

    [JsonPropertyName("ChgPrcnt")]
    public double ChgPrcnt { get; set; }

    [JsonPropertyName("VolM")]
    public double VolM { get; set; }

    [JsonPropertyName("AverageVol3mM")]
    public double AverageVol3mM { get; set; }

    [JsonPropertyName("MarketCapM")]
    public double MarketCapM { get; set; }

    [JsonPropertyName("RevenueM")]
    public double RevenueM { get; set; }

    [JsonPropertyName("PERatio")]
    public double PERatio { get; set; }

    [JsonPropertyName("Beta")]
    public double Beta { get; set; }

    [JsonPropertyName("LastTradePrice")]
    public double LastTradePrice { get; set; }

    [JsonPropertyName("MovingAvg50DayPrice")]
    public double MovingAvg50DayPrice { get; set; }

    [JsonPropertyName("MovingAvg200DayPrice")]
    public double MovingAvg200DayPrice { get; set; }

    [JsonPropertyName("ADX14d")]
    public double ADX14d { get; set; }

    [JsonPropertyName("ATR14d")]
    public double ATR14d { get; set; }

    [JsonPropertyName("BullBear13d")]
    public double BullBear13d { get; set; }

    [JsonPropertyName("CCI14d")]
    public double CCI14d { get; set; }

    [JsonPropertyName("HighsLows14d")]
    public double HighsLows14d { get; set; }

    [JsonPropertyName("MACD12d26d")]
    public double MACD12d26d { get; set; }

    [JsonPropertyName("ROC1dPrcnt")]
    public double ROC1dPrcnt { get; set; }

    [JsonPropertyName("RSI14d")]
    public double RSI14d { get; set; }

    [JsonPropertyName("StochasticOscillator14d")]
    public double StochasticOscillator14d { get; set; }

    [JsonPropertyName("StochasticRSI14d")]
    public double StochasticRSI14d { get; set; }

    [JsonPropertyName("UltimateOscillator14d")]
    public double UltimateOscillator14d { get; set; }

    [JsonPropertyName("WilliamsPercentRange")]
    public double WilliamsPercentRange { get; set; }

    [JsonPropertyName("ChangeFrom52WkHighPrcnt")]
    public double ChangeFrom52WkHighPrcnt { get; set; }

    [JsonPropertyName("ChangeFrom52WkLowPrcnt")]
    public double ChangeFrom52WkLowPrcnt { get; set; }

    [JsonPropertyName("NseName")]
    public string? NseName { get; set; }

    [JsonPropertyName("MargineRate")]
    public double MargineRate { get; set; }

    [JsonPropertyName("PreviousClose")]
    public double PreviousClose { get; set; }

    [JsonPropertyName("Open")]
    public double Open { get; set; }

    [JsonPropertyName("Close")]
    public double Close { get; set; }

    [JsonPropertyName("High")]
    public double High { get; set; }

    [JsonPropertyName("Low")]
    public double Low { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }

    [JsonPropertyName("Range")]
    public double Range { get; set; }
}

