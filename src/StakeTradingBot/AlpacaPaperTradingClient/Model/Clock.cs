using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace StakeTradingBot.AlpacaPaperTradingClient.Model
{
    public class Clock
    {
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;

        [JsonPropertyName("is_open")]
        public bool IsOpen { get; set; }

        [JsonPropertyName("next_open")]
        public string NextOpen { get; set; }

        [JsonPropertyName("next_close")]
        public string NextClose { get; set; }
    }
}
