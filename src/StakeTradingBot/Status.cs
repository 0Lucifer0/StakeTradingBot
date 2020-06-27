using System.Text.Json.Serialization;

namespace StakeTradingBot.StakeClient.Model
{
    public class Status
    {
        [JsonPropertyName("change_at")]
        public string ChangeAt { get; set; } = string.Empty;

        [JsonPropertyName("next")]
        public MarketStatus Next { get; set; }

        [JsonPropertyName("current")]
        public MarketStatus Current { get; set; }
    }

    public enum MarketStatus
    {
        Pre,
        Close,
        Open
    }
}