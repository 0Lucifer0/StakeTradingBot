using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace StakeTradingBot.StakeClient.Model
{
    public class StakeOrder
    {
        [JsonPropertyName("comments")]
        public string Comments { get; set; } = string.Empty;

        [JsonPropertyName("itemId")]
        public string ItemId { get; set; } = string.Empty;

        [JsonPropertyName("itemType")]
        public string ItemType { get; set; } = string.Empty;

        [JsonPropertyName("limitPrice")]
        public string LimitPrice { get; set; } = string.Empty;

        [JsonPropertyName("orderType")]
        public OrderType OrderType { get; set; }

        [JsonPropertyName("quantity")]
        public string Quantity { get; set; } = string.Empty;

        [JsonPropertyName("stopPrice")]
        public string StopPrice { get; set; } = string.Empty;

        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;
    }

    public enum OrderType
    {
        Market,
        Limit,
        Stop,
        StopLimit //stake doesn't support this option yet
    }
}
