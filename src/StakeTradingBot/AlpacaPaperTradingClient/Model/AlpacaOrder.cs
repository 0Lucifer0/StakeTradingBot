using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using StakeTradingBot.StakeClient.Model;

namespace StakeTradingBot.AlpacaPaperTradingClient.Model
{
    public class AlpacaOrder
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("qty")]
        public string Quantity { get; set; } = string.Empty;

        [JsonPropertyName("side")]
        public string Side { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("time_in_force")]
        public string TimeInForce { get; set; } = string.Empty;

        [JsonPropertyName("limit_price")]
        public string? LimitPrice { get; set; }

        [JsonPropertyName("stop_price")]
        public string? StopPrice { get; set; }
    }
}
