using System;
using System.Text.Json.Serialization;

namespace StakeTradingBot.AlpacaPaperTradingClient.Model
{
    public class AccountModel
    {
        [JsonPropertyName("account_blocked")]
        public bool AccountBlocked { get; set; }

        [JsonPropertyName("account_number")]
        public string AccountNumber { get; set; } = string.Empty;

        [JsonPropertyName("buying_power")]
        public string BuyingPower { get; set; } = string.Empty;

        [JsonPropertyName("cash")]
        public string Cash { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("daytrade_count")]
        public long DaytradeCount { get; set; }

        [JsonPropertyName("daytrading_buying_power")]
        public string DaytradingBuyingPower { get; set; } = string.Empty;

        [JsonPropertyName("equity")]
        public string Equity { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("initial_margin")]
        public string InitialMargin { get; set; } = string.Empty;

        [JsonPropertyName("last_equity")]
        public string LastEquity { get; set; } = string.Empty;

        [JsonPropertyName("last_maintenance_margin")]
        public string LastMaintenanceMargin { get; set; } = string.Empty;

        [JsonPropertyName("long_market_value")]
        public string LongMarketValue { get; set; } = string.Empty;

        [JsonPropertyName("maintenance_margin")]
        public string MaintenanceMargin { get; set; } = string.Empty;

        [JsonPropertyName("multiplier")]
        public string Multiplier { get; set; } = string.Empty;

        [JsonPropertyName("pattern_day_trader")]
        public bool PatternDayTrader { get; set; }

        [JsonPropertyName("portfolio_value")]
        public string PortfolioValue { get; set; } = string.Empty;

        [JsonPropertyName("regt_buying_power")]
        public string RegtBuyingPower { get; set; } = string.Empty;

        [JsonPropertyName("short_market_value")]
        public string ShortMarketValue { get; set; } = string.Empty;

        [JsonPropertyName("shorting_enabled")]
        public bool ShortingEnabled { get; set; }

        [JsonPropertyName("sma")]
        public string Sma { get; set; }

        [JsonPropertyName("status")] 
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("trade_suspended_by_user")]
        public bool TradeSuspendedByUser { get; set; }

        [JsonPropertyName("trading_blocked")]
        public bool TradingBlocked { get; set; }

        [JsonPropertyName("transfers_blocked")]
        public bool TransfersBlocked { get; set; }
    }
}
