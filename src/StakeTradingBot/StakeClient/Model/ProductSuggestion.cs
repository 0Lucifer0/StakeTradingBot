using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace StakeTradingBot.StakeClient.Model
{
    public class ProductSuggestion
    {
        [JsonPropertyName("instruments")]
        public Instrument[] Instruments { get; set; } = null!;

        [JsonPropertyName("themes")]
        public object[] Themes { get; set; } = null!;

        [JsonPropertyName("instrumentTags")] 
        public object[] InstrumentTags { get; set; } = null!;
    }

    public class Instrument
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("encodedName")]
        public string EncodedName { get; set; } = string.Empty;

        [JsonPropertyName("instrumentId")]
        public Guid InstrumentId { get; set; }

        [JsonPropertyName("imageUrl")]
        public Uri ImageUrl { get; set; } = null!;
    }
}
