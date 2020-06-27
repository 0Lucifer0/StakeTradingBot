using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace StakeTradingBot.StakeClient.Model
{
    public class AuthModel
    {
        [JsonPropertyName("userID")]
        public Guid UserId { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("loginState")]
        public object LoginState { get; set; } = string.Empty;

        [JsonPropertyName("commissionRate")]
        public object CommissionRate { get; set; } = string.Empty;

        [JsonPropertyName("wlpID")]
        public object? WlpId { get; set; }

        [JsonPropertyName("referralCode")]
        public string ReferralCode { get; set; } = string.Empty;

        [JsonPropertyName("guest")]
        public object? Guest { get; set; }

        [JsonPropertyName("sessionKey")]
        public string SessionKey { get; set; } = string.Empty;

        [JsonPropertyName("appTypeID")]
        public object? AppTypeId { get; set; }

        [JsonPropertyName("defaultProductDetailPage")]
        public object? DefaultProductDetailPage { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("truliooStatus")]
        public string TruliooStatus { get; set; } = string.Empty;

        [JsonPropertyName("dwStatus")]
        public string DwStatus { get; set; } = string.Empty;

        [JsonPropertyName("macStatus")]
        public string MacStatus { get; set; } = string.Empty;

        [JsonPropertyName("accountType")]
        public string AccountType { get; set; } = string.Empty;

        [JsonPropertyName("regionIdentifier")]
        public string RegionIdentifier { get; set; } = string.Empty;
    }
}
