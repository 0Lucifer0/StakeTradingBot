using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace StakeTradingBot.StakeClient.Model
{
    public class MarketStatusType
    {
        [JsonPropertyName("response")] 
        public Response Response { get; set; } = null!;
    }

    public class Response
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("unixtime")]
        public string Unixtime { get; set; } = string.Empty;

        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;

        [JsonPropertyName("status")] 
        public Status Status { get; set; } = null!;

        [JsonPropertyName("elapsedtime")]
        public string Elapsedtime { get; set; } = string.Empty;

        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("versionNumber")] 
        public string VersionNumber { get; set; } = string.Empty;
    }

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
